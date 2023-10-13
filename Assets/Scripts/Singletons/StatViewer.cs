using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshController))]
[RequireComponent(typeof(MeshCollider))]
public class StatViewer : MonoBehaviour
{
    #region Singleton
    public static StatViewer Instance { get; private set; }
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
    #endregion

    private MeshController _mesh;

    private enum Clickable
    {
        Exit
    };
    private Dictionary<Clickable, RectInt> _clickables = new();

    private void Start()
    {
        _mesh = GetComponent<MeshController>();

        CameraController.Instance.resize += OnCameraResize;
    }

    public void OnCameraResize(Rect bounds)
    {
        Size = new((bounds.width - bounds.height) / 2, bounds.height);
        Position = new(bounds.xMax - Size.x / 2, 0);
        Resolution = new((int)(512 * Size.x / Size.y), 512);

        Draw();
    }

    public void Click(int i)
    {
        Click(new Vector2Int(i % Resolution.x, i / Resolution.x));
    }

    public void Click(Vector2Int position)
    {
        foreach ((Clickable name, RectInt rect) in _clickables)
        {
            if (rect.Contains(position))
            {
                switch (name)
                {
                    case Clickable.Exit:
                        GameController.Instance.Quit();
                        break;
                    default:
                        break;
                }
                break;
            }
        }
    }

    private void Draw()
    {
        _mesh.Clear();
        _clickables.Clear();
        //DrawFPS();
        DrawExit();
    }

    //private void DrawFPS()
    //{
    //    DrawShape(9, Resolution.y - 9, Shapes.Instance.Font.F);
    //    DrawShape(27, Resolution.y - 9, Shapes.Instance.Font.P);
    //    DrawShape(45, Resolution.y - 9, Shapes.Instance.Font.S);

    //    _clickables.Add(Clickable.FpsDown, DrawShape(81, Resolution.y - 9, Shapes.Instance.UI.ArrowLeft));
    //    _clickables.Add(Clickable.FpsUp, DrawShape(135, Resolution.y - 9, Shapes.Instance.UI.ArrowRight));
    //}

    private void DrawExit()
    {
        _clickables.Add(Clickable.Exit, DrawShape(Resolution.x - 9, Resolution.y - 9, Shapes.Instance.UI.Exit));
    }

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
}
