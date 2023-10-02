public class BoolGrid : Grid
{
    private bool[] current, last;

    protected override void CopyGrid()
    {
        System.Array.Copy(current, last, Length);
    }

    protected override bool GetCurrent(int i)
    {
        return current[i];
    }

    protected override bool GetLast(int i)
    {
        return last[i];
    }

    protected override void ResetGrid()
    {
        current = new bool[Length];
        last = new bool[Length];
    }

    protected override void SetCurrent(int i, bool state)
    {
        current[i] = state;
    }

    protected override void SetLast(int i, bool state)
    {
        last[i] = state;
    }
}