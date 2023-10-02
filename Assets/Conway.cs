using UnityEngine;
using static Shapes;

public class Conway : Grid
{
    private Cell[] current, last;

    protected override void SetShape(int x, int y, IShape shape)
    {
        foreach (int position in GetIndices(x, y, shape))
            current[position].Alive = true;
    }

    protected override void ResetGrid()
    {
        current = new Cell[Length];
        last = new Cell[Length];

        for (int i = 0; i < Length; i++)
        {
            current[i] = new Cell();
            last[i] = new Cell();
        }
    }

    protected override void UpdateCells()
    {
        for (int i = 0; i < current.Length; i++)
            last[i].Alive = current[i].Alive;

        for (int i = 0; i < current.Length; i++)
        {
            int neighbours = 0;

            foreach (int neighbour in GetIndices(i % Resolution.x, i / Resolution.x, Shapes.Instance.Neighbors))
                if (last[neighbour].Alive) neighbours++;

            current[i].Alive = (last[i].Alive ? 2 : 3) <= neighbours && neighbours <= 3;
        }
    }

    protected override void UpdateTriangles()
    {
        _triangles.Clear();

        for (int y = 0; y < Resolution.y; y++)
            for (int x = 0; x < Resolution.x; x++)
            {
                int i = x + y * Resolution.x;

                if (!current[i].Alive) continue;

                i += y;
                _triangles.Add(i);
                _triangles.Add(i + Resolution.x + 1);
                _triangles.Add(i + 1);

                _triangles.Add(i + Resolution.x + 2);
                _triangles.Add(i + 1);
                _triangles.Add(i + Resolution.x + 1);
            }

        mesh.triangles = _triangles.ToArray();
    }
}

public class Cell
{
    public bool Alive {  get; set; } = false;
}