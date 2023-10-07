using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Shapes;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public abstract class Grid : MonoBehaviour
{
    #region Properties
    protected Vector2 Increments { get; private set; }
    protected int Length { get; private set; }
    protected List<bool[]> States { get; } = new();
    protected abstract int[] Durations { get; }
    protected bool[] Last { 
        get => States.Last();
        set => States.Add((bool[])value.Clone());
    }

    public bool rewind;

    public Vector2Int Resolution
    {
        get => _resolution;
        set
        {
            _resolution = value;
            Increments = Size / value;
            Length = value.x * value.y;
            ResetGrid();
            UpdateVertices();
            UpdateTriangles();
        }
    }
    private Vector2Int _resolution = new(256, 256);

    public Vector2 Size
    {
        get => _size;
        set
        {
            _size = value;
            Increments = value / Resolution;
            UpdateVertices();
        }
    }
    private Vector2 _size = new(20, 20);
    #endregion

    #region Internal variables
    protected Color32[] __colors;
    protected List<Color32> _colors = new();
    protected List<int> _indices = new();
    protected List<int> _triangles = new();
    protected List<Vector3> _vertices = new();
    protected Mesh _mesh;
    protected float _nextUpdate;
    #endregion

    #region Abstract methods
    protected abstract Color GetColor(int i);
    protected abstract bool GetCurrent(int i);
    protected abstract void ResetGrid();
    protected abstract void LoadState();
    protected abstract void SaveState();
    protected abstract void SetCurrent(int i, bool state);
    #endregion

    #region Unity methods
    void Awake()
    {
        GetComponent<MeshFilter>().mesh = _mesh = new() { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };

        Resolution = _resolution;
        Size = _size;
    }

    private void Update()
    {
        rewind = Input.GetKey(KeyCode.Backspace);

        if (Input.GetMouseButtonDown(0)) ClickShape();
        if (Input.GetMouseButton(1)) ClickDraw();
        if (Input.GetMouseButtonDown(2)) ClickFill();

        if (Time.time > _nextUpdate)
        {
            if (rewind) try { LoadState(); } catch { }
            else UpdateCells();
            _nextUpdate = Time.time + 1f / 16;
        }

        UpdateColors();
    }
    #endregion

    public void ClickDraw()
    {
        var position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) / Increments + Resolution / 2;
        SetCurrent((int)position.x + (int)position.y * Resolution.x, true);
    }

    public void ClickFill()
    {
        for (int i = 0; i < Length; i++)
            if (Random.Range(0, 100) < 20)
                SetCurrent(i, true);
    }

    public void ClickShape()
    {
        var position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) / Increments + Resolution / 2;
        AddShape((int)position.x, (int)position.y, Shapes.Instance.Pulsar);
    }

    public void AddShape(int x, int y, Shape shape)
    {
        foreach (int position in GetIndices(x, y, shape))
            SetCurrent(position, true);
    }

    protected IEnumerable<int> GetAlive()
    {
        return Enumerable.Range(0, Length).Where(i => GetCurrent(i));
    }

    protected List<int> GetIndices(int x, int y, Shape shape)
    {
        _indices.Clear();

        foreach (var position in shape.Positions)
            _indices.Add(
                (x + position.x + Resolution.x) % Resolution.x +
                ((y + position.y + Resolution.y) % Resolution.y) * Resolution.x
            );

        return _indices;
    }

    protected void UpdateCells()
    {
        SaveState();

        for (int i = 0; i < Length; i++)
        {
            int neighbours = 0;

            foreach (int neighbour in GetIndices(i % Resolution.x, i / Resolution.x, Shapes.Instance.Neighbourhood))
                if (Last[neighbour]) neighbours++;

            SetCurrent(i, (Last[i] ? 2 : 3) <= neighbours && neighbours <= 3);
        }
    }

    protected void UpdateColors()
    {
        _colors.Clear();

        for (int i = 0; i < Length; i++)
        {
            var color = GetColor(i);

            for (int j = 0; j < 4; j++)
                _colors.Add(color);
        }

        _mesh.colors32 = _colors.ToArray();
    }

    protected void UpdateTriangles()
    {
        _triangles.Clear();

        for (int i = 0; i < Length * 4; i+=4)
        {
            _triangles.Add(i);
            _triangles.Add(i + 2);
            _triangles.Add(i + 1);

            _triangles.Add(i + 3);
            _triangles.Add(i + 1);
            _triangles.Add(i + 2);
        }

        _mesh.triangles = _triangles.ToArray();
    }

    protected void UpdateVertices()
    {
        _vertices.Clear();

        var halfSize = Size / 2;

        for (int y = 0; y < Resolution.y; y++)
            for (int x = 0; x < Resolution.x; x++)
            {
                var xPos = x * Increments.x - halfSize.x;
                var yPos = y * Increments.y - halfSize.y;

                _vertices.Add(new Vector3(xPos, yPos));
                _vertices.Add(new Vector3(xPos + Increments.x, yPos));
                _vertices.Add(new Vector3(xPos, yPos + Increments.y));
                _vertices.Add(new Vector3(xPos + Increments.x, yPos + Increments.y));
            }

        _mesh.vertices = _vertices.ToArray();
    }
}