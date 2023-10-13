using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MeshController : MonoBehaviour
{
    #region Properties
    public Color32[] Colors { get; set; }

    public Vector2 Increment { get; private set; } = new();

    public Vector2Int Resolution
    {
        get => _resolution;
        set
        {
            _resolution = value;
            Increment = Size / value;
            Colors = new Color32[Resolution.x * Resolution.y];
            UpdateMesh();
        }
    }
    private Vector2Int _resolution = Vector2Int.one;

    public Vector2 Size
    {
        get => _size;
        set
        {
            _size = value;
            Increment = value / Resolution;
            UpdateMesh();
        }
    }
    private Vector2 _size = Vector2.one;
    #endregion

    #region Internal variables
    private List<Color32> _colors = new();
    private List<int> _triangles = new();
    //private List<Vector2> _uvs = new();
    private List<Vector3> _vertices = new();
    private Mesh _mesh;
    private MeshCollider _collider;
    #endregion

    #region Unity methods
    void Awake()
    {
        GetComponent<MeshFilter>().mesh = _mesh = new() { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
        TryGetComponent<MeshCollider>(out _collider);
    }

    void LateUpdate()
    {
        UpdateColors();
    }
    #endregion

    #region Public methods
    public void Clear(Color32 color)
    {
        Colors = Enumerable.Repeat(color, Resolution.x * Resolution.y).ToArray();
    }

    public void Clear()
    {
        Clear(Color.black);
    }

    public void SetPixel(int x, int y, Color32 color)
    {
        Colors[x + y * Resolution.x] = color;
    }

    public void SetPixel(int i, Color32 color)
    {
        Colors[i] = color;
    }
    #endregion

    #region Private methods
    private void UpdateMesh()
    {
        _mesh.Clear();
        UpdateVertices();
        UpdateTriangles();
        UpdateCollider();
    }

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
        //_uvs.Clear();

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


                //_uvs.Add(new Vector2((float)x / Resolution.x, (float)y / Resolution.y));
                //_uvs.Add(new Vector2((float)(x + 1) / Resolution.x, (float)y / Resolution.y));
                //_uvs.Add(new Vector2((float)x / Resolution.x, (float)(y + 1) / Resolution.y));
                //_uvs.Add(new Vector2((float)(x + 1) / Resolution.x, (float)(y + 1) / Resolution.y));
            }

        _mesh.vertices = _vertices.ToArray();
        //_mesh.uv = _uvs.ToArray();
    }

    private void UpdateCollider()
    {
        if (_collider is null) return;
        _collider.sharedMesh = _mesh;
    }

    private void UpdateColors()
    {
        _colors.Clear();

        for (int i = 0; i < Colors.Length; i++)
            for (int j = 0; j < 4; j++)
                _colors.Add(Colors[i]);

        try
        {
            _mesh.colors32 = _colors.ToArray();
        }
        catch
        {
            UpdateMesh();
            _mesh.colors32 = _colors.ToArray();
        }
    }
    #endregion
}
