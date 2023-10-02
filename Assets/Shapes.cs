using System.Collections.Generic;
using System;
using UnityEngine;
using JetBrains.Annotations;
using UnityEditor.Tilemaps;

public sealed class Shapes
{
    private Shapes() { }
    private static readonly Lazy<Shapes> lazy = new(() => new Shapes());
    public static Shapes Instance { get => lazy.Value; }

    public struct IShape
    {
        public int[,] Shape { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Vector2Int Center { get; private set; }
        public Vector2Int[] Positions { get; private set; }

        public IShape(int[,] shape)
        {
            Shape = shape;
            Width = shape.GetLength(0);
            Height = shape.GetLength(1);
            Center = new Vector2Int(Width / 2, Height / 2);

            List<Vector2Int> positions = new();
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (Shape[x, y] == 1)
                        positions.Add(new Vector2Int(x - Center.x, y - Center.y));
            Positions = positions.ToArray();
        }

        public readonly IShape FlipX
        {
            get
            {
                int[,] temp = new int[Width, Height];

                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                        temp[x, y] = Shape[Height - x - 1, y];

                return new IShape(temp);
            }
        }

        public readonly IShape FlipY
        {
            get
            {
                int[,] temp = new int[Width, Height];

                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                        temp[x, y] = Shape[x, Height - y - 1];

                return new IShape(temp);
            }
        }

        public readonly IShape FlipXY {  get => FlipX.FlipY;  }

        public readonly IShape Rotate180 { get => FlipXY; }
    }

    public IShape Neighbors = new(new int[,] {
        { 1, 1, 1 },
        { 1, 0, 1 },
        { 1, 1, 1 }
    });

    #region Still lifes
    public IShape Beehive = new(new int[,] {
        { 0, 1, 1, 0 },
        { 1, 0, 0, 1 },
        { 0, 1, 1, 0 }
    });

    public IShape Block = new(new int[,] {
        { 1, 1 },
        { 1, 1 },
    });

    public IShape Boat = new(new int[,] {
        { 1, 1, 0 },
        { 1, 0, 1 },
        { 0, 1, 0 }
    });

    public IShape Loaf = new(new int[,] {
        { 0, 1, 1, 0 },
        { 1, 0, 0, 1 },
        { 0, 1, 0, 1 },
        { 0, 0, 1, 0 }
    });

    public IShape Tub = new(new int[,] {
        { 0, 1, 0 },
        { 1, 0, 1 },
        { 0, 1, 0 }
    });
    #endregion

    #region Oscillators
    public IShape Beacon = new(new int[,] {
        { 1, 1, 0, 0 },
        { 1, 0, 0, 0 },
        { 0, 0, 0, 1 },
        { 0, 0, 1, 1 }
    });

    public IShape Blinker = new(new int[,] {
        { 1, 1, 1 }
    });

    public IShape Toad = new(new int[,] {
        { 0, 0, 1, 0 },
        { 1, 0, 0, 1 },
        { 1, 0, 0, 1 },
        { 0, 1, 0, 0 }
    });
    #endregion

    #region Spaceships
    public IShape Glider = new(new int[,] {
        { 0, 0, 1 },
        { 1, 0, 1 },
        { 0, 1, 1 }
    });
    #endregion

    public IShape Acorn = new(new int[,] {
        { 0, 1, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 1, 0, 0, 0 },
        { 1, 1, 0, 0, 1, 1, 1 }
    });
}