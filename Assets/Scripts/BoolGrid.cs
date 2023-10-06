using System.Linq;
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
        int gen = 0;

        for (int j = States.Count - 1; j >= 0; j--)
            if (States[j][i] != current[i])
            {
                gen = States.Count - j;
                break;
            }

        gen *= (current[i] ? 1 : -1);

        if (gen == 0) return Color.black;
        if (gen > 0) return Color.HSVToRGB(Mathf.Lerp(1f / 36, 2f / 3, (float)gen / 50), 1f, 1f);
        return Color.HSVToRGB(1f / 36, 1f, Mathf.Lerp(1f, 0f, -(float)gen / 10));
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