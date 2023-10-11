using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MeshController : MonoBehaviour
{
    #region Properties
    public List<Color32> Colors { get; set; } = new();
    public Vector2 Increment { get; private set; } = new();

    public Vector2Int Resolution
    {
        get => _resolution;
        set
        {
            _resolution = value;
            Increment = Size / value;
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
            Increment = value / Resolution;
            UpdateVertices();
        }
    }
    private Vector2 _size = new(20, 20);
    #endregion

    #region Internal variables
    private List<int> _triangles = new();
    private List<Vector3> _vertices = new();
    private Mesh _mesh;
    #endregion

    #region Unity methods
    void Awake()
    {
        GetComponent<MeshFilter>().mesh = _mesh = new() { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };

        Resolution = _resolution;
        Size = _size;
    }

    void LateUpdate()
    {
        _mesh.colors32 = Colors.ToArray();
    }
    #endregion

    #region Methods
    private void UpdateTriangles()
    {
        _triangles.Clear();

        for (int i = 0; i < _vertices.Count; i += 4)
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

    private void UpdateVertices()
    {
        _vertices.Clear();

        var halfSize = Size / 2;

        for (int y = 0; y < Resolution.y; y++)
            for (int x = 0; x < Resolution.x; x++)
            {
                var xPos = x * Increment.x - halfSize.x;
                var yPos = y * Increment.y - halfSize.y;

                _vertices.Add(new Vector3(xPos, yPos));
                _vertices.Add(new Vector3(xPos + Increment.x, yPos));
                _vertices.Add(new Vector3(xPos, yPos + Increment.y));
                _vertices.Add(new Vector3(xPos + Increment.x, yPos + Increment.y));
            }

        _mesh.vertices = _vertices.ToArray();
    }

    public void SetPixel(int x, int y, Color32 color)
    {
        x = Mathf.Clamp(x, 0, Resolution.x);
        y = Mathf.Clamp(y, 0, Resolution.y);
        Colors[x + y * Resolution.x] = color;
    }
    #endregion
}
