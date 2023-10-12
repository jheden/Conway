using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshController))]
[RequireComponent(typeof(MeshCollider))]
public class ShapeSelector : MonoBehaviour
{
    #region Singleton
    public static ShapeSelector Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    #endregion

    #region properties
    public Vector2 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public Vector2Int Resolution
    {
        get => _mesh.Resolution;
        set => _mesh.Resolution = value;
    }

    public Vector2 Size
    {
        get => _mesh.Size;
        set => _mesh.Size = value;
    }

    public Shape Shape {
        get => _shape;
        set
        {
            var center = Resolution / 2;
            if (Shape is not null)
                foreach (var position in Shape.Positions)
                    _mesh.SetPixel(position.x + center.x, position.y + center.y, Color.black);

            _shape = value;
            foreach (var position in value.Positions)
                _mesh.SetPixel(position.x + center.x, position.y + center.y, Color.white);
        }
    }
    #endregion

    private Shape _shape;
    private List<Shape> _conwayShapes = new();
    private MeshController _mesh;

    private void Start()
    {
        _mesh = GetComponent<MeshController>();

        foreach (KeyValuePair<string, object> item in Shapes.Instance.Conway)
            _conwayShapes.Add(item.Value as Shape);

        DrawUI();

        Shape = Shapes.Instance.Conway.Acorn;

        CameraController.Instance.resize += OnCameraResize;
    }

    public void OnCameraResize(Rect bounds)
    {
        float aspect = bounds.width / bounds.height;
        Size = new((bounds.width - bounds.height) / 2, bounds.height);
        Position = new(bounds.xMin + Size.x / 2, 0);
        Resolution = new(160, (int)(160 * aspect));
    }

    public void Click(int pixel)
    {
        Click(pixel % Resolution.x, pixel / Resolution.x);
    }

    public void Click(int x, int y)
    {
        if (x < 5 && y > Resolution.y - 5) Shape = Shape.RotateLeft;
        else if (x > Resolution.x - 5 && y > Resolution.y - 5) Shape = Shape.RotateRight;
        else if (y < Shapes.Instance.UI.FlipX.Height) Shape = Shape.FlipX;
        else if (x < Shapes.Instance.UI.FlipY.Width) Shape = Shape.FlipY;
    }

    private void DrawUI()
    {
        DrawFlipX();
        DrawFlipY();
    }

    private void DrawFlipX()
    {
        print("Drawing");
        _mesh.SetPixel(0, 0, Color.white);
        foreach (var position in Shapes.Instance.UI.FlipX.Positions)
            _mesh.SetPixel(position.x + Resolution.x / 2, position.y + Shapes.Instance.UI.FlipX.Height / 2, Color.white);
    }

    private void DrawFlipY()
    {
        foreach (var position in Shapes.Instance.UI.FlipY.Positions)
            _mesh.SetPixel(position.x + Shapes.Instance.UI.FlipY.Width / 2, position.y + Resolution.y / 2, Color.white);
    }
}
