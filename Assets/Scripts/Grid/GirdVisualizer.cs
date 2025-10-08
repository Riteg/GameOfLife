using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[DisallowMultipleComponent]
public class GridVisualizer : MonoBehaviour
{
    [Header("Manager")]
    [SerializeField] private GridManager _gridManager;

    [Header("Prefab")]
    [SerializeField] private CellVisualizer _cellPrefab;

    [Header("Sprites")]
    [SerializeField] private Sprite _aliveCellSprite;
    [SerializeField] private Sprite _deadCellSprite;

    private CellVisualizer[,] _cellVisualizers;

    private void Awake()
    {
        if (_gridManager == null)
        {
            UnityEngine.Debug.LogError("GridManager reference is missing in GridVisualizer.");
            return;
        }

        _gridManager.OnGridUpdated += UpdateGrid;
    }

    public void CreateGrid(Grid grid)
    {
        Stopwatch time = new Stopwatch();
        time.Start();

        _cellVisualizers = new CellVisualizer[grid.Width, grid.Height];

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                byte cell = grid.GetCellAt(x, y);
                if (cell == 2) continue;
                Vector3 cellPosition = grid.CalculateCellPosition(x, y);
                CellVisualizer cellVisualizer = Instantiate(_cellPrefab, cellPosition, Quaternion.identity, transform);
                if (cell == 1)
                {
                    cellVisualizer.SetCellSprite(_aliveCellSprite);
                }
                else
                {
                    cellVisualizer.SetCellSprite(_deadCellSprite);
                }

                _cellVisualizers[x, y] = cellVisualizer;
            }
        }

        time.Stop();
        StatsMenuController.Instance.UpdateGridCreateTime(time.ElapsedMilliseconds);
    }

    private void UpdateGrid(Grid grid)
    {
        if (_cellVisualizers == null) { CreateGrid(_gridManager.Grid); return; }

        Stopwatch time = new Stopwatch();
        time.Start();

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                byte cell = grid.GetCellAt(x, y);
                if (cell == 2) continue;
                CellVisualizer cellVisualizer = _cellVisualizers[x, y];
                if (cell == 1)
                {
                    cellVisualizer.SetCellSprite(_aliveCellSprite);
                }
                else
                {
                    cellVisualizer.SetCellSprite(_deadCellSprite);
                }
            }
        }

        time.Stop();
        StatsMenuController.Instance.UpdateGridUpdateTime(time.ElapsedMilliseconds);
    }
}