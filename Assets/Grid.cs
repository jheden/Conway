using System.Collections.Generic;
using UnityEngine;
using static Shapes;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public abstract class Grid : MonoBehaviour
{
    public Vector2 Increments { get; private set; }
    public int Length { get; private set; }

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
    protected Vector2Int _resolution = new(512, 512);

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

    protected List<int> _indices = new();
    protected List<int> _triangles = new();
    protected List<Vector3> _vertices = new();

    protected Mesh mesh;

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new() { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };

        Resolution = _resolution;
        Size = _size;

        SetShape(Resolution.x / 2, Resolution.y / 2, Shapes.Instance.Acorn);
    }

    private void FixedUpdate()
    {
        UpdateCells();
        UpdateTriangles();
    }

    protected abstract void SetShape(int x, int y, IShape shape);

    protected List<int> GetIndices(int x, int y, IShape shape)
    {
        _indices.Clear();

        foreach (var position in shape.Cock)
            _indices.Add(
                (x + position.x + Resolution.x) % Resolution.x +
                ((y + position.y + Resolution.y) % Resolution.y) * Resolution.x
            );

        return _indices;
    }

    protected abstract void ResetGrid();

    protected abstract void UpdateCells();

    protected abstract void UpdateTriangles();

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

        mesh.vertices = _vertices.ToArray();
    }
}