using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int _gridWidth;
    private int _gridHeight;

    private Cell[,] _cells;

    public int GridWidth => _gridWidth;
    public int GridHeight => _gridHeight;

    public Grid(int gridWidth, int gridHeight, bool populateGrid = false)
    {
        _gridWidth = gridWidth;
        _gridHeight = gridHeight;
        Initialize(_gridWidth, _gridHeight);
        if (populateGrid) RandomlyPopulateGrid();
    }

    private void Initialize(int gridWidth, int gridHeight)
    {
        _cells = new Cell[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                _cells[x, y] = new Cell(x, y, false);
            }
        }
    }

    public void RandomlyPopulateGrid(float chance = 0.8f)
    {
        if (_cells == null) return;

        foreach (var cell in _cells)
        {
            cell.isAlive = Random.value > chance;
        }
    }

    public void RandomlyPopulateGrid(int maxAliveCount)
    {
        if (_cells == null) return;

        var aliveCells = new HashSet<Cell>();

        while (aliveCells.Count < maxAliveCount)
        {
            int x = Random.Range(0, _gridWidth);
            int y = Random.Range(0, _gridHeight);
            var cell = _cells[x, y];
            if (!cell.isAlive)
            {
                cell.isAlive = true;
                aliveCells.Add(cell);
            }
        }
    }

    public Cell[,] GetCells()
    {
        return _cells;
    }

    public Cell GetCellAt(int x, int y)
    {
        if (_cells == null) return null;

        return _cells[x, y];
    }
}
