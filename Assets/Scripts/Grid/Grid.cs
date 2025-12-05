using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int _width;
    private int _height;

    public const byte Alive = 1;
    public const byte Dead = 0;

    private byte[] _cells;

    public int Width => _width;
    public int Height => _height;

    public int PWidth => _width + 2;
    public int PHeight => _height + 2;

    public byte[] CellPadded => _cells;

    private int CenterX => (Width + 1) / 2;
    private int CenterY => (Height + 1) / 2;

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

    private void EnsureCellsAllocated()
    {
        if (_cells == null) throw new InvalidOperationException("Cells array is not initialized.");
    }

    private void ValidateInteriorCoordinate(int x, int y)
    {
        if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException(nameof(x), $"X coordinate {x} is outside interior width {Width}.");
        if (y < 0 || y >= Height) throw new ArgumentOutOfRangeException(nameof(y), $"Y coordinate {y} is outside interior height {Height}.");
    }

    private int ToPaddedIndex(int x, int y) => (y + 1) * PWidth + (x + 1);

    private void ClearInterior(byte value)
    {
        EnsureCellsAllocated();
        int pWidth = PWidth;
        for (int y = 1; y <= Height; y++)
        {
            int row = y * pWidth;
            for (int x = 1; x <= Width; x++) _cells[row + x] = value;
        }
    }

    private void ClearIfRequested(bool clearFirst, byte emptyValue)
    {
        if (clearFirst) ClearInterior(emptyValue);
    }

    private void ForEachInteriorCell(Action<int, int, int> action)
    {
        int pWidth = PWidth;
        for (int y = 1; y <= Height; y++)
        {
            int row = y * pWidth;
            for (int x = 1; x <= Width; x++) action(x, y, row + x);
        }
    }

    private static double NextRandom01(System.Random rng) => rng?.NextDouble() ?? UnityEngine.Random.value;

    public void RandomlyPopulateGrid(float chance = 0.2f, System.Random rng = null, byte aliveValue = Alive, byte deadValue = Dead)
    {
        EnsureCellsAllocated();

        ForEachInteriorCell((x, y, idx) =>
        {
            _cells[idx] = NextRandom01(rng) < chance ? aliveValue : deadValue;
        });
    }

    public byte GetCellAt(int x, int y)
    {
        EnsureCellsAllocated();
        ValidateInteriorCoordinate(x, y);
        return _cells[ToPaddedIndex(x, y)];
    }

    public void SetCellAt(int x, int y, byte value)
    {
        EnsureCellsAllocated();
        ValidateInteriorCoordinate(x, y);
        _cells[ToPaddedIndex(x, y)] = value;
    }

    public void SetCellsPadded(byte[] cells)
    {
        EnsureCellsAllocated();
        if (cells == null) throw new ArgumentNullException(nameof(cells));
        if (cells.Length != _cells.Length) throw new ArgumentException("Input cells array length does not match the grid size.", nameof(cells));
        _cells = (byte[])cells.Clone();
    }

    public enum StartPattern
    {
        Square = 0,
        Circle = 1,
        Plus = 2,
        Cross = 3,
        Border = 4,
        Checkerboard = 5,
        RandomNoise = 6,
        PerlinIslands = 7,
        RoomsAndCorridors = 8,
        HallowSquare = 9,
        Diagonal = 10,
        SymmetricHalf = 11,
        CircleRingGrid = 12,
        RadialSpokes = 13,
        Stripe = 14,
        Square_Alt = 15,
        Circle_Alt = 16,
    }

    #region StartPatterns

    #region Square
    /// <summary>
    /// Draws a centered square; supports hollow outlines.
    /// </summary>
    public void StartWithSquare(int size, bool clearFirst = true, byte fillValue = Alive, byte emptyValue = Dead, bool hollow = false, int thickness = 1)
    {
        EnsureCellsAllocated();
        if (size <= 0) return;
        if (thickness < 1) thickness = 1;

        int cx = CenterX;
        int cy = CenterY;

        ClearIfRequested(clearFirst, emptyValue);

        int half = size / 2;
        int x0 = Math.Max(1, cx - half);
        int x1 = Math.Min(Width, cx + (size - 1 - half));
        int y0 = Math.Max(1, cy - half);
        int y1 = Math.Min(Height, cy + (size - 1 - half));

        if (!hollow)
        {
            for (int y = y0; y <= y1; y++)
            {
                int row = y * PWidth;
                for (int x = x0; x <= x1; x++) _cells[row + x] = fillValue;
            }
        }
        else
        {
            // Top and bottom edges
            for (int t = 0; t < thickness; t++)
            {
                int yt = Math.Min(y0 + t, y1);
                int yb = Math.Max(y1 - t, y0);
                int rowT = yt * PWidth;
                int rowB = yb * PWidth;
                for (int x = x0; x <= x1; x++)
                {
                    _cells[rowT + x] = fillValue;
                    _cells[rowB + x] = fillValue;
                }
            }
            // Left and right edges
            for (int t = 0; t < thickness; t++)
            {
                int xl = Math.Min(x0 + t, x1);
                int xr = Math.Max(x1 - t, x0);
                for (int y = y0; y <= y1; y++)
                {
                    int row = y * PWidth;
                    _cells[row + xl] = fillValue;
                    _cells[row + xr] = fillValue;
                }
            }
        }
    }
    #endregion

    #region Circle
    /// <summary>
    /// Draws a filled circle or hollow ring at the center.
    /// </summary>
    /// <param name="radius">Radius in cells (>=1).</param>
    /// <param name="clearFirst">Clear the interior before drawing.</param>
    /// <param name="fillValue">Value for filled cells.</param>
    /// <param name="emptyValue">Value used when clearing.</param>
    /// <param name="hollow">When true, draws only the ring.</param>
    /// <param name="thickness">Ring thickness in cells.</param>
    public void StartWithCircle(int radius, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0, bool hollow = false, int thickness = 1)
    {
        if (_cells == null) return;
        if (radius < 1) return;
        if (thickness < 1) thickness = 1;

        int cx = (Width + 1) / 2;
        int cy = (Height + 1) / 2;

        if (clearFirst)
        {
            for (int y = 1; y <= Height; y++)
            {
                int row = y * PWidth;
                for (int x = 1; x <= Width; x++) _cells[row + x] = emptyValue;
            }
        }

        int r2 = radius * radius;
        int innerR = Math.Max(0, radius - thickness);
        int inner2 = innerR * innerR;

        for (int y = 1; y <= Height; y++)
        {
            int row = y * PWidth;
            int dy = y - cy;
            for (int x = 1; x <= Width; x++)
            {
                int dx = x - cx;
                int d2 = dx * dx + dy * dy;
                if (!hollow)
                {
                    if (d2 <= r2) _cells[row + x] = fillValue;
                }
                else
                {
                    if (d2 <= r2 && d2 > inner2) _cells[row + x] = fillValue;
                }
            }
        }
    }

    #endregion

    #region plus
    /// <summary>
    /// Draws a centered plus sign using vertical and horizontal bars.
    /// </summary>
    /// <param name="thickness">Bar thickness in cells.</param>
    /// <param name="clearFirst">Clear the interior before drawing.</param>
    /// <param name="fillValue">Value for filled cells.</param>
    /// <param name="emptyValue">Value used when clearing.</param>
    public void StartWithPlus(int thickness = 1, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0)
    {
        if (_cells == null) return;
        if (thickness < 1) thickness = 1;

        int cx = (Width + 1) / 2;
        int cy = (Height + 1) / 2;

        if (clearFirst)
        {
            for (int y = 1; y <= Height; y++)
            {
                int row = y * PWidth;
                for (int x = 1; x <= Width; x++) _cells[row + x] = emptyValue;
            }
        }

        int half = (thickness - 1) / 2;
        int x0 = Math.Max(1, cx - half);
        int x1 = Math.Min(Width, cx + (thickness - 1 - half));
        int y0 = Math.Max(1, cy - half);
        int y1 = Math.Min(Height, cy + (thickness - 1 - half));

        // Vertical bar
        for (int y = 1; y <= Height; y++)
        {
            int row = y * PWidth;
            for (int x = x0; x <= x1; x++) _cells[row + x] = fillValue;
        }
        // Horizontal bar
        for (int y = y0; y <= y1; y++)
        {
            int row = y * PWidth;
            for (int x = 1; x <= Width; x++) _cells[row + x] = fillValue;
        }
    }

    #endregion

    #region Cross
    /// <summary>
    /// Draws a centered X using diagonals.
    /// </summary>
    /// <param name="thickness">Line thickness in cells.</param>
    /// <param name="clearFirst">Clear the interior before drawing.</param>
    /// <param name="fillValue">Value for filled cells.</param>
    /// <param name="emptyValue">Value used when clearing.</param>
    public void StartWithCross(int thickness = 1, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0)
    {
        if (_cells == null) return;
        if (thickness < 1) thickness = 1;

        int cx = (Width + 1) / 2;
        int cy = (Height + 1) / 2;

        if (clearFirst)
        {
            for (int y = 1; y <= Height; y++)
            {
                int row = y * PWidth;
                for (int x = 1; x <= Width; x++) _cells[row + x] = emptyValue;
            }
        }

        for (int y = 1; y <= Height; y++)
        {
            int row = y * PWidth;
            int dy = y - cy;
            for (int x = 1; x <= Width; x++)
            {
                int dx = x - cx;
                if (Math.Abs(dx - dy) < thickness || Math.Abs(dx + dy) < thickness)
                    _cells[row + x] = fillValue;
            }
        }
    }

    #endregion

    #region Border
    /// <summary>
    /// Draws a border along interior edges.
    /// </summary>
    /// <param name="thickness">Border thickness in cells.</param>
    /// <param name="clearFirst">Clear the interior before drawing.</param>
    /// <param name="fillValue">Value for filled cells.</param>
    /// <param name="emptyValue">Value used when clearing.</param>
    public void StartWithBorder(int thickness = 1, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0)
    {
        if (_cells == null) return;
        if (thickness < 1) thickness = 1;

        if (clearFirst)
        {
            for (int y = 1; y <= Height; y++)
            {
                int row = y * PWidth;
                for (int x = 1; x <= Width; x++) _cells[row + x] = emptyValue;
            }
        }

        for (int y = 1; y <= Height; y++)
        {
            int row = y * PWidth;
            for (int x = 1; x <= Width; x++)
            {
                bool onBorder =
                    x <= Math.Min(thickness, Width) ||
                    x > Width - thickness ||
                    y <= Math.Min(thickness, Height) ||
                    y > Height - thickness;
                if (onBorder) _cells[row + x] = fillValue;
            }
        }
    }
    #endregion

    #region Checkerboard
    /// <summary>
    /// Draws a checkerboard pattern with optional block size/offset.
    /// </summary>
    /// <param name="cellSize">Block size for each square.</param>
    /// <param name="offsetX">Horizontal block offset.</param>
    /// <param name="offsetY">Vertical block offset.</param>
    /// <param name="clearFirst">Clear the interior before drawing.</param>
    /// <param name="fillValue">Value for filled cells.</param>
    /// <param name="emptyValue">Value used when clearing.</param>
    public void StartWithCheckerboard(int cellSize = 1, int offsetX = 0, int offsetY = 0, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0)
    {
        if (_cells == null) return;
        if (cellSize < 1) cellSize = 1;

        if (clearFirst)
        {
            for (int y = 1; y <= Height; y++)
            {
                int row = y * PWidth;
                for (int x = 1; x <= Width; x++) _cells[row + x] = emptyValue;
            }
        }

        for (int y = 1; y <= Height; y++)
        {
            int row = y * PWidth;
            int by = ((y - 1 + offsetY) / cellSize);
            for (int x = 1; x <= Width; x++)
            {
                int bx = ((x - 1 + offsetX) / cellSize);
                bool fill = ((bx + by) & 1) == 0;
                _cells[row + x] = fill ? fillValue : emptyValue;
            }
        }
    }
    #endregion

    #region RandomNoise
    /// <summary>
    /// Randomly fills cells using a probability.
    /// </summary>
    public void StartWithRandomNoise(float chance = 0.2f, int? seed = null, bool clearFirst = true, byte fillValue = Alive, byte emptyValue = Dead, System.Random rng = null)
    {
        EnsureCellsAllocated();
        if (chance <= 0f && clearFirst)
        {
            ClearInterior(emptyValue);
            return;
        }
        if (chance >= 1f && clearFirst)
        {
            ClearInterior(fillValue);
            return;
        }

        System.Random rnd = seed.HasValue ? new System.Random(seed.Value) : rng;

        ClearIfRequested(clearFirst, emptyValue);

        ForEachInteriorCell((x, y, idx) =>
        {
            if (NextRandom01(rnd) < chance) _cells[idx] = fillValue;
        });
    }

    #endregion

    #region PerlinIslands
    /// <summary>
    /// Generates island- or cave-like noise using value noise.
    /// </summary>
    /// <param name="scale">Noise scale (smaller values produce larger islands).</param>
    /// <param name="threshold">Fill threshold in [0..1].</param>
    /// <param name="seed">Optional deterministic seed.</param>
    /// <param name="clearFirst">Clear the interior before drawing.</param>
    /// <param name="fillValue">Value for filled cells.</param>
    /// <param name="emptyValue">Value used when clearing.</param>
    public void StartWithPerlinIslands(float scale = 0.1f, float threshold = 0.5f, int? seed = null, bool clearFirst = true, byte fillValue = Alive, byte emptyValue = Dead, System.Random rng = null)
    {
        if (_cells == null) return;
        if (scale <= 0f) scale = 0.0001f;

        int baseSeed = seed ?? (rng != null ? rng.Next(int.MinValue, int.MaxValue) : UnityEngine.Random.Range(int.MinValue, int.MaxValue));

        if (clearFirst)
        {
            for (int y = 1; y <= Height; y++)
            {
                int row = y * PWidth;
                for (int x = 1; x <= Width; x++) _cells[row + x] = emptyValue;
            }
        }

        // Hash fonksiyonu: deterministik pseudorandom [0,1)
        int Hash(int x, int y)
        {
            unchecked
            {
                int h = baseSeed;
                h ^= x * 374761393;
                h = (h << 13) ^ h;
                h = h * 1274126177;
                h ^= y * 668265263;
                h = (h << 15) ^ h;
                return h;
            }
        }
        float Val(int xi, int yi)
        {
            // Convert to [0,1] range
            uint u = (uint)Hash(xi, yi);
            return (u / (float)uint.MaxValue);
        }
        float Smooth(float t) => t * t * (3f - 2f * t); // fade

        for (int y = 1; y <= Height; y++)
        {
            int row = y * PWidth;
            float fy = y * scale;
            int y0 = (int)Math.Floor(fy);
            float ty = fy - y0;
            float sy = Smooth(ty);

            for (int x = 1; x <= Width; x++)
            {
                float fx = x * scale;
                int x0 = (int)Math.Floor(fx);
                float tx = fx - x0;
                float sx = Smooth(tx);

                // Corner values
                float v00 = Val(x0, y0);
                float v10 = Val(x0 + 1, y0);
                float v01 = Val(x0, y0 + 1);
                float v11 = Val(x0 + 1, y0 + 1);

                // Bilinear + smoothstep
                float vx0 = v00 + (v10 - v00) * sx;
                float vx1 = v01 + (v11 - v01) * sx;
                float v = vx0 + (vx1 - vx0) * sy;

                if (v >= threshold) _cells[row + x] = fillValue;
            }
        }
    }

    #endregion

    #region RoomsAndCorridors
    /// <summary>
    /// Places random rooms and connects them with corridors.
    /// </summary>
    /// <param name="roomCount">How many rooms to place.</param>
    /// <param name="minRoomSize">Minimum room side length.</param>
    /// <param name="maxRoomSize">Maximum room side length.</param>
    /// <param name="corridorWidth">Corridor thickness in cells.</param>
    /// <param name="seed">Optional deterministic seed.</param>
    /// <param name="clearFirst">Clear the interior before drawing.</param>
    /// <param name="fillValue">Value for filled cells.</param>
    /// <param name="emptyValue">Value used when clearing.</param>
    public void StartWithRoomsAndCorridors(int roomCount = 4, int minRoomSize = 3, int maxRoomSize = 7, int corridorWidth = 1, int? seed = null, bool clearFirst = true, byte fillValue = Alive, byte emptyValue = Dead, System.Random rng = null)
    {
        if (_cells == null) return;
        if (roomCount < 1) roomCount = 1;
        if (minRoomSize < 2) minRoomSize = 2;
        if (maxRoomSize < minRoomSize) maxRoomSize = minRoomSize;
        if (corridorWidth < 1) corridorWidth = 1;

        System.Random rnd = seed.HasValue ? new System.Random(seed.Value) : (rng ?? new System.Random());

        if (clearFirst)
        {
            for (int y = 1; y <= Height; y++)
            {
                int row = y * PWidth;
                for (int x = 1; x <= Width; x++) _cells[row + x] = emptyValue;
            }
        }

        var centers = new System.Collections.Generic.List<(int x, int y)>();
        var rooms = new System.Collections.Generic.List<(int x0, int y0, int x1, int y1)>();

        // Odalarý yerleþtir
        for (int i = 0; i < roomCount; i++)
        {
            int w = rnd.Next(minRoomSize, maxRoomSize + 1);
            int h = rnd.Next(minRoomSize, maxRoomSize + 1);

            int x0 = rnd.Next(1, Math.Max(2, Width - w + 2));
            int y0 = rnd.Next(1, Math.Max(2, Height - h + 2));
            int x1 = Math.Min(Width, x0 + w - 1);
            int y1 = Math.Min(Height, y0 + h - 1);

            for (int y = y0; y <= y1; y++)
            {
                int row = y * PWidth;
                for (int x = x0; x <= x1; x++) _cells[row + x] = fillValue;
            }

            int cx = (x0 + x1) / 2;
            int cy = (y0 + y1) / 2;
            centers.Add((cx, cy));
            rooms.Add((x0, y0, x1, y1));
        }

        if (centers.Count <= 1) return;

        // Prim ile basit MST (merkezler arasý)
        int n = centers.Count;
        var inTree = new bool[n];
        var dist = new int[n];
        var parent = new int[n];
        for (int i = 0; i < n; i++) { dist[i] = int.MaxValue; parent[i] = -1; }
        dist[0] = 0;

        for (int k = 0; k < n; k++)
        {
            int u = -1, best = int.MaxValue;
            for (int i = 0; i < n; i++)
                if (!inTree[i] && dist[i] < best) { best = dist[i]; u = i; }
            if (u == -1) break;
            inTree[u] = true;

            for (int v = 0; v < n; v++)
            {
                if (inTree[v] || v == u) continue;
                int dx = centers[u].x - centers[v].x;
                int dy = centers[u].y - centers[v].y;
                int d = dx * dx + dy * dy;
                if (d < dist[v]) { dist[v] = d; parent[v] = u; }
            }
        }

        // Draw corridors in an L shape: horizontal then vertical
        void DrawCorridor(int xA, int yA, int xB, int yB)
        {
            int x0 = Math.Max(1, Math.Min(xA, xB));
            int x1 = Math.Min(Width, Math.Max(xA, xB));
            int y0 = Math.Max(1, Math.Min(yA, yB));
            int y1 = Math.Min(Height, Math.Max(yA, yB));

            // Horizontal section
            int yMid0 = Math.Max(1, yA - (corridorWidth - 1) / 2);
            int yMid1 = Math.Min(Height, yA + (corridorWidth - 1) / 2);
            for (int y = yMid0; y <= yMid1; y++)
            {
                int row = y * PWidth;
                for (int x = x0; x <= x1; x++) _cells[row + x] = fillValue;
            }

            // Vertical section
            int xMid0 = Math.Max(1, xB - (corridorWidth - 1) / 2);
            int xMid1 = Math.Min(Width, xB + (corridorWidth - 1) / 2);
            int yStart = Math.Min(yA, yB);
            int yEnd = Math.Max(yA, yB);
            for (int y = yStart; y <= yEnd; y++)
            {
                int row = y * PWidth;
                for (int x = xMid0; x <= xMid1; x++) _cells[row + x] = fillValue;
            }
        }

        for (int v = 0; v < n; v++)
        {
            int p = parent[v];
            if (p >= 0)
            {
                var a = centers[v];
                var b = centers[p];
                DrawCorridor(a.x, a.y, b.x, b.y);
            }
        }
    }

    #endregion

    #region HollowSquare
    /// <summary>
    /// Draws only the outline of a square.
    /// </summary>
    /// <param name="size">Square side length.</param>
    /// <param name="thickness">Border thickness in cells.</param>
    /// <param name="clearFirst">Clear the interior before drawing.</param>
    /// <param name="fillValue">Value for filled cells.</param>
    /// <param name="emptyValue">Value used when clearing.</param>
    public void StartWithHollowSquare(int size, int thickness = 1, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0)
    {
        StartWithSquare(size, clearFirst, fillValue, emptyValue, true, thickness);
    }
    #endregion

    #region Diagonal
    /// <summary>
    /// Draws either the main or counter diagonal.
    /// </summary>
    /// <param name="mainDiagonal">True for main diagonal, false for counter diagonal.</param>
    /// <param name="thickness">Line thickness in cells.</param>
    /// <param name="clearFirst">Clear the interior before drawing.</param>
    /// <param name="fillValue">Value for filled cells.</param>
    /// <param name="emptyValue">Value used when clearing.</param>
    public void StartWithDiagonal(bool mainDiagonal = true, int thickness = 1, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0)
    {
        if (_cells == null) return;
        if (thickness < 1) thickness = 1;

        if (clearFirst)
        {
            for (int y = 1; y <= Height; y++)
            {
                int row = y * PWidth;
                for (int x = 1; x <= Width; x++) _cells[row + x] = emptyValue;
            }
        }

        for (int y = 1; y <= Height; y++)
        {
            int row = y * PWidth;
            for (int x = 1; x <= Width; x++)
            {
                if (mainDiagonal)
                {
                    if (Math.Abs((y - 1) - (x - 1)) < thickness) _cells[row + x] = fillValue;
                }
                else
                {
                    if (Math.Abs((x - 1) + (y - 1) - (Width - 1)) < thickness) _cells[row + x] = fillValue;
                }
            }
        }
    }

    #endregion

    #region SymmetricHalf
    /// <summary>
    /// Generates part of the grid and mirrors it across an axis.
    /// </summary>
    /// <param name="axis">'H', 'V', or 'Q' to pick the mirror axis.</param>
    /// <param name="seed">Optional deterministic seed.</param>
    /// <param name="clearFirst">Clear the interior before drawing.</param>
    /// <param name="fillValue">Value for filled cells.</param>
    /// <param name="emptyValue">Value used when clearing.</param>
    public void StartWithSymmetricHalf(char axis = 'H', int? seed = null, bool clearFirst = true, byte fillValue = Alive, byte emptyValue = Dead, System.Random rng = null)
    {
        if (_cells == null) return;
        System.Random rnd = seed.HasValue ? new System.Random(seed.Value) : (rng ?? new System.Random());

        if (clearFirst)
        {
            for (int y = 1; y <= Height; y++)
            {
                int row = y * PWidth;
                for (int x = 1; x <= Width; x++) _cells[row + x] = emptyValue;
            }
        }

        if (axis == 'V') // build left half and mirror to the right
        {
            int mid = (Width + 1) / 2;
            for (int y = 1; y <= Height; y++)
            {
                int row = y * PWidth;
                for (int x = 1; x <= mid; x++)
                {
                    bool on = rnd.NextDouble() < 0.5;
                    if (on) _cells[row + x] = fillValue;
                    int xr = Width - x + 1;
                    _cells[row + xr] = _cells[row + x];
                }
            }
        }
        else if (axis == 'H') // build top half and mirror downward
        {
            int mid = (Height + 1) / 2;
            for (int y = 1; y <= mid; y++)
            {
                int row = y * PWidth;
                for (int x = 1; x <= Width; x++)
                {
                    bool on = rnd.NextDouble() < 0.5;
                    if (on) _cells[row + x] = fillValue;
                    int yr = Height - y + 1;
                    _cells[yr * PWidth + x] = _cells[row + x];
                }
            }
        }
        else // 'Q' : build top-left quarter and mirror to all quadrants
        {
            int midX = (Width + 1) / 2;
            int midY = (Height + 1) / 2;
            for (int y = 1; y <= midY; y++)
            {
                int row = y * PWidth;
                for (int x = 1; x <= midX; x++)
                {
                    bool on = rnd.NextDouble() < 0.5;
                    if (on) _cells[row + x] = fillValue;

                    int xr = Width - x + 1;
                    int yr = Height - y + 1;

                    _cells[row + xr] = _cells[row + x];                 // top-right
                    _cells[yr * PWidth + x] = _cells[row + x];          // bottom-left
                    _cells[yr * PWidth + xr] = _cells[row + x];         // bottom-right
                }
            }
        }
    }

    #endregion

    #region CircleRingGrid
    /// <summary>
    /// Draws concentric rings around the center.
    /// </summary>
    /// <param name="ringCount">Number of rings to draw.</param>
    /// <param name="ringThickness">Thickness of each ring in cells.</param>
    /// <param name="spacing">Spacing between rings.</param>
    /// <param name="clearFirst">Clear the interior before drawing.</param>
    /// <param name="fillValue">Value for filled cells.</param>
    /// <param name="emptyValue">Value used when clearing.</param>
    public void StartWithCircleRingGrid(int ringCount = 3, int ringThickness = 1, int spacing = 1, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0)
    {
        if (_cells == null) return;
        if (ringCount < 1) return;
        if (ringThickness < 1) ringThickness = 1;
        if (spacing < 0) spacing = 0;

        int cx = (Width + 1) / 2;
        int cy = (Height + 1) / 2;

        if (clearFirst)
        {
            for (int y = 1; y <= Height; y++)
            {
                int row = y * PWidth;
                for (int x = 1; x <= Width; x++) _cells[row + x] = emptyValue;
            }
        }

        int maxR = Math.Min(Width, Height) / 2;
        for (int y = 1; y <= Height; y++)
        {
            int row = y * PWidth;
            int dy = y - cy;
            for (int x = 1; x <= Width; x++)
            {
                int dx = x - cx;
                int d2 = dx * dx + dy * dy;
                // Outer radius of ring k: (k+1)*ringThickness + k*spacing
                for (int k = 0; k < ringCount; k++)
                {
                    int outer = (k + 1) * ringThickness + k * spacing;
                    int inner = Math.Max(0, outer - ringThickness);
                    if (outer > maxR) break;
                    int outer2 = outer * outer;
                    int inner2 = inner * inner;
                    if (d2 <= outer2 && d2 > inner2)
                    {
                        _cells[row + x] = fillValue;
                        break;
                    }
                }
            }
        }
    }

    #endregion

    #region RadialSpokes
    /// <summary>
    /// Draws equally spaced spokes from the center.
    /// </summary>
    /// <param name="spokeCount">Number of spokes.</param>
    /// <param name="thickness">Line thickness in cells.</param>
    /// <param name="clearFirst">Clear the interior before drawing.</param>
    /// <param name="fillValue">Value for filled cells.</param>
    /// <param name="emptyValue">Value used when clearing.</param>
    public void StartWithRadialSpokes(int spokeCount = 8, int thickness = 1, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0)
    {
        if (_cells == null) return;
        if (spokeCount < 1) spokeCount = 1;
        if (thickness < 1) thickness = 1;

        int cx = (Width + 1) / 2;
        int cy = (Height + 1) / 2;

        if (clearFirst)
        {
            for (int y = 1; y <= Height; y++)
            {
                int row = y * PWidth;
                for (int x = 1; x <= Width; x++) _cells[row + x] = emptyValue;
            }
        }

        int R = Math.Max(Width, Height);
        // Bresenham ile merkezden sýnýr noktasýna
        void Plot(int x, int y)
        {
            if (x < 1 || x > Width || y < 1 || y > Height) return;
            _cells[y * PWidth + x] = fillValue;
        }
        void PlotThick(int x, int y)
        {
            int r = thickness / 2;
            for (int oy = -r; oy <= r; oy++)
            {
                int yy = y + oy;
                if (yy < 1 || yy > Height) continue;
                int row = yy * PWidth;
                for (int ox = -r; ox <= r; ox++)
                {
                    int xx = x + ox;
                    if (xx < 1 || xx > Width) continue;
                    _cells[row + xx] = fillValue;
                }
            }
        }
        void Line(int x0, int y0, int x1, int y1)
        {
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = dx + dy;
            while (true)
            {
                if (thickness > 1) PlotThick(x0, y0); else Plot(x0, y0);
                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * err;
                if (e2 >= dy) { err += dy; x0 += sx; }
                if (e2 <= dx) { err += dx; y0 += sy; }
            }
        }

        for (int k = 0; k < spokeCount; k++)
        {
            double ang = (Math.PI * 2.0 * k) / spokeCount;
            int ex = cx + (int)Math.Round(Math.Cos(ang) * R);
            int ey = cy + (int)Math.Round(Math.Sin(ang) * R);
            Line(cx, cy, ex, ey);
        }
    }

    #endregion

    #region Stripe
    /// <summary>
    /// Draws repeating vertical or horizontal stripes.
    /// </summary>
    /// <param name="vertical">True for vertical stripes; false for horizontal.</param>
    /// <param name="stripeWidth">Stripe thickness in cells.</param>
    /// <param name="gap">Gap between stripes.</param>
    /// <param name="offset">Pixel offset to start the pattern.</param>
    /// <param name="clearFirst">Clear the interior before drawing.</param>
    /// <param name="fillValue">Value for filled cells.</param>
    /// <param name="emptyValue">Value used when clearing.</param>
    public void StartWithStripe(bool vertical = true, int stripeWidth = 2, int gap = 2, int offset = 0, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0)
    {
        if (_cells == null) return;
        if (stripeWidth < 1) stripeWidth = 1;
        if (gap < 0) gap = 0;

        int period = stripeWidth + gap;
        if (period < 1) period = 1;

        if (clearFirst)
        {
            for (int y = 1; y <= Height; y++)
            {
                int row = y * PWidth;
                for (int x = 1; x <= Width; x++) _cells[row + x] = emptyValue;
            }
        }

        if (vertical)
        {
            for (int x = 1; x <= Width; x++)
            {
                bool on = ((x - 1 + offset) % period + period) % period < stripeWidth;
                if (!on) continue;
                for (int y = 1; y <= Height; y++) _cells[y * PWidth + x] = fillValue;
            }
        }
        else
        {
            for (int y = 1; y <= Height; y++)
            {
                bool on = ((y - 1 + offset) % period + period) % period < stripeWidth;
                if (!on) continue;
                int row = y * PWidth;
                for (int x = 1; x <= Width; x++) _cells[row + x] = fillValue;
            }
        }
    }

    #endregion

    #region Square_Alt
    /// <summary>
    /// Alias for StartWithSquare for explicit intent.
    /// </summary>
    /// <param name="size">Kenarlýk boyutu.</param>
    /// <param name="clearFirst">true ise temizler.</param>
    /// <param name="fillValue">Dolu deðer.</param>
    /// <param name="emptyValue">Boþ deðer.</param>
    /// <param name="hollow">Draw outline only when true.</param>
    /// <param name="thickness">Outline thickness in cells.</param>
    public void StartWithSquare_Alt(int size, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0, bool hollow = false, int thickness = 1)
    {
        StartWithSquare(size, clearFirst, fillValue, emptyValue, hollow, thickness);
    }

    #endregion

    #region Circle_Alt
    /// <summary>
    /// Alias for StartWithCircle for explicit intent.
    /// </summary>
    /// <param name="radius">Circle radius.</param>
    /// <param name="clearFirst">Clear the interior before drawing.</param>
    /// <param name="fillValue">Value for filled cells.</param>
    /// <param name="emptyValue">Value used when clearing.</param>
    /// <param name="hollow">When true, draw only the ring.</param>
    /// <param name="thickness">Ring thickness in cells.</param>
    public void StartWithCircle_Alt(int radius, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0, bool hollow = false, int thickness = 1)
    {
        StartWithCircle(radius, clearFirst, fillValue, emptyValue, hollow, thickness);
    }

    #endregion

    #endregion
}
