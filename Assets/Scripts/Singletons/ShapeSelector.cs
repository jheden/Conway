using System.Collections.Generic;
using UnityEngine;

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

    #region Properties
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
            if (Shape is not null)
                DrawShape(80, 80, Shape, Color.black);

            DrawShape(80, 80, value);

            _shape = value;
        }
    }
    #endregion

    #region Internal variables
    private MeshController _mesh;
    private Shape _shape;
    private List<Shape> _conwayShapes = new();
    private int _shapeIndex;
    #endregion

    private enum Clickable {
        FpsDown,
        FpsUp,
        PrevShape,
        NextShape,
        FlipX,
        FlipY,
        RotLeft,
        RotRight
    };
    private Dictionary<Clickable, RectInt> _clickables = new();

    private void Start()
    {
        _mesh = GetComponent<MeshController>();

        foreach (KeyValuePair<string, object> item in Shapes.Instance.Conway)
            _conwayShapes.Add(item.Value as Shape);

        CameraController.Instance.resize += OnCameraResize;
    }

    public void OnCameraResize(Rect bounds)
    {
        Size = new((bounds.width - bounds.height) / 2, bounds.height);
        Position = new(bounds.xMin + Size.x / 2, 0);
        Resolution = new((int)(512 * Size.x / Size.y), 512);

        Draw();
    }

    #region void Click
    public void Click(Vector2Int position)
    {
        foreach ((Clickable name, RectInt rect) in _clickables)
        {
            if (rect.Contains(position))
            {
                switch (name)
                {
                    case Clickable.FpsDown:
                        break;
                    case Clickable.FpsUp:
                        break;
                    case Clickable.PrevShape:
                        Shape = _conwayShapes[(--_shapeIndex + _conwayShapes.Count) % _conwayShapes.Count];
                        break;
                    case Clickable.NextShape:
                        Shape = _conwayShapes[(++_shapeIndex + _conwayShapes.Count) % _conwayShapes.Count];
                        break;
                    case Clickable.FlipX:
                        Shape = Shape.FlipX;
                        break;
                    case Clickable.FlipY:
                        Shape = Shape.FlipY;
                        break;
                    case Clickable.RotLeft:
                        Shape = Shape.RotateLeft;
                        break;
                    case Clickable.RotRight:
                        Shape = Shape.RotateRight;
                        break;
                    default:
                        break;
                }
                break;
            }
        }
    }

    public void Click(int i)
    {
        Click(new Vector2Int(i % Resolution.x, i / Resolution.x));
    }
    #endregion

    private void Draw()
    {
        _mesh.Clear();
        _clickables.Clear();
        DrawFPS();
        DrawSelector();
    }

    private void DrawFPS()
    {
        DrawShape(9, Resolution.y - 9, Shapes.Instance.Font.F);
        DrawShape(27, Resolution.y - 9, Shapes.Instance.Font.P);
        DrawShape(45, Resolution.y - 9, Shapes.Instance.Font.S);

        _clickables.Add(Clickable.FpsDown, DrawShape(81, Resolution.y - 9, Shapes.Instance.UI.ArrowLeft));
        _clickables.Add(Clickable.FpsUp, DrawShape(135, Resolution.y - 9, Shapes.Instance.UI.ArrowRight));
    }

    private void DrawSelector()
    {
        _clickables.Add(Clickable.FlipX, DrawShape(80, 9, Shapes.Instance.UI.FlipX));
        _clickables.Add(Clickable.FlipY, DrawShape(9, 80, Shapes.Instance.UI.FlipY));

        _clickables.Add(Clickable.PrevShape, DrawShape(32, 155, Shapes.Instance.UI.ArrowLeft));
        _clickables.Add(Clickable.NextShape, DrawShape(128, 155, Shapes.Instance.UI.ArrowRight));

        _clickables.Add(Clickable.RotLeft, DrawShape(64, 155, Shapes.Instance.UI.RotateLeft));
        _clickables.Add(Clickable.RotRight, DrawShape(96, 155, Shapes.Instance.UI.RotateRight));

        Shape = Shape;
    }

    #region RectInt DrawShape
    RectInt DrawShape(int x, int y, Shape shape, Color32 color)
    {
        foreach (var position in shape.Positions)
            _mesh.SetPixel(x + position.x, y + position.y, color);
        return new RectInt(
            x - shape.Width / 2,
            y - shape.Height / 2,
            shape.Width,
            shape.Height
        );
    }

    RectInt DrawShape(int x, int y, Shape shape)
    {
        return DrawShape(x, y, shape, Color.white);
    }
    #endregion
}
