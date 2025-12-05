using System;
using System.Diagnostics;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;

    [Header("Settings")]
    [SerializeField] private bool _useRollingSimulation = true;
    [SerializeField] private bool _useForNeighborsFind = false;

    private const byte CellDead = 0;
    private const byte CellAlive = 1;
    private const byte CellBlocked = 2;

    private bool _simulationRunning = false;


    private void Awake()
    {
        if (_gridManager == null)
        {
            UnityEngine.Debug.LogError("GridManager reference is missing in Simulation script.");
            return;
        }

        _gridManager.Init();
    }

    private void OnDisable()
    {
        if (TickManager.Instance == null) return;

        TickManager.Instance.OnTick -= OnTick;
    }

    private void OnTick()
    {
        if (_useRollingSimulation)
        {
            RollingSimulation();
        }
        else
        {
            ForSimulation();
        }
    }

    private void RollingSimulation()
    {
        var timer = Stopwatch.StartNew();

        int width = _gridManager.Grid.Width;
        int height = _gridManager.Grid.Height;
        int paddedWidth = width + 2;

        byte[] current = _gridManager.Grid.CellPadded;
        byte[] next = new byte[paddedWidth * (height + 2)];
        byte[] horizontalSums = new byte[next.Length];

        ComputeHorizontalSums(width, height, paddedWidth, current, horizontalSums);
        ApplyRulesWithVerticalSums(width, height, paddedWidth, current, horizontalSums, next);

        timer.Stop();
        StatsMenuController.Instance.UpdateSimulationCalcTime(timer.ElapsedMilliseconds);

        _gridManager.SetCells(next);
    }

    private static void ComputeHorizontalSums(int width, int height, int paddedWidth, byte[] current, byte[] horizontalSums)
    {
        for (int y = 1; y <= height; y++)
        {
            int rowStart = y * paddedWidth;

            int index = rowStart + 1;
            int sum = current[index - 1] + current[index] + current[index + 1];
            horizontalSums[index] = (byte)sum;

            for (int x = 2; x <= width; x++)
            {
                index++;
                sum += current[index + 1] - current[index - 2]; // slide window
                horizontalSums[index] = (byte)sum;
            }
        }
    }

    private static void ApplyRulesWithVerticalSums(int width, int height, int paddedWidth, byte[] current, byte[] horizontalSums, byte[] output)
    {
        for (int x = 1; x <= width; x++)
        {
            int sum = horizontalSums[x]
                + horizontalSums[paddedWidth + x]
                + horizontalSums[(2 * paddedWidth) + x];

            for (int y = 1; y <= height; y++)
            {
                int index = (y * paddedWidth) + x;
                int neighbors = sum - current[index];

                output[index] = NextCellState(current[index], neighbors);

                if (y < height)
                {
                    int oldTop = ((y - 1) * paddedWidth) + x;
                    int newBottom = ((y + 2) * paddedWidth) + x;
                    sum += horizontalSums[newBottom] - horizontalSums[oldTop]; // slide window
                }
            }
        }
    }

    private static byte NextCellState(byte cell, int neighbors)
    {
        if (cell == CellBlocked) return CellBlocked;
        if (cell == CellAlive) return (neighbors == 2 || neighbors == 3) ? CellAlive : CellDead;
        return neighbors == 3 ? CellAlive : CellDead;
    }


    private void ForSimulation()
    {
        var timer = Stopwatch.StartNew();

        Grid grid = _gridManager.Grid;
        int width = grid.Width;
        int height = grid.Height;

        var newGrid = new Grid(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                byte cell = grid.GetCellAt(x, y);
                if (cell == CellBlocked) continue;

                int aliveNeighbors = GetAliveNeighbors(grid, x, y);
                byte nextState = NextCellState(cell, aliveNeighbors);

                newGrid.SetCellAt(x, y, nextState);
            }
        }

        timer.Stop();
        StatsMenuController.Instance.UpdateSimulationCalcTime(timer.ElapsedMilliseconds);

        _gridManager.SetGrid(newGrid);
    }

    private int GetAliveNeighbors(Grid grid, int x, int y)
    {
        return _useForNeighborsFind
            ? grid.GetAliveNeighborsCountFor(x, y)
            : grid.GetAliveNeighborsCount(x, y);
    }

    public void StartSimulation()
    {
        if (_simulationRunning) return;
        TickManager.Instance.OnTick += OnTick;
        _simulationRunning = true;
    }

    public void StopSimulation()
    {
        if (!_simulationRunning) return;
        TickManager.Instance.OnTick -= OnTick;
        _simulationRunning = false;
    }
}
