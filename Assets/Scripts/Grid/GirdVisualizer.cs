using System;
using System.Collections.Generic;
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
            Debug.LogError("GridManager reference is missing in GridVisualizer.");
            return;
        }

        _gridManager.OnGridUpdated += UpdateGrid;
    }

    public void CreateGrid(Grid grid)
    {
        _cellVisualizers = new CellVisualizer[grid.GridWidth, grid.GridHeight];

        for (int x = 0; x < grid.GridWidth; x++)
        {
            for (int y = 0; y < grid.GridHeight; y++)
            {
                Cell cell = grid.GetCellAt(x, y);
                if (cell == null) continue;
                Vector3 cellPosition = grid.CalculateCellPosition(x, y);
                CellVisualizer cellVisualizer = Instantiate(_cellPrefab, cellPosition, Quaternion.identity, transform);
                if (cell.isAlive)
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
    }

    private void UpdateGrid(Grid grid)
    {
        if (_cellVisualizers == null) { CreateGrid(_gridManager.Grid); return; }

        for (int x = 0; x < grid.GridWidth; x++)
        {
            for (int y = 0; y < grid.GridHeight; y++)
            {
                Cell cell = grid.GetCellAt(x, y);
                if (cell == null) continue;
                CellVisualizer cellVisualizer = _cellVisualizers[x, y];
                if (cell.isAlive)
                {
                    cellVisualizer.SetCellSprite(_aliveCellSprite);
                }
                else
                {
                    cellVisualizer.SetCellSprite(_deadCellSprite);
                }
            }
        }
    }
}