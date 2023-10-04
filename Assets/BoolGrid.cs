using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using UnityEngine;

public class BoolGrid : Grid
{
    private bool[] current;

    protected override int[] Durations
    {
        get => Array.ConvertAll(current, state => state ? -1 : 1);
    }

    protected override void SaveState()
    {
        //int index = States.FindLastIndex(state => Enumerable.SequenceEqual(current, state));
        //if (index != -1)
        //    print($"Stable state discovered at generation {index}, meaning period is {States.Count - index}");
        Last = current;
    }

    protected override void LoadState()
    {
        current = States.Last();
        States.RemoveAt(States.Count - 1);
    }

    protected override Color GetColor(int i)
    {
        return current[i] ? Color.HSVToRGB(1f / 36, 1f, 1f) : Color.black;
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