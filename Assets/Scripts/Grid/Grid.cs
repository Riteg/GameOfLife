using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int _width;
    private int _height;

    private byte[] _cells;

    public int Width => _width;
    public int Height => _height;

    public int PWidth => _width + 2;
    public int PHeight => _height + 2;

    public byte[] CellPadded => _cells;

    public Grid(int gridWidth, int gridHeight, bool populateGrid = false)
    {
        _width = gridWidth;
        _height = gridHeight;
        Initialize(PWidth, PHeight);
        if (populateGrid) RandomlyPopulateGrid();
    }

    private void Initialize(int gridWidth, int gridHeight)
    {
        _cells = new byte[gridWidth * gridHeight];
    }

    public void RandomlyPopulateGrid(float chance = 0.8f)
    {
        if (_cells == null) return;

        for (int y = 1; y <= Height; y++)
        {
            for (int x = 1; x <= Width; x++)
            {
                _cells[y * PWidth + x] = Random.value > chance ? (byte)1 : (byte)0;
            }
        }
    }

    public byte GetCellAt(int x, int y)
    {
        if (_cells == null) throw new System.Exception("Cells array is not initialized.");

        try
        {
            return _cells[(y + 1) * PWidth + (x + 1)];
        }
        catch (System.Exception)
        {

            throw new System.Exception($"[Grid] Index out of bounds (x:{x}, y:{y}, PWidth:{PWidth}): {y * PWidth + x} >= {_cells.Length}");
        }

    }

    public void SetCellAt(int x, int y, byte value)
    {
        if (_cells == null) throw new System.Exception("Cells array is not initialized.");

        _cells[(y + 1) * PWidth + (x + 1)] = value;
    }

    public void SetCellsPadded(byte[] cells)
    {
        if (cells.Length != _cells.Length) throw new System.Exception("Input cells array length does not match the grid size.");
        _cells = cells;
    }
}
