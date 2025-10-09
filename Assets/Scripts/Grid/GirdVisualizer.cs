using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

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

    Texture2D lifeTex;
    Material lifeMat;
    GameObject quad;

    RawImage raw;

    private void Start()
    {
        Stopwatch time = new Stopwatch();
        time.Start();

        CreateLifeTexture();
        CreateLifeMaterial();
        CreateQuad();

        UploadCells(_gridManager.Grid.CellPadded);

        time.Stop();
        StatsMenuController.Instance.UpdateGridCreateTime(time.ElapsedMilliseconds);

        _gridManager.OnGridUpdated += OnGridUpdated;
    }

    private void CreateLifeTexture()
    {
        lifeTex = new Texture2D(_gridManager.Grid.PWidth, _gridManager.Grid.PHeight, TextureFormat.R8, mipChain: false, linear: true);
        lifeTex.filterMode = FilterMode.Point;
        lifeTex.wrapMode = TextureWrapMode.Clamp;
    }

    private void CreateLifeMaterial()
    {
        var shader = Shader.Find("Unlit/LifePalette");

        UnityEngine.Debug.Assert(shader != null, "Shader 'Unlit/LifePalette' not found. Make sure it exists in the project.");

        lifeMat = new Material(shader);
        lifeMat.mainTexture = lifeTex;
    }

    private void CreateQuad()
    {
        quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = "LifeQuad";
        quad.GetComponent<MeshRenderer>().sharedMaterial = lifeMat;

        // Position and scale the quad to fit the grid
        float cellSize = 1f; // Assuming each cell is 1 unit in size
        quad.transform.localScale = new Vector3(_gridManager.Grid.PWidth * cellSize, _gridManager.Grid.PHeight * cellSize, 1f);
        quad.transform.position = Vector3.zero;
    }

    private void CreateUI()
    {
        var canvasGO = new GameObject("LifeCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var rawGo = new GameObject("LifeRawImage", typeof(RawImage));
        rawGo.transform.SetParent(canvasGO.transform, false);
        raw = rawGo.GetComponent<RawImage>();
        raw.texture = lifeTex;
        raw.material = lifeMat;
        raw.SetNativeSize();
    }

    public void UploadCells(byte[] _cells)
    {
        lifeTex.LoadRawTextureData(_cells);
        lifeTex.Apply(updateMipmaps: false, makeNoLongerReadable: false);
    }

    public void OnGridUpdated(Grid grid)
    {
        Stopwatch time = new Stopwatch();
        time.Start();

        UploadCells(grid.CellPadded);

        time.Stop();
        StatsMenuController.Instance.UpdateGridUpdateTime(time.ElapsedTicks);
    }



    //private void Awake()
    //{
    //    if (_gridManager == null)
    //    {
    //        UnityEngine.Debug.LogError("GridManager reference is missing in GridVisualizer.");
    //        return;
    //    }

    //    _gridManager.OnGridUpdated += UpdateGrid;
    //}

    //public void CreateGrid(Grid grid)
    //{
    //    Stopwatch time = new Stopwatch();
    //    time.Start();

    //    _cellVisualizers = new CellVisualizer[grid.Width, grid.Height];

    //    for (int x = 0; x < grid.Width; x++)
    //    {
    //        for (int y = 0; y < grid.Height; y++)
    //        {
    //            byte cell = grid.GetCellAt(x, y);
    //            if (cell == 2) continue;
    //            Vector3 cellPosition = grid.CalculateCellPosition(x, y);
    //            CellVisualizer cellVisualizer = Instantiate(_cellPrefab, cellPosition, Quaternion.identity, transform);
    //            if (cell == 1)
    //            {
    //                cellVisualizer.SetCellSprite(_aliveCellSprite);
    //            }
    //            else
    //            {
    //                cellVisualizer.SetCellSprite(_deadCellSprite);
    //            }

    //            _cellVisualizers[x, y] = cellVisualizer;
    //        }
    //    }

    //    time.Stop();
    //    StatsMenuController.Instance.UpdateGridCreateTime(time.ElapsedMilliseconds);
    //}

    //private void UpdateGrid(Grid grid)
    //{
    //    if (_cellVisualizers == null) { CreateGrid(_gridManager.Grid); return; }

    //    Stopwatch time = new Stopwatch();
    //    time.Start();

    //    for (int x = 0; x < grid.Width; x++)
    //    {
    //        for (int y = 0; y < grid.Height; y++)
    //        {
    //            byte cell = grid.GetCellAt(x, y);
    //            if (cell == 2) continue;
    //            CellVisualizer cellVisualizer = _cellVisualizers[x, y];
    //            if (cell == 1)
    //            {
    //                cellVisualizer.SetCellSprite(_aliveCellSprite);
    //            }
    //            else
    //            {
    //                cellVisualizer.SetCellSprite(_deadCellSprite);
    //            }
    //        }
    //    }

    //    time.Stop();
    //    StatsMenuController.Instance.UpdateGridUpdateTime(time.ElapsedMilliseconds);
    //}
}