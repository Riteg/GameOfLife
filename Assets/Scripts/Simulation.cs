using System;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;

    private void Awake()
    {
        if (_gridManager == null)
        {
            Debug.LogError("GridManager reference is missing in Simulation script.");
            return;
        }
    }

    private void OnEnable()
    {
        if (TickManager.Instance == null) return;

        TickManager.Instance.OnTick += OnTick;
    }

    private void OnDisable()
    {
        if (TickManager.Instance == null) return;

        TickManager.Instance.OnTick -= OnTick;
    }

    private void OnTick()
    {
        var newGrid = new Grid(_gridManager.Grid.GridWidth, _gridManager.Grid.GridHeight);

        for (int x = 0; x < _gridManager.Grid.GridWidth; x++)
        {
            for (int y = 0; y < _gridManager.Grid.GridHeight; y++)
            {
                var cell = _gridManager.Grid.GetCellAt(x, y);
                if (cell == null) continue;
                int aliveNeighbors = _gridManager.Grid.GetAliveNeighborsCount(x, y);
                // Apply Conway's Game of Life rules
                if (cell.isAlive)
                {
                    // Any live cell with two or three live neighbours survives.
                    if (aliveNeighbors == 2 || aliveNeighbors == 3)
                    {
                        newGrid.GetCellAt(x, y).isAlive = true;
                    }
                    else
                    {
                        // All other live cells die in the next generation. Similarly, all other dead cells stay dead.
                        newGrid.GetCellAt(x, y).isAlive = false;
                    }
                }
                else
                {
                    // Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
                    if (aliveNeighbors == 3)
                    {
                        newGrid.GetCellAt(x, y).isAlive = true;
                    }
                    else
                    {
                        newGrid.GetCellAt(x, y).isAlive = false;
                    }
                }
            }
        }

        _gridManager.SetGrid(newGrid);
    }
}