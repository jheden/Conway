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
    protected List<int> _indices = new();
    protected List<int> _triangles = new();
    protected List<Vector3> _vertices = new();
    protected Mesh _mesh;
    protected float _nextUpdate;
    #endregion

    #region Abstract methods
    protected abstract void CopyGrid();
    protected abstract bool GetCurrent(int i);
    protected abstract void ResetGrid();
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
        if (Input.GetMouseButtonDown(0)) ClickShape();
        if (Input.GetMouseButton(1)) ClickDraw();
        if (Input.GetMouseButtonDown(2)) ClickFill();

        if (Time.time > _nextUpdate)
        {
            UpdateCells();
            _nextUpdate = Time.time + 1f / 16;
        }

        UpdateTriangles();
    }
    #endregion

    private void ClickDraw()
    {
        var position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) / Increments + Resolution / 2;
        SetCurrent((int)position.x + (int)position.y * Resolution.x, true);
    }

    private void ClickFill()
    {
        for (int i = 0; i < Length; i++)
            if (Random.Range(0, 100) < 20)
                SetCurrent(i, true);
    }

    private void ClickShape()
    {
        var position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) / Increments + Resolution / 2;
        AddShape((int)position.x, (int)position.y, Shapes.Instance.Pulsar);
    }

    public void AddShape(int x, int y, IShape shape)
    {
        foreach (int position in GetIndices(x, y, shape))
            SetCurrent(position, true);
    }

    protected IEnumerable<int> GetAlive()
    {
        return Enumerable.Range(0, Length).Where(i => GetCurrent(i));
    }

    protected List<int> GetIndices(int x, int y, IShape shape)
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
        CopyGrid();

        for (int i = 0; i < Length; i++)
        {
            int neighbours = 0;

            foreach (int neighbour in GetIndices(i % Resolution.x, i / Resolution.x, Shapes.Instance.Neighbors))
                if (Last[neighbour]) neighbours++;

            SetCurrent(i, (Last[i] ? 2 : 3) <= neighbours && neighbours <= 3);
        }
    }

    protected void UpdateTriangles()
    {
        _triangles.Clear();

        foreach (int i in GetAlive())
        {
            int j = i + i / Resolution.x;

            _triangles.Add(j);
            _triangles.Add(j + Resolution.x + 1);
            _triangles.Add(j + 1);

            _triangles.Add(j + Resolution.x + 2);
            _triangles.Add(j + 1);
            _triangles.Add(j + Resolution.x + 1);
        }

        _mesh.triangles = _triangles.ToArray();
    }

    protected void UpdateVertices()
    {
        _vertices.Clear();

        var halfX = Size.x / 2;
        var halfY = Size.y / 2;

        for (int y = 0; y <= Resolution.y; y++)
            for (int x = 0; x <= Resolution.x; x++)
                _vertices.Add(new Vector3(
                    x * Increments.x - halfX,
                    y * Increments.y - halfY
                ));

        _mesh.vertices = _vertices.ToArray();
    }
}