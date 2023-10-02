public class CellGrid : Grid
{
    private Cell[] current, last;

    protected override void CopyGrid()
    {
        for (int i = 0; i < Length; i++)
            last[i].Alive = current[i].Alive;
    }

    protected override bool GetCurrent(int i)
    {
        return current[i].Alive;
    }

    protected override bool GetLast(int i)
    {
        return last[i].Alive;
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

    protected override void SetCurrent(int i, bool state)
    {
        current[i].Alive = state;
    }

    protected override void SetLast(int i, bool state)
    {
        last[i].Alive = state;
    }
}

public class Cell
{
    public bool Alive { get; set; } = false;
}