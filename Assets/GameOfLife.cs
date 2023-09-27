using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UIElements;
using static Shapes;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class GameOfLife : MonoBehaviour
{
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
    private Vector2Int _resolution = new(300, 300);

    public Vector2 Size {
        get => _size;
        set
        {
            _size = value;
            Increments = value / Resolution;
            UpdateVertices();
}
    }
    private Vector2 _size = new(20, 20);

    public Vector2 Increments { get; private set; }

    public bool[] current, last;

    public int Length { get; private set; }

    private Mesh mesh;

    void Awake()
    {
        mesh = new();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        Resolution = _resolution;
        Size = _size;

        SetShape(Resolution.x/2, Resolution.y/2, Shapes.Instance.Acorn);
    }

    void Update()
    {
    }

    private void FixedUpdate()
    {
        UpdateCells();
        mesh.Clear();
        UpdateVertices();
        UpdateTriangles();
    }

    private void UpdateCells()
    {
        System.Array.Copy(current, last, Length);

        for (int y = 0; y < Resolution.y; y++)
            for (int x = 0; x < Resolution.x; x++)
            {
                int currentCell = x + y * Resolution.x;
                int neighbours = 0;

                //foreach (int neighbour in GetIndices(x, y, Shapes.Instance.Neighbors))
                //    if (last[neighbour])
                //        neighbours++;

                foreach (var direction in Shapes.Instance.Neighbors.Cock)
                {
                    int neighbourCell = (x + direction.x + Resolution.x) % Resolution.x + ((y + direction.y + Resolution.y) % Resolution.y) * Resolution.x;
                    if (last[neighbourCell]) neighbours++;
                }

                if (last[currentCell] && neighbours < 2) current[currentCell] = false;
                else if (last[currentCell] && (neighbours == 2 || neighbours == 3)) current[currentCell] = true;
                else if (last[currentCell] && neighbours > 3) current[currentCell] = false;
                else if (!last[currentCell] && neighbours == 3) current[currentCell] = true;
                else current[currentCell] = false;
            }
    }

    void SetShape(int x, int y, IShape shape)
    {
        foreach (int position in GetIndices(x, y, Shapes.Instance.Acorn))
            current[position] = true;
    }

    private int[] GetIndices(int x, int y, IShape shape)
    {
        List<int> indices = new();

        foreach (var position in shape.Cock)
            indices.Add(
                (x + position.x + Resolution.x) % Resolution.x +
                ((y + position.y + Resolution.y) % Resolution.y) * Resolution.x
            );

        return indices.ToArray();
    }

    void UpdateTriangles()
    {
        List<int> triangles = new();

        for (int y = 0; y < Resolution.y; y++)
            for (int x = 0; x < Resolution.x; x++)
            {
                int i = x + y * Resolution.x;

                if (!current[i]) continue;

                i += y;
                triangles.Add(i);
                triangles.Add(i + Resolution.x + 1);
                triangles.Add(i + 1);

                triangles.Add(i + Resolution.x + 2);
                triangles.Add(i + 1);
                triangles.Add(i + Resolution.x + 1);
            }

        mesh.triangles = triangles.ToArray();
    }

    void UpdateVertices()
    {
        List<Vector3> vertices = new();

        for (int y = 0; y <= Resolution.y; y++)
            for (int x = 0; x <= Resolution.x; x++)
                vertices.Add(new Vector3(
                    Size.x * x / Resolution.x - Size.x / 2,
                    Size.y * y / Resolution.y - Size.y / 2,
                    Mathf.Sin((x + Time.frameCount) * Mathf.Deg2Rad) + Mathf.Cos((y + Time.frameCount) * Mathf.Deg2Rad)
                ));

        mesh.vertices = vertices.ToArray();
    }

    private void ResetGrid()
    {
        current = new bool[Length];
        last = new bool[Length];
    }

    #region logging
    void Log()
    {
        for (int y = 0; y < Resolution.y; y++)
            LogRow(y);
    }

    void LogRow(int y)
    {
        string log = "";

        for (int x = 0; x < Resolution.x; x++)
            log += current[x + y * Resolution.x] ? "[X]" : "[  ]";

        Debug.Log(log);
    }
    #endregion
}

public sealed class Shapes
{
    private Shapes() { }
    private static readonly Lazy<Shapes> lazy = new(() => new Shapes());
    public static Shapes Instance { get => lazy.Value; }

    public struct IShape
    {
        public int[,] Shape { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Vector2Int Center { get; private set; }
        public Vector2Int[] Cock { get; private set; }

        public IShape(int[,] shape)
        {
            Shape = shape;
            Width = shape.GetLength(0);
            Height = shape.GetLength(1);
            Center = new Vector2Int(Width / 2, Height / 2);

            List<Vector2Int> cock = new();
            for (int y = 0; y < Shape.GetLength(1); y++)
                for (int x = 0; x < Shape.GetLength(0); x++)
                    if (Shape[x, y] == 1)
                        cock.Add(new Vector2Int(x - Center.x, y - Center.y));
            Cock = cock.ToArray();
        }
    }

    public IShape Neighbors = new(new int[,] {
        { 1, 1, 1 },
        { 1, 0, 1 },
        { 1, 1, 1 }
    });

    public IShape Acorn = new(new int[,] {
        { 0, 1, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 1, 0, 0, 0 },
        { 1, 1, 0, 0, 1, 1, 1 }
    });

    public IShape Glider = new(new int[,] {
        { 0, 0, 1 },
        { 1, 0, 1 },
        { 0, 1, 1 }
    });
}