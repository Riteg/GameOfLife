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

    public void Init()
    {
        _grid = new Grid(_width, _height);
        ChangeStartPattern(Grid.StartPattern.Square);
        OnGridUpdated?.Invoke(_grid);
    }

    public void SetGrid(Grid grid)
    {
        _grid = grid;
        OnGridUpdated?.Invoke(_grid);
    }

    public void SetCells(byte[] cells)
    {
        if (_grid == null)
        {
            Debug.LogError("Grid is can not be null.");
            return;
        }
        //if (cells.Length != _grid.Width * _grid.Height)
        //{
        //    Debug.LogError("Cells array length does not match grid dimensions.");
        //    return;
        //}

        _grid.SetCellsPadded(cells);
        OnGridUpdated?.Invoke(_grid);
    }

    public void ChangeStartPattern(Grid.StartPattern startPattern)
    {
        switch (startPattern)
        {
            case Grid.StartPattern.Square:
                _grid.StartWithSquare(_width / 4);
                break;
            case Grid.StartPattern.Circle:
                _grid.StartWithCircle(_width / 7);
                break;
            case Grid.StartPattern.Plus:
                _grid.StartWithPlus(20);
                break;
            case Grid.StartPattern.Cross:
                _grid.StartWithCross(20);
                break;
            case Grid.StartPattern.Border:
                _grid.StartWithBorder(20);
                break;
            case Grid.StartPattern.Checkerboard:
                _grid.StartWithCheckerboard(5);
                break;
            case Grid.StartPattern.RandomNoise:
                _grid.StartWithRandomNoise(_populateChance);
                break;
            case Grid.StartPattern.PerlinIslands:
                _grid.StartWithPerlinIslands();
                break;
            case Grid.StartPattern.RoomsAndCorridors:
                _grid.StartWithRoomsAndCorridors(roomCount: 20, minRoomSize: 25, maxRoomSize: 50, corridorWidth: 5);
                break;
            case Grid.StartPattern.HallowSquare:
                _grid.StartWithHollowSquare(20, 5);
                break;
            case Grid.StartPattern.Diagonal:
                _grid.StartWithDiagonal(thickness: 5);
                break;
            case Grid.StartPattern.SymmetricHalf:
                _grid.StartWithSymmetricHalf();
                break;
            case Grid.StartPattern.CircleRingGrid:
                _grid.StartWithCircleRingGrid(ringCount: 10, ringThickness: 5, spacing: 20);
                break;
            case Grid.StartPattern.RadialSpokes:
                _grid.StartWithRadialSpokes(spokeCount: 12, thickness: 3);
                break;
            case Grid.StartPattern.Stripe:
                _grid.StartWithStripe();
                break;
            case Grid.StartPattern.Square_Alt:
                _grid.StartWithSquare_Alt(_width / 2);
                break;
            case Grid.StartPattern.Circle_Alt:
                _grid.StartWithCircle_Alt(_width / 3);
                break;
        }
        OnGridUpdated?.Invoke(_grid);
    }
}