using System;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private Grid _grid;

    [SerializeField] private int _width = 25;
    [SerializeField] private int _height = 25;

    [SerializeField] private float _populateChance = 0.2f;

    public Grid Grid => _grid;

    public event Action<Grid> OnGridUpdated;

    private void Start()
    {
        _grid = new Grid(_width, _height);
        _grid.StartWithSquare(_width / 2);
        OnGridUpdated?.Invoke(_grid);
    }

    public void SetGrid(Grid grid)
    {
        _grid = grid;
        OnGridUpdated?.Invoke(_grid);
    }

    public void SetCells(byte[] cells)
    {
        _grid.SetCellsPadded(cells);
        OnGridUpdated?.Invoke(_grid);
    }
}