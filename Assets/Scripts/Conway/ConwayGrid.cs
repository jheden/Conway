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
    protected List<int> _indices = new();
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

        Size = new Vector2(20, 20);
        Resolution = new Vector2Int(256, 256);
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

    public void Fill(int percent)
    {
        percent = Mathf.Clamp(percent, 0, 100);
        for (int i = 0; i < Length; i++)
            if (Random.Range(0, 100) < percent)
                SetCurrent(i, true);
    }

    public void Click(int pixel)
    {
        AddShape(pixel, ShapeSelector.Instance.Shape);
    }

    public void AddShape(int x, int y, Shape shape)
    {
        foreach (int position in GetIndices(x, y, shape))
            SetCurrent(position, true);
    }

    public void AddShape(int i, Shape shape)
    {
        foreach (int position in GetIndices(i % Resolution.x, i / Resolution.y, shape))
            SetCurrent(position, true);
    }

    protected IEnumerable<int> GetAlive()
    {
        return Enumerable.Range(0, Length).Where(i => GetCurrent(i));
    }

    protected List<int> GetIndices(int x, int y, Shape shape)
    {
        _indices.Clear();

        foreach (var position in shape.Positions)
            _indices.Add(
                (x + position.x + Resolution.x) % Resolution.x +
                ((y + position.y + Resolution.y) % Resolution.y) * Resolution.x
            );

        return _indices;
    }

    protected void UpdateCells()
    {
        SaveState();

        for (int i = 0; i < Length; i++)
        {
            int neighbours = 0;

            foreach (int neighbour in GetIndices(i % Resolution.x, i / Resolution.x, Shapes.Instance.Helpers.Neighbourhood))
                if (Last[neighbour]) neighbours++;

            SetCurrent(i, (Last[i] ? 2 : 3) <= neighbours && neighbours <= 3);
        }
    }

    protected void UpdateColors()
    {
        for (int i = 0; i < Length; i++)
            _mesh.SetPixel(i, GetColor(i));
    }
}