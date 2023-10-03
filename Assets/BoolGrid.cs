using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BoolGrid : Grid
{
    private bool[] current;

    protected override void CopyGrid()
    {
        int index = States.FindLastIndex(state => Enumerable.SequenceEqual(current, state));
        if (index != -1)
            print($"Stable state discovered at generation {index}, meaning period is {States.Count - index}");
        Last = current;
    }

    protected override bool GetCurrent(int i)
    {
        return current[i];
    }

    protected override void ResetGrid()
    {
        current = new bool[Length];
    }

    protected override void SetCurrent(int i, bool state)
    {
        current[i] = state;
    }
}