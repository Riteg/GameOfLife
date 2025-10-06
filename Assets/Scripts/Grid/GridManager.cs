using System;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private Grid _grid;

    [SerializeField] private int _width = 25;
    [SerializeField] private int _height = 25;

    public Grid Grid => _grid;

    public event Action<Grid> OnGridUpdated;

    private void Start()
    {
        _grid = new Grid(_width, _height);
        _grid.RandomlyPopulateGrid();
        OnGridUpdated?.Invoke(_grid);
    }

    public void SetGrid(Grid grid)
    {
        _grid = grid;
        OnGridUpdated?.Invoke(_grid);
    }
}