using System;
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

    public void RandomlyPopulateGrid(float chance = 0.2f)
    {
        int pWidth = PWidth;
        if (_cells == null) return;

        for (int y = 1; y <= Height; y++)
        {
            for (int x = 1; x <= Width; x++)
            {
                _cells[y * pWidth + x] = UnityEngine.Random.value < chance ? (byte)1 : (byte)0;
            }
        }
    }

    public void StartWitBorder(int size)
    {
        int pWidth = PWidth;
        if (_cells == null) return;

        // Ortadaki kareyi bulmak i�in ba�lang�� noktas�
        int startX = (Width / 2) - (size / 2);
        int startY = (Height / 2) - (size / 2);

        for (int y = 0; y < Height + 2; y++)
        {
            for (int x = 0; x < Width + 2; x++)
            {
                // Kare s�n�rlar� i�inde mi?
                if (x >= startX && x < startX + size &&
                    y >= startY && y < startY + size)
                {
                    _cells[y * pWidth + x] = 1;
                }
                else
                {
                    _cells[y * pWidth + x] = 0;
                }
            }
        }
    }

    public void StartWithSquare(int size)
    {
        if (_cells == null || size <= 0) return;

        int pWidth = PWidth;

        // 1) �� alan� (padding hari�) temizle
        for (int y = 1; y <= Height; y++)
        {
            for (int x = 1; x <= Width; x++)
            {
                _cells[y * pWidth + x] = (byte)0;
            }
        }

        // 2) Merkez ve kare s�n�rlar�n� hesapla
        int cx = (Width + 1) / 2;   // i� koordinatta merkez h�cre
        int cy = (Height + 1) / 2;

        int half = size / 2;
        int startX = cx - half;
        int startY = cy - half;
        int endX = startX + size - 1;
        int endY = startY + size - 1;

        // 3) �� alana (1..Width, 1..Height) clamp et
        if (startX < 1) startX = 1;
        if (startY < 1) startY = 1;
        if (endX > Width) endX = Width;
        if (endY > Height) endY = Height;

        // 4) Karemizi dolu �ekilde yaz
        for (int y = startY; y <= endY; y++)
        {
            for (int x = startX; x <= endX; x++)
            {
                _cells[y * pWidth + x] = (byte)1;
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

    #region StartPatterns

    #region Square
    /// <summary>
    /// Merkeze kare (dolu veya bo�luklu �er�eve) yerle�tirir.
    /// </summary>
    /// <param name="size">Karenin kenar uzunlu�u (i� �zgara koordinatlar�yla).</param>
    /// <param name="clearFirst">true ise �nce t�m i� alan� <paramref name="emptyValue"/> ile temizler.</param>
    /// <param name="fillValue">Dolu h�cre de�eri.</param>
    /// <param name="emptyValue">Bo� h�cre de�eri.</param>
    /// <param name="hollow">true ise sadece �er�eve �izer.</param>
    /// <param name="thickness">�er�eve/�izgi kal�nl��� (>=1).</param>
    /// <remarks>
    /// Merkez: ((Width + 1)/2, (Height + 1)/2)
    /// </remarks>
    /// <example>
    /// // 9x9'luk merkez kare (dolu)
    /// StartWithSquare(9);
    /// // 12'lik hollow kare, kal�nl�k 2
    /// StartWithSquare(12, true, 1, 0, true, 2);
    /// // Temizlemeden �st�ne yaz
    /// StartWithSquare(5, false);
    /// // Farkl� dolu/bo� de�erleri
    /// StartWithSquare(7, true, 2, 0);
    /// </example>
    public void StartWithSquare(int size, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0, bool hollow = false, int thickness = 1)
    {
        if (_cells == null) return;
        if (size <= 0) return;
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
            // �st ve alt kenarlar
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
            // Sol ve sa� kenarlar
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
    /// Merkezde yar��ap� verilen dolu disk veya bo�luklu �ember �izer (�klid mesafe).
    /// </summary>
    /// <param name="radius">Yar��ap (h�cre cinsinden, >=1).</param>
    /// <param name="clearFirst">true ise �nce t�m i� alan� temizler.</param>
    /// <param name="fillValue">Dolu de�er.</param>
    /// <param name="emptyValue">Bo� de�er.</param>
    /// <param name="hollow">true ise sadece halka (�ember kal�nl��� ile) �izer.</param>
    /// <param name="thickness">Halka kal�nl��� (>=1).</param>
    /// <example>
    /// // Dolu disk
    /// StartWithCircle(6);
    /// // Hollow �ember, kal�nl�k 2
    /// StartWithCircle(8, true, 1, 0, true, 2);
    /// // Temizlemeden �st�ne yaz
    /// StartWithCircle(5, false);
    /// // Farkl� de�erlerle
    /// StartWithCircle(7, true, 2, 0);
    /// </example>
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
    /// Merkezden ge�en art� (+) �ekli �izer (dikey + yatay �erit).
    /// </summary>
    /// <param name="thickness">Kol kal�nl��� (>=1).</param>
    /// <param name="clearFirst">true ise �nce temizler.</param>
    /// <param name="fillValue">Dolu de�er.</param>
    /// <param name="emptyValue">Bo� de�er.</param>
    /// <example>
    /// // �nce art�
    /// StartWithPlus();
    /// // Kal�n art�
    /// StartWithPlus(3);
    /// // �st�ne yaz
    /// StartWithPlus(2, false);
    /// // Farkl� de�erler
    /// StartWithPlus(2, true, 2, 0);
    /// </example>
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

        // Dikey kol
        for (int y = 1; y <= Height; y++)
        {
            int row = y * PWidth;
            for (int x = x0; x <= x1; x++) _cells[row + x] = fillValue;
        }
        // Yatay kol
        for (int y = y0; y <= y1; y++)
        {
            int row = y * PWidth;
            for (int x = 1; x <= Width; x++) _cells[row + x] = fillValue;
        }
    }

    #endregion

    #region Cross
    /// <summary>
    /// Merkezden ge�en iki diyagonal (X) �izer. Kal�nl�k destekli.
    /// </summary>
    /// <param name="thickness">�izgi kal�nl��� (>=1).</param>
    /// <param name="clearFirst">true ise �nce temizler.</param>
    /// <param name="fillValue">Dolu de�er.</param>
    /// <param name="emptyValue">Bo� de�er.</param>
    /// <example>
    /// // �nce X
    /// StartWithCross();
    /// // Kal�n X
    /// StartWithCross(3);
    /// // �st�ne yaz
    /// StartWithCross(2, false);
    /// // Farkl� de�erler
    /// StartWithCross(2, true, 2, 0);
    /// </example>
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
    /// �� alan kenarlar� boyunca �er�eve �izer (kal�nl�k destekli).
    /// </summary>
    /// <param name="thickness">�er�eve kal�nl��� (>=1).</param>
    /// <param name="clearFirst">true ise �nce temizler.</param>
    /// <param name="fillValue">Dolu de�er.</param>
    /// <param name="emptyValue">Bo� de�er.</param>
    /// <example>
    /// // �nce kenarl�k
    /// StartWithBorder();
    /// // Kal�n kenarl�k
    /// StartWithBorder(3);
    /// // �st�ne yaz
    /// StartWithBorder(2, false);
    /// // Farkl� de�erler
    /// StartWithBorder(2, true, 2, 0);
    /// </example>
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
    /// Satran� tahtas� (checkerboard) deseni �izer. H�cre boyutu ve ofset destekli.
    /// </summary>
    /// <param name="cellSize">Bir kare blo�un boyutu (>=1).</param>
    /// <param name="offsetX">X y�n�nde blok ofseti.</param>
    /// <param name="offsetY">Y y�n�nde blok ofseti.</param>
    /// <param name="clearFirst">true ise �nce temizler.</param>
    /// <param name="fillValue">Dolu de�er.</param>
    /// <param name="emptyValue">Bo� de�er.</param>
    /// <example>
    /// // 1x1 standart
    /// StartWithCheckerboard();
    /// // 2x2 bloklu checker
    /// StartWithCheckerboard(2);
    /// // Ofsetli
    /// StartWithCheckerboard(2, 1, 1);
    /// // �st�ne yaz ve farkl� de�erler
    /// StartWithCheckerboard(3, 0, 0, false, 2, 0);
    /// </example>
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
    /// Olas�l��a g�re rastgele doldurma (deterministik tohum opsiyonu ile).
    /// </summary>
    /// <param name="chance">[0..1] aras� doluluk olas�l���.</param>
    /// <param name="seed">Deterministik sonu� i�in tohum. null ise nondeterministik.</param>
    /// <param name="clearFirst">true ise �nce temizler.</param>
    /// <param name="fillValue">Dolu de�er.</param>
    /// <param name="emptyValue">Bo� de�er.</param>
    /// <example>
    /// // %20 doluluk
    /// StartWithRandomNoise(0.2f);
    /// // Deterministik
    /// StartWithRandomNoise(0.35f, 12345);
    /// // �st�ne yaz
    /// StartWithRandomNoise(0.5f, null, false);
    /// // Farkl� de�erler
    /// StartWithRandomNoise(0.6f, 42, true, 2, 0);
    /// </example>
    public void StartWithRandomNoise(float chance = 0.2f, int? seed = null, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0)
    {
        if (_cells == null) return;
        if (chance <= 0f && clearFirst)
        { // her �eyi bo�alt
            for (int y = 1; y <= Height; y++)
            {
                int row = y * PWidth;
                for (int x = 1; x <= Width; x++) _cells[row + x] = emptyValue;
            }
            return;
        }
        if (chance >= 1f && clearFirst)
        { // her �eyi doldur
            for (int y = 1; y <= Height; y++)
            {
                int row = y * PWidth;
                for (int x = 1; x <= Width; x++) _cells[row + x] = fillValue;
            }
            return;
        }

        System.Random rnd = seed.HasValue ? new System.Random(seed.Value) : null;

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
                double r = rnd != null ? rnd.NextDouble() : UnityEngine.Random.value;
                if (r < chance) _cells[row + x] = fillValue;
            }
        }
    }

    #endregion

    #region PerlinIslands
    /// <summary>
    /// Basit 2D value-noise ile ada/ma�ara benzeri desen olu�turur (threshold ile).
    /// </summary>
    /// <param name="scale">G�r�lt� �l�e�i (daha k���k = daha b�y�k adalar).</param>
    /// <param name="threshold">[0..1] e�ik; �st� dolu kabul edilir.</param>
    /// <param name="seed">Deterministik tohum (null ise nondeterministik).</param>
    /// <param name="clearFirst">true ise temizler.</param>
    /// <param name="fillValue">Dolu de�er.</param>
    /// <param name="emptyValue">Bo� de�er.</param>
    /// <example>
    /// // Orta yo�unluk
    /// StartWithPerlinIslands(0.1f, 0.5f);
    /// // Daha p�r�zl� ve deterministik
    /// StartWithPerlinIslands(0.2f, 0.45f, 1234);
    /// // �st�ne yazma
    /// StartWithPerlinIslands(0.08f, 0.55f, null, false);
    /// // Farkl� de�erler
    /// StartWithPerlinIslands(0.12f, 0.4f, 7, true, 2, 0);
    /// </example>
    public void StartWithPerlinIslands(float scale = 0.1f, float threshold = 0.5f, int? seed = null, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0)
    {
        if (_cells == null) return;
        if (scale <= 0f) scale = 0.0001f;

        int baseSeed = seed ?? UnityEngine.Random.Range(int.MinValue, int.MaxValue);

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
            // [0,1] aral���na getir
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

                // K��e de�erleri
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
    /// Rastgele dikd�rtgen odalar ve bunlar� ba�layan koridorlar �retir (basit MST ile ba�lar).
    /// </summary>
    /// <param name="roomCount">Oda say�s�.</param>
    /// <param name="minRoomSize">Min oda kenar� (>=2 �nerilir).</param>
    /// <param name="maxRoomSize">Max oda kenar� (>=minRoomSize).</param>
    /// <param name="corridorWidth">Koridor geni�li�i (>=1).</param>
    /// <param name="seed">Deterministik tohum.</param>
    /// <param name="clearFirst">true ise temizler.</param>
    /// <param name="fillValue">Dolu de�er.</param>
    /// <param name="emptyValue">Bo� de�er.</param>
    /// <example>
    /// // 4 oda
    /// StartWithRoomsAndCorridors();
    /// // Daha fazla ve geni� odalar
    /// StartWithRoomsAndCorridors(8, 4, 9, 2);
    /// // Deterministik
    /// StartWithRoomsAndCorridors(6, 3, 7, 1, 1234);
    /// // �st�ne yazma
    /// StartWithRoomsAndCorridors(5, 3, 6, 1, null, false);
    /// </example>
    public void StartWithRoomsAndCorridors(int roomCount = 4, int minRoomSize = 3, int maxRoomSize = 7, int corridorWidth = 1, int? seed = null, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0)
    {
        if (_cells == null) return;
        if (roomCount < 1) roomCount = 1;
        if (minRoomSize < 2) minRoomSize = 2;
        if (maxRoomSize < minRoomSize) maxRoomSize = minRoomSize;
        if (corridorWidth < 1) corridorWidth = 1;

        System.Random rnd = seed.HasValue ? new System.Random(seed.Value) : new System.Random();

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

        // Odalar� yerle�tir
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

        // Prim ile basit MST (merkezler aras�)
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

        // Koridorlar� �iz (L-�ekli: �nce X sonra Y)
        void DrawCorridor(int xA, int yA, int xB, int yB)
        {
            int x0 = Math.Max(1, Math.Min(xA, xB));
            int x1 = Math.Min(Width, Math.Max(xA, xB));
            int y0 = Math.Max(1, Math.Min(yA, yB));
            int y1 = Math.Min(Height, Math.Max(yA, yB));

            // X y�n�
            int yMid0 = Math.Max(1, yA - (corridorWidth - 1) / 2);
            int yMid1 = Math.Min(Height, yA + (corridorWidth - 1) / 2);
            for (int y = yMid0; y <= yMid1; y++)
            {
                int row = y * PWidth;
                for (int x = x0; x <= x1; x++) _cells[row + x] = fillValue;
            }

            // Y y�n�
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
    /// Sadece kare �er�eve �izer (StartWithSquare'�n k�sa yolu).
    /// </summary>
    /// <param name="size">Karenin kenar�.</param>
    /// <param name="thickness">�er�eve kal�nl��� (>=1).</param>
    /// <param name="clearFirst">true ise temizler.</param>
    /// <param name="fillValue">Dolu de�er.</param>
    /// <param name="emptyValue">Bo� de�er.</param>
    /// <example>
    /// // Hollow kare
    /// StartWithHollowSquare(9);
    /// // Kal�n �er�eve
    /// StartWithHollowSquare(12, 2);
    /// // �st�ne yaz
    /// StartWithHollowSquare(6, 1, false);
    /// // Farkl� de�erler
    /// StartWithHollowSquare(7, 3, true, 2, 0);
    /// </example>
    public void StartWithHollowSquare(int size, int thickness = 1, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0)
    {
        StartWithSquare(size, clearFirst, fillValue, emptyValue, true, thickness);
    }
    #endregion

    #region Diagonal
    /// <summary>
    /// Ana (sol-�stten sa�-alt) veya yan (sa�-�stten sol-alt) diyagonal �izer.
    /// </summary>
    /// <param name="mainDiagonal">true: ana diagonal, false: yan diagonal.</param>
    /// <param name="thickness">�izgi kal�nl��� (>=1).</param>
    /// <param name="clearFirst">true ise temizler.</param>
    /// <param name="fillValue">Dolu de�er.</param>
    /// <param name="emptyValue">Bo� de�er.</param>
    /// <example>
    /// // Ana diagonal
    /// StartWithDiagonal(true);
    /// // Yan diagonal kal�n
    /// StartWithDiagonal(false, 3);
    /// // �st�ne yaz
    /// StartWithDiagonal(true, 2, false);
    /// // Farkl� de�erler
    /// StartWithDiagonal(true, 2, true, 2, 0);
    /// </example>
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
    /// Yar�m alan� rastgele �retip simetri uygular. 'H': yukar�-a�a��, 'V': sol-sa�, 'Q': d�rtl� simetri.
    /// </summary>
    /// <param name="axis">'H', 'V' veya 'Q'.</param>
    /// <param name="seed">Deterministik tohum.</param>
    /// <param name="clearFirst">true ise temizler.</param>
    /// <param name="fillValue">Dolu de�er.</param>
    /// <param name="emptyValue">Bo� de�er.</param>
    /// <example>
    /// // Dikey simetri (sol �retilir, sa�a ayna)
    /// StartWithSymmetricHalf('V');
    /// // Yatay simetri (�st �retilir, alta ayna)
    /// StartWithSymmetricHalf('H', 1234);
    /// // D�rtl� simetri
    /// StartWithSymmetricHalf('Q', null, true, 1, 0);
    /// // �st�ne yaz
    /// StartWithSymmetricHalf('V', null, false);
    /// </example>
    public void StartWithSymmetricHalf(char axis = 'H', int? seed = null, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0)
    {
        if (_cells == null) return;
        System.Random rnd = seed.HasValue ? new System.Random(seed.Value) : new System.Random();

        if (clearFirst)
        {
            for (int y = 1; y <= Height; y++)
            {
                int row = y * PWidth;
                for (int x = 1; x <= Width; x++) _cells[row + x] = emptyValue;
            }
        }

        if (axis == 'V') // sol yar�m� �ret, sa�a kopyala
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
        else if (axis == 'H') // �st yar�m� �ret, alta kopyala
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
        else // 'Q' : sol-�st �ret, di�er �� �eyre�e yans�t
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

                    _cells[row + xr] = _cells[row + x];                 // sa�-�st
                    _cells[yr * PWidth + x] = _cells[row + x];          // sol-alt
                    _cells[yr * PWidth + xr] = _cells[row + x];         // sa�-alt
                }
            }
        }
    }

    #endregion

    #region CircleRingGrid
    /// <summary>
    /// Merkezde e�merkezli halka �zgaras� �izer.
    /// </summary>
    /// <param name="ringCount">Halka say�s�.</param>
    /// <param name="ringThickness">Her halkan�n kal�nl��� (>=1).</param>
    /// <param name="spacing">Halkalar aras� bo�luk (>=0).</param>
    /// <param name="clearFirst">true ise temizler.</param>
    /// <param name="fillValue">Dolu de�er.</param>
    /// <param name="emptyValue">Bo� de�er.</param>
    /// <example>
    /// // 3 halka
    /// StartWithCircleRingGrid();
    /// // Daha kal�n ve aral�kl�
    /// StartWithCircleRingGrid(4, 2, 2);
    /// // �st�ne yaz
    /// StartWithCircleRingGrid(5, 1, 1, false);
    /// // Farkl� de�erler
    /// StartWithCircleRingGrid(2, 3, 1, true, 2, 0);
    /// </example>
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
                // Halkalar�n d�� yar��aplar�: r_k = (k+1)*ringThickness + k*spacing
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
    /// Merkezden ��kan e�it a��l� ���nlar (spoke) �izer. Bresenham benzeri �izim, kal�nl�k destekli.
    /// </summary>
    /// <param name="spokeCount">Kol say�s� (>=1).</param>
    /// <param name="thickness">�izgi kal�nl��� (>=1).</param>
    /// <param name="clearFirst">true ise temizler.</param>
    /// <param name="fillValue">Dolu de�er.</param>
    /// <param name="emptyValue">Bo� de�er.</param>
    /// <example>
    /// // 8 kollu
    /// StartWithRadialSpokes();
    /// // 12 kollu, kal�nl�k 2
    /// StartWithRadialSpokes(12, 2);
    /// // �st�ne yaz
    /// StartWithRadialSpokes(6, 1, false);
    /// // Farkl� de�erler
    /// StartWithRadialSpokes(10, 3, true, 2, 0);
    /// </example>
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
        // Bresenham ile merkezden s�n�r noktas�na
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
    /// Dikey veya yatay �eritler �izer (�izgi-bo�luk deseni).
    /// </summary>
    /// <param name="vertical">true: dikey, false: yatay.</param>
    /// <param name="stripeWidth">�erit kal�nl��� (>=1).</param>
    /// <param name="gap">�eritler aras� bo�luk (>=0).</param>
    /// <param name="offset">Ba�lang�� ofseti (piksel).</param>
    /// <param name="clearFirst">true ise temizler.</param>
    /// <param name="fillValue">Dolu de�er.</param>
    /// <param name="emptyValue">Bo� de�er.</param>
    /// <example>
    /// // Dikey �eritler
    /// StartWithStripe();
    /// // Yatay ve kal�n
    /// StartWithStripe(false, 3, 2);
    /// // Ofsetli ve �st�ne yaz
    /// StartWithStripe(true, 2, 3, 1, false);
    /// // Farkl� de�erler
    /// StartWithStripe(true, 1, 1, 0, true, 2, 0);
    /// </example>
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
    /// Kare �er�eve (hollow) veya dolu kare: StartWithSquare ile ayn� ama ad� netlik i�in.
    /// </summary>
    /// <param name="size">Kenarl�k boyutu.</param>
    /// <param name="clearFirst">true ise temizler.</param>
    /// <param name="fillValue">Dolu de�er.</param>
    /// <param name="emptyValue">Bo� de�er.</param>
    /// <param name="hollow">true ise �er�eve, false ise dolu kare.</param>
    /// <param name="thickness">�er�eve kal�nl���.</param>
    /// <example>
    /// // Dolu kare
    /// StartWithSquare(5);
    /// // Hollow kare
    /// StartWithSquare(10, true, 1, 0, true, 2);
    /// // �st�ne yaz
    /// StartWithSquare(7, false);
    /// // Farkl� de�erler
    /// StartWithSquare(9, true, 2, 0, true, 3);
    /// </example>
    public void StartWithSquare_Alt(int size, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0, bool hollow = false, int thickness = 1)
    {
        StartWithSquare(size, clearFirst, fillValue, emptyValue, hollow, thickness);
    }

    #endregion

    #region Circle_Alt
    /// <summary>
    /// Merkeze g�re �ember/�ember halkas�: StartWithCircle i�in alternatif ad.
    /// </summary>
    /// <param name="radius">Yar��ap.</param>
    /// <param name="clearFirst">true ise temizler.</param>
    /// <param name="fillValue">Dolu de�er.</param>
    /// <param name="emptyValue">Bo� de�er.</param>
    /// <param name="hollow">true ise halka.</param>
    /// <param name="thickness">Halka kal�nl���.</param>
    /// <example>
    /// StartWithCircle(6);
    /// StartWithCircle(8, true, 1, 0, true, 2);
    /// StartWithCircle(5, false);
    /// StartWithCircle(7, true, 2, 0);
    /// </example>
    public void StartWithCircle_Alt(int radius, bool clearFirst = true, byte fillValue = 1, byte emptyValue = 0, bool hollow = false, int thickness = 1)
    {
        StartWithCircle(radius, clearFirst, fillValue, emptyValue, hollow, thickness);
    }

    #endregion

    #endregion
}
