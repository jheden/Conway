using UnityEngine;

[RequireComponent(typeof(MeshController))]
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

    public int Resolution
    {
        get => _mesh.Resolution.x;
        set =>_mesh.Resolution = new(value, value);
    }

    public Shape Shape {
        get => _shape;
        set {
            _shape = value;
            Resolution = Mathf.Max(value.Width, value.Height);

            var halfRes = Resolution / 2;
            foreach (var position in value.Positions)
                _mesh.SetPixel(position.x + halfRes, position.y + halfRes, Color.white);
        }
    }
    private Shape _shape;

    private MeshController _mesh;

    private void Start()
    {
        _mesh = GetComponent<MeshController>();
        _mesh.Size = new Vector2(3, 3);

        Shape = Shapes.Instance.Acorn;
    }
}
