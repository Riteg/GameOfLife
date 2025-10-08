using UnityEngine;

public static class GridHelper
{
    /// <summary>
    /// Calculates the world position of a cell in the grid based on its grid coordinates (x, y).
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static Vector3 CalculateCellPosition(this Grid grid, int x, int y)
    {
        // Calculate the world position of the cell based on its grid coordinates

        // Calculate the offset to center the grid at the origin
        Vector3 offset = new Vector3(grid.Width / 2f, grid.Height / 2f, 0);
        return new Vector3(x, y, 0) - offset;
    }

    public static int GetAliveNeighborsCount(this Grid grid, int x, int y)
    {
        //int aliveNeighbors = 0;
        //// Check all 8 neighboring cells
        //for (int dx = -1; dx <= 1; dx++)
        //{
        //    for (int dy = -1; dy <= 1; dy++)
        //    {
        //        // Skip the cell itself
        //        if (dx == 0 && dy == 0) continue;
        //        int neighborX = x + dx;
        //        int neighborY = y + dy;
        //        // Check if the neighbor is within grid bounds
        //        if (neighborX >= 0 && neighborX < grid.Width &&
        //            neighborY >= 0 && neighborY < grid.Height)
        //        {
        //            if (grid.GetCellAt(neighborX, neighborY) == 1)
        //            {
        //                aliveNeighbors++;
        //            }
        //        }
        //    }
        //}
        //return aliveNeighbors;


        int aliveNeighbors = 0;

        // Check all 8 neighboring cells
        aliveNeighbors += grid.GetCellAt(x - 1, y - 1); // Top-left
        aliveNeighbors += grid.GetCellAt(x, y - 1);     // Top
        aliveNeighbors += grid.GetCellAt(x + 1, y - 1); // Top-right
        aliveNeighbors += grid.GetCellAt(x - 1, y);     // Left
        aliveNeighbors += grid.GetCellAt(x + 1, y);     // Right
        aliveNeighbors += grid.GetCellAt(x - 1, y + 1); // Bottom-left
        aliveNeighbors += grid.GetCellAt(x, y + 1);     // Bottom
        aliveNeighbors += grid.GetCellAt(x + 1, y + 1); // Bottom-right

        return aliveNeighbors;
    }
}