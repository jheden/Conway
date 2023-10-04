using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CellGrid : Grid
{
    private Cell[] current;

    protected override int[] Durations
    {
        get => Array.ConvertAll(current, cell => cell.Duration);
    }

    protected override void SaveState()
    {
        var tmp = Array.ConvertAll(current, state => (bool)state);
        //int index = States.FindLastIndex(state => Enumerable.SequenceEqual(tmp, state));
        //if (index != -1)
        //    print($"Stable state discovered at generation {index}, meaning period is {States.Count - index}");
        Last = tmp;
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
                if (j == 0) gen = Mathf.Max(0, gen);
            }

            current[i].Alive = state[i];
            current[i].Duration = gen;
        }

        States.RemoveAt(States.Count - 1);
    }

    protected override Color GetColor(int i)
    {
        if (current[i].Duration == 0) return Color.black;
        if (current[i].Alive) return Color.HSVToRGB(Mathf.Lerp(1f / 36, 2f / 3, (float)current[i].Duration / 50), 1f, 1f);
        return Color.HSVToRGB(1f / 36, 1f, Mathf.Lerp(1f, 0f, -(float)current[i].Duration / 10));
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
    public bool Alive { 
        get => _alive;
        set {
            _alive = value;
            if (value) Duration = Mathf.Max(1, Duration + 1);
            else if (Duration == 0) return;
            else Duration = Mathf.Min(-1, Duration - 1);
        }
    }
    private bool _alive = false;

    public int Duration { get; set; }

    public static implicit operator bool(Cell cell)
    {
        return cell.Alive;
    }

    public static implicit operator int(Cell cell)
    {
        return cell.Duration;
    }
}