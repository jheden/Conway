using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshController))]
[RequireComponent(typeof(MeshCollider))]
public abstract class ConwayGrid : MonoBehaviour
{
    bool[] _grid;

    #region Properties
    public bool Rewind { get; set; }
    public float ZoomInput { get; set; }

    public Vector2 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public Vector2Int Resolution {
        get => _mesh.Resolution;
        set
        {
            _mesh.Resolution = value;
            Length = value.x * value.y;
            ResetGrid();
            InitShader();
            _grid = new bool[Length];
        }
    }

    public float Size {
        get => _mesh.Size.x;
        set => _mesh.Size = new Vector2(value, value);
    }
    protected int Length { get; private set; }
    protected List<bool[]> States { get; } = new();
    protected abstract int[] Durations { get; }
    protected bool[] Last
    {
        get => States.Last();
        set => States.Add((bool[])value.Clone());
    }
    #endregion

    #region Public vars
    public Texture input;
    public ComputeShader shader;
    #endregion

    #region Internal vars
    private int _minScale = 1;
    private int _maxScale = 8;
    protected MeshController _mesh;
    protected float _nextUpdate;

    #region Shader vars
    private int _kernel;
    private bool _pingPong;
    private RenderTexture _ping;
    private RenderTexture _pong;
    protected Texture2D _texture;
    #endregion

    #endregion

    #region Abstract methods
    protected abstract Color32 GetColor(int i);
    protected abstract bool GetCurrent(int i);
    protected abstract void ResetGrid();
    protected abstract void LoadState();
    protected abstract void SaveState();
    protected abstract void SetCurrent(int i, bool state);
    #endregion

    #region Unity methods
    void Start()
    {
        _mesh = GetComponent<MeshController>();

        Size = 10;
        Resolution = new Vector2Int(512, 512);

        InitShader();
    }

    void Update()
    {
        if (ZoomInput != 0f)
            transform.localScale = Vector2.one * Mathf.Clamp(transform.localScale.x + ZoomInput * transform.localScale.x * Time.unscaledDeltaTime, _minScale, _maxScale);

        if (Time.time > _nextUpdate)
        {
            if (Rewind) try { LoadState(); } catch { }
            else UpdateCells();

            _nextUpdate = Time.time + 1f / 16;
        }

        UpdateColors();
    }
    #endregion

    public void Click(int i)
    {
        DrawShape(i, ShapeSelector.Instance.Shape);
    }

    #region Draw methods
    public void Clear()
    {
        _mesh.Clear();
    }

    #region void DrawShape
    public void DrawShape(int x, int y, Shape shape)
    {
        RenderTexture.active = _pingPong ? _pong : _ping;
        _texture.ReadPixels(new Rect(0, 0, _texture.width, _texture.height), 0, 0);
        foreach (var position in Shapes.Instance.Conway.Acorn.Positions)
        {
            _texture.SetPixel(x + position.x, y + position.y, Color.white);
            print($"setting {x + position.x}, {y + position.y}");
        }
        _texture.Apply();
        RenderTexture.active = null;

        DispatchShader();
        UpdateColors();
    }

    public void DrawShape(int i, Shape shape)
    {
        DrawShape(i % Resolution.x, i / Resolution.y, shape);
    }
    #endregion

    public void Fill(int percent)
    {
        percent = Mathf.Clamp(percent, 0, 100);
        for (int i = 0; i < Length; i++)
            SetCurrent(i, Random.Range(0, 100) < percent);
    }
    #endregion

    protected IEnumerable<int> GetAlive()
    {
        return Enumerable.Range(0, Length).Where(i => GetCurrent(i));
    }

    protected int[] GetIndices(int x, int y, Shape shape)
    {
        List<int> indices = new();

        foreach (var position in shape.Positions)
            indices.Add(
                (x + position.x + Resolution.x) % Resolution.x +
                ((y + position.y + Resolution.y) % Resolution.y) * Resolution.x
            );

        return indices.ToArray();
    }

    protected void UpdateCells()
    {
        DispatchShader();

        RenderTexture.active = _pingPong ? _pong : _ping;
        _texture.ReadPixels(new Rect(0, 0, _texture.width, _texture.height), 0, 0);
        RenderTexture.active = null;
        _grid = _texture.GetPixels32().Select(color => color.r > 0.5f).ToArray();
        _pingPong = !_pingPong;
    }

    protected void UpdateColors()
    {
        _mesh.Colors = _grid.Select(alive => alive ? (Color32)Color.white : (Color32)Color.black).ToArray();
    }

    #region Shader
    void InitShader()
    {
        _kernel = shader.FindKernel("Conway");

        _ping = new RenderTexture(Resolution.x, Resolution.y, 24);
        _pong = new RenderTexture(Resolution.x, Resolution.y, 24);
        _ping.wrapMode = _pong.wrapMode = TextureWrapMode.Repeat;
        _ping.enableRandomWrite = _pong.enableRandomWrite = true;
        _ping.filterMode = _pong.filterMode = FilterMode.Point;
        _ping.useMipMap = _pong.useMipMap = false;
        _ping.Create();
        _pong.Create();

        _texture = new Texture2D(Resolution.x, Resolution.y);

        Graphics.Blit(input, _pong);

        shader.SetFloat("Width", Resolution.x);
        shader.SetFloat("Height", Resolution.y);
        shader.SetVectorArray("neighborPos", new Vector4[8] {
            new(-1, -1),
            new(0, -1),
            new(1, -1),
            new(-1, 0),
            new(1, 0),
            new(-1, 1),
            new(0, 1),
            new(1, 1)
        });
    }

    void DispatchShader()
    {
        shader.SetTexture(_kernel, "In", _pingPong ? _ping : _pong);
        shader.SetTexture(_kernel, "Out", _pingPong ? _pong : _ping);
        shader.Dispatch(_kernel, Resolution.x / 8, Resolution.y / 8, 1);
    }
    #endregion
}