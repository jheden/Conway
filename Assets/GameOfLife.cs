using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UIElements;

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
            Length = value.x * value.y;
            ResetGrid();
            UpdateVertices();
        }
    }
    private Vector2Int _resolution = new(10, 10);

    public Vector2 Size
    {
        get => _size;
        set
        {
            _size = value;
            Render();
        }
    }
    private Vector2 _size = new(5, 5);

    private readonly Vector2Int[] _directions = new Vector2Int[8]
    {
        Vector2Int.down + Vector2Int.left,
        Vector2Int.down,
        Vector2Int.down + Vector2Int.right,
        Vector2Int.left,
        Vector2Int.right,
        Vector2Int.up + Vector2Int.left,
        Vector2Int.up,
        Vector2Int.up + Vector2Int.right,
    };

    private readonly int[,] _neighbors = {
        { 1, 1, 1 },
        { 1, 0, 1 },
        { 1, 1, 1 }
    };

    private readonly int[,] _acorn = {
        { 0, 1, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 1, 0, 0, 0 },
        { 1, 1, 0, 0, 1, 1, 1 }
    };

    public bool[] grid;

    public int Length { get; private set; }

    private Mesh mesh;

    private Vector3[] _vertices;
    private int[] _triangles;

    void Awake()
    {
        mesh = new();
        GetComponent<MeshFilter>().mesh = mesh;

        Resolution = _resolution;

        Size = _size;
        //grid[184] = grid[282] = grid[284] = grid[383] = grid[384] = true;
        //grid[5000] = grid[5001] = grid[5004] = grid[5005] = grid[5006] = grid[4803] = grid[4601] = true;
        //grid[45000] = grid[45001] = grid[45004] = grid[45005] = grid[45006] = grid[44703] = grid[44401] = true;

        foreach (var index in GetIndices(12, _neighbors))
            grid[index] = true;

        Log();
    }

    void Log()
    {
        for (int y = 0; y < Resolution.y; y++)
            LogRow(y);
    }

    void LogRow(int y)
    {
        string log = "";

        for (int x = 0; x < Resolution.x; x++)
            log += grid[x + y * Resolution.x] ? "[X]" : "[  ]";

        Debug.Log(log);
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    int clickedCell = (int)((mousePos.y + 5) * Resolution.x) + (int)((mousePos.x + 10) * Resolution.x / 20);
        //    print(mousePos);
        //    print(clickedCell);

        //    Vector2Int[] _acorn = new Vector2Int[7]
        //    {
        //        Vector2Int.up * 1 + Vector2Int.left * 2,
        //        Vector2Int.zero,
        //        Vector2Int.down * 1 + Vector2Int.left * 3,
        //        Vector2Int.down * 1 + Vector2Int.left * 2,
        //        Vector2Int.down * 1 + Vector2Int.right * 1,
        //        Vector2Int.down * 1 + Vector2Int.right * 2,
        //        Vector2Int.down * 1 + Vector2Int.right * 3,
        //    };
        //    foreach (var position in _acorn)
        //        grid[clickedCell + position.x + position.y * Resolution.x] = true;
        //}
        //UpdateCells();
        //UpdateTriangles();
        //Render();
    }

    private void UpdateCells()
    {
        var newGrid = new bool[Length];

        for (int y = 0; y < Resolution.y; y++)
            for (int x = 0; x < Resolution.x; x++)
            {
                int currentCell = x + y * Resolution.x;
                int neighbours = 0;

                foreach (int position in GetIndices(currentCell, _neighbors)) {
                    
                }

                //foreach (var direction in _directions)
                //{
                //    int neighbourCell = (x + direction.x + Resolution.x) % Resolution.x + ((y + direction.y + Resolution.y) % Resolution.y) * Resolution.x;
                //    if (grid[neighbourCell]) neighbours++;
                //}

                if (grid[currentCell] && neighbours < 2) newGrid[currentCell] = false;
                else if (grid[currentCell] && (neighbours == 2 || neighbours == 3)) newGrid[currentCell] = true;
                else if (grid[currentCell] && neighbours > 3) newGrid[currentCell] = false;
                else if (!grid[currentCell] && neighbours == 3) newGrid[currentCell] = true;
                else newGrid[currentCell] = false;
            }

        System.Array.Copy(newGrid, grid, Length);
    }

    private int[] GetIndices(int center, int[,] shape)
    {
        int width = shape.GetLength(0);
        int height = shape.GetLength(1);
        int xOffset = width / 2;
        int yOffset = height / 2;
        List<int> indices = new();
        
        for (int y = 0; y < height; y++)
            for (int x = 0; x <= xOffset; x++)
                if (shape[x, y] == 1)
                    indices.Add(x - width / 2 + (y - height / 2) * Resolution.x);

        return indices.ToArray();
    }

    void Render()
    {
        mesh.Clear();
        mesh.vertices = _vertices;
        mesh.triangles = _triangles;
    }

    void UpdateTriangles()
    {
        List<int> triangles = new();

        for (int y = 0; y < Resolution.y; y++)
            for (int x = 0; x < Resolution.x; x++)
            {
                int currentCell = x + y * Resolution.x;
                if (grid[currentCell])
                {
                    int currentTri = currentCell + y;

                    triangles.Add(currentTri);

                    for (int i = 0; i < 2; i++)
                    {
                        triangles.Add(currentTri + 1);
                        triangles.Add(currentTri + Resolution.x + 1);
                    }

                    triangles.Add(currentTri + Resolution.x + 2);
                }
            }

        _triangles = triangles.ToArray();
    }

    void UpdateVertices()
    {
        List<Vector3> vertices = new();

        for (int y = 0; y < Resolution.y + 1; y++)
            for (int x = 0; x < Resolution.x + 1; x++)
                vertices.Add(new Vector3(
                    Size.x * x / Resolution.x - Size.x / 2,
                    Size.y * y / Resolution.y - Size.y / 2
                ));

        _vertices = vertices.ToArray();
    }

    private void ResetGrid()
    {
        grid = new bool[Length];
    }
}
