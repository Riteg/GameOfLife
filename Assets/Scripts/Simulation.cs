using System;
using System.Diagnostics;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;

    private void Awake()
    {
        if (_gridManager == null)
        {
            UnityEngine.Debug.LogError("GridManager reference is missing in Simulation script.");
            return;
        }
    }

    private void OnEnable()
    {
        if (TickManager.Instance == null) return;

        TickManager.Instance.OnTick += OnTickNew;
    }

    private void OnDisable()
    {
        if (TickManager.Instance == null) return;

        TickManager.Instance.OnTick -= OnTickNew;
    }

    private void OnTick()
    {
        Stopwatch time = new Stopwatch();
        time.Start();

        var newGrid = new Grid(_gridManager.Grid.Width, _gridManager.Grid.Height);

        for (int x = 0; x < _gridManager.Grid.Width; x++)
        {
            for (int y = 0; y < _gridManager.Grid.Height; y++)
            {
                byte cell = _gridManager.Grid.GetCellAt(x, y);
                if (cell == 2) continue;
                int aliveNeighbors = _gridManager.Grid.GetAliveNeighborsCount(x, y);
                // Apply Conway's Game of Life rules
                if (cell == 1)
                {
                    // Any live cell with two or three live neighbours survives.
                    if (aliveNeighbors == 2 || aliveNeighbors == 3)
                    {
                        newGrid.SetCellAt(x, y, 1);
                    }
                    else
                    {
                        // All other live cells die in the next generation. Similarly, all other dead cells stay dead.
                        newGrid.SetCellAt(x, y, 0);
                    }
                }
                else
                {
                    // Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
                    if (aliveNeighbors == 3)
                    {
                        newGrid.SetCellAt(x, y, 1);
                    }
                    else
                    {
                        newGrid.SetCellAt(x, y, 0);
                    }
                }
            }
        }

        _gridManager.SetGrid(newGrid);

        time.Stop();
        StatsMenuController.Instance.UpdateSimulationCalcTime(time.ElapsedMilliseconds);
    }

    private void OnTickNew()
    {
        Stopwatch time = new Stopwatch();
        time.Start();

        int W = _gridManager.Grid.Width;
        int H = _gridManager.Grid.Height;

        int PW = W + 2;
        int PH = H + 2;

        byte[] current = _gridManager.Grid.CellPadded;
        byte[] next = new byte[PW * PH];

        byte[] Hsum = new byte[PW * PH];


        // Horizontal sum
        for (int y = 1; y <= H; y++)
        {
            int row = y * PW;

            int i = row + 1;
            int sum = current[i - 1] + current[i] + current[i + 1];
            Hsum[i] = (byte)sum;

            for (int x = 2; x <= W; x++)
            {
                i++;
                sum += current[i + 1]
                    - current[i - 2];
                Hsum[i] = (byte)sum;
            }
        }

        // Vertical sum and apply rules
        for (int x = 1; x <= W; x++)
        {
            int iTop = (0 * PW) + x;
            int iMid = (1 * PW) + x;
            int iBottom = (2 * PW) + x;
            int sum = Hsum[iTop] + Hsum[iMid] + Hsum[iBottom];

            for (int y = 1; y <= H; y++)
            {
                int i = y * PW + x;

                int neighbors = sum - current[i];

                byte cell = current[i];
                byte outVal;

                if (cell == 2)
                {
                    outVal = 2;
                }
                else if (cell == 1)
                {
                    outVal = (neighbors == 2 || neighbors == 3) ? (byte)1 : (byte)0;
                }
                else
                {
                    outVal = (neighbors == 3) ? (byte)1 : (byte)0;
                }

                next[i] = outVal;

                if (y < H)
                {
                    int iOldTop = (y - 1) * PW + x;
                    int iNewBottom = (y + 2) * PW + x;
                    sum += Hsum[iNewBottom] - Hsum[iOldTop];
                }
            }
        }
        time.Stop();
        StatsMenuController.Instance.UpdateSimulationCalcTime(time.ElapsedMilliseconds);

        _gridManager.SetCells(next);

    }
}