using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CellGrid : Grid
{
    private Cell[] current;

    protected override void CopyGrid()
    {
        var tmp = Array.ConvertAll(current, state => (bool)state);
        int index = States.FindLastIndex(state => Enumerable.SequenceEqual(tmp, state));
        if (index != -1)
            print($"Stable state discovered at generation {index}, meaning period is {States.Count - index}");
        Last = tmp;
    }

    protected override bool GetCurrent(int i)
    {
        return current[i].Alive;
    }

    protected override void ResetGrid()
    {
        current = new Cell[Length];

        for (int i = 0; i < Length; i++)
            current[i] = new Cell();
    }

    protected override void SetCurrent(int i, bool state)
    {
        current[i].Alive = state;
    }
}

public class Cell
{
    public bool Alive { get; set; } = false;

    public static implicit operator bool(Cell cell) {
        return cell.Alive;
    }
}