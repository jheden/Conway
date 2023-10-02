using System.Collections.Generic;
using System;
using UnityEngine;

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
            for (int y = 0; y < Shape.GetLength(1); y++)
                for (int x = 0; x < Shape.GetLength(0); x++)
                    if (Shape[x, y] == 1)
                        positions.Add(new Vector2Int(x - Center.x, y - Center.y));
            Positions = positions.ToArray();
        }
    }

    public IShape Neighbors = new(new int[,] {
        { 1, 1, 1 },
        { 1, 0, 1 },
        { 1, 1, 1 }
    });

    public IShape Acorn = new(new int[,] {
        { 0, 1, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 1, 0, 0, 0 },
        { 1, 1, 0, 0, 1, 1, 1 }
    });

    public IShape Glider = new(new int[,] {
        { 0, 0, 1 },
        { 1, 0, 1 },
        { 0, 1, 1 }
    });
}