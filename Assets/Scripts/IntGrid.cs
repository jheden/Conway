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
    }

    protected override void LoadState()
    {
        bool[] state = States.Last();

        for (int i = 0; i < Length; i++)
        {
            if (current[i] == 0) continue;

            bool broke = false;
            int gen = 0;
            for (int j = States.Count - 1; j >= 0; j--)
            {
                if (States[j][i] != state[i])
                {
                    broke = true;
                    break;
                }
                gen += States[j][i] ? 1 : -1;
            }
            current[i] = broke ? gen : Mathf.Max(0, gen);
        }

        States.RemoveAt(States.Count - 1);
    }

    protected override Color GetColor(int i)
    {
        if (current[i] == 0) return Color.black;
        if (current[i] > 0) return Color.HSVToRGB(Mathf.Lerp(1f / 36, 2f / 3, (float)current[i] / 50), 1f, 1f);
        return Color.HSVToRGB(1f / 36, 1f, Mathf.Lerp(1f, 0f, -(float)current[i] / 10));
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
        else if (current[i] == 0) return;
        else current[i] = Mathf.Min(-1, current[i] - 1);
    }
}