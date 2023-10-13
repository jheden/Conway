using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshController))]
[RequireComponent(typeof(MeshCollider))]
public abstract class ConwayGrid : MonoBehaviour
{
    #region Properties
    public bool Rewind { get; set; }

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

    public Vector2 Size {
        get => _mesh.Size;
        set => _mesh.Size = value;
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
    protected List<int> _neighborIndices = new();
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

        Size = new Vector2(10, 10);
        Resolution = new Vector2Int(512, 512);

        foreach (int neighbor in GetIndices(0, 0, Shapes.Instance.Helpers.Neighborhood))
            _neighborIndices.Add(neighbor);
    }

    private void Update()
    {
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

    protected void UpdateCells()
    {
        SaveState();

        for (int i = 0; i < Length; i++)
        {
            int neighbors = 0;

            foreach (int neighbor in _neighborIndices)
                if (Last[(neighbor + i) % Last.Length]) neighbors++;

            SetCurrent(i, (Last[i] ? 2 : 3) <= neighbors && neighbors <= 3);
        }
    }

    protected void UpdateColors()
    {
        for (int i = 0; i < Length; i++)
            _mesh.SetPixel(i, GetColor(i));
    }
}