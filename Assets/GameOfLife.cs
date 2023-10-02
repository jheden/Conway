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

                if (last[currentCell])
                    if (neighbours == 2 || neighbours == 3) current[currentCell] = true;
                    else current[currentCell] = false;
                else
                    if (neighbours == 3) current[currentCell] = true;
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