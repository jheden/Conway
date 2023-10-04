using System.Linq;
using System;
using UnityEngine;

public class IntGrid : Grid
{
    private int[] current;

    protected override int[] Durations { get => current; }

    protected override void SaveState()
    {
        var tmp = Array.ConvertAll(current, state => state > 0);
        //int n = States.Count;
        //int start = Mathf.Max(0, n - 100);
        //int index = States.GetRange(start, n - start).FindLastIndex(state => Enumerable.SequenceEqual(tmp, state));
        //if (index != -1)
        //    print($"Stable state discovered at generation {index}, meaning period is {States.Count - index}");
        Last = tmp;
        print(current[0]);
    }

    protected override void LoadState()
    {
        bool[] state = States.Last();

        for (int i = 0; i < Length; i++)
        {
            int gen = 0;
            for (int j = States.Count - 1; j >= 0; j--)
            {
                if (States[j][i] != state[i]) break;
                gen += States[j][i] ? 1 : -1;
            }
            current[i] = gen;
        }

        States.RemoveAt(States.Count - 1);
        print(current[0]);
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
        else current[i] = Mathf.Min(-1, current[i] - 1);
    }
}