using System.Linq;
using System;
using UnityEngine;

public class IntGrid : Grid
{
    private int[] current;

    protected override void CopyGrid()
    {
        var tmp = Array.ConvertAll(current, state => state > 0);
        int index = States.FindLastIndex(state => Enumerable.SequenceEqual(tmp, state));
        if (index != -1)
            print($"Stable state discovered at generation {index}, meaning period is {States.Count - index}");
        Last = tmp;
    }

    protected override bool GetCurrent(int i)
    {
        return current[i] > 0;
    }

    protected override void ResetGrid()
    {
        current = new int[Length];
    }

    protected override void SetCurrent(int i, bool state)
    {
        if (state) current[i] = Mathf.Max(1, current[i] + 1);
        else current[i] = Mathf.Min(0, current[i] - 1);
    }
}