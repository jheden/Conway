using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Shapes;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public abstract class Grid : MonoBehaviour
{
    #region Properties
    public Vector2 Increments { get; private set; }
    public int Length { get; private set; }
    protected List<bool[]> States { get; } = new();
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
    protected Vector2Int _resolution = new(256, 256);

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
    protected Vector2 _size = new(20, 20);
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

        SetShape(Resolution.x / 2, Resolution.y / 2, Shapes.Instance.Pulsar);
    }

    private void Update()
    {
        if (Time.time < _nextUpdate) return;

        UpdateCells();
        UpdateTriangles();
        _nextUpdate = Time.time + 1f / 16;
    }
    #endregion

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

    public void SetShape(int x, int y, IShape shape)
    {
        foreach (int position in GetIndices(x, y, shape))
            SetCurrent(position, true);
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