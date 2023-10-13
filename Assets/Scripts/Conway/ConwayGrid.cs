using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

[RequireComponent(typeof(MeshController))]
[RequireComponent(typeof(MeshCollider))]
public abstract class ConwayGrid : MonoBehaviour
{

    private int minSize = 10;
    private int maxSize = 50;
    private float zoomStep = 5f;

    #region Properties
    public bool Rewind { get; set; }
    public float ZoomInput { get; set; }

    public Vector2 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public Vector2Int Resolution {
        get => _mesh.Resolution;
        set
        {
            _mesh.Resolution = value;
            Length = value.x * value.y;
            ResetGrid();
        }
    }

    public float Size {
        get => _mesh.Size.x;
        set => _mesh.Size = new Vector2(value, value);
    }
    protected int Length { get; private set; }
    protected List<bool[]> States { get; } = new();
    protected abstract int[] Durations { get; }
    protected bool[] Last
    {
        get => States.Last();
        set => States.Add((bool[])value.Clone());
    }
    #endregion

    #region Internal variables
    protected MeshController _mesh;
    protected float _nextUpdate;
    #endregion

    #region Abstract methods
    protected abstract Color32 GetColor(int i);
    protected abstract bool GetCurrent(int i);
    protected abstract void ResetGrid();
    protected abstract void LoadState();
    protected abstract void SaveState();
    protected abstract void SetCurrent(int i, bool state);
    #endregion

    #region Unity methods
    private void Start()
    {
        _mesh = GetComponent<MeshController>();

        Size = 10;
        Resolution = new Vector2Int(512, 512);
    }

    private void Update()
    {
        if (ZoomInput != 0f)
            Size = Mathf.Clamp(Size + ZoomInput * zoomStep * Time.unscaledDeltaTime, minSize, maxSize);

        if (Time.time > _nextUpdate)
        {
            if (Rewind) try { LoadState(); } catch { }
            else UpdateCells();
            _nextUpdate = Time.time + 1f / 16;
        }

        UpdateColors();
    }
    #endregion

    public void Click(int pixel)
    {
        DrawShape(pixel, ShapeSelector.Instance.Shape);
    }

    #region Draw methods
    public void Clear()
    {
        _mesh.Clear();
    }

    #region public void DrawShape
    public void DrawShape(int x, int y, Shape shape)
    {
        foreach (int position in GetIndices(x, y, shape))
            SetCurrent(position, true);
    }

    public void DrawShape(int i, Shape shape)
    {
        foreach (int position in GetIndices(i % Resolution.x, i / Resolution.y, shape))
            SetCurrent(position, true);
    }

    public void DrawShape(Vector2Int pos, Shape shape)
    {
        foreach (int position in GetIndices(pos.x, pos.y, shape))
            SetCurrent(position, true);
    }
    #endregion

    public void Fill(int percent)
    {
        percent = Mathf.Clamp(percent, 0, 100);
        for (int i = 0; i < Length; i++)
            SetCurrent(i, Random.Range(0, 100) < percent);
    }
    #endregion

    protected IEnumerable<int> GetAlive()
    {
        return Enumerable.Range(0, Length).Where(i => GetCurrent(i));
    }

    protected List<int> GetIndices(int x, int y, Shape shape)
    {
        List<int> indices = new();

        foreach (var position in shape.Positions)
            indices.Add(
                (x + position.x + Resolution.x) % Resolution.x +
                ((y + position.y + Resolution.y) % Resolution.y) * Resolution.x
            );

        return indices;
    }

    protected List<int> GetNeighbors(int x, int y)
    {
        List<int> indices = new();

        for (int dy = -1; dy <= 1; dy++)
            for (int dx = -1; dx <= 1; dx++)
                if (!(dx == 0 && dy == 0))
                    indices.Add(
                        (x + dx + Resolution.x) % Resolution.x +
                        ((y + dy + Resolution.y) % Resolution.y) * Resolution.x
                    );

        return indices;
    }

    protected void UpdateCells()
    {
        int[] aliveNeighbors = new int[Length];

        foreach (int i in GetAlive())
            foreach (int neighbor in GetNeighbors(i % Resolution.x, i / Resolution.x))
                aliveNeighbors[neighbor]++;

        SaveState();

        for (int i = 0; i < Length; i++)
            SetCurrent(i, (Last[i] ? 2 : 3) <= aliveNeighbors[i] && aliveNeighbors[i] <= 3);
    }

    protected void UpdateColors()
    {
        for (int i = 0; i < Length; i++)
            _mesh.SetPixel(i, GetColor(i));
    }
}