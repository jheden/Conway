using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public sealed class Shapes : MonoBehaviour
{
    #region Singleton
    public static Shapes Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        LoadTextures();
    }
    #endregion

    #region Properties
    public dynamic Conway { get; private set; } = new ExpandoObject();
    public dynamic Helpers { get; private set; } = new ExpandoObject();
    public dynamic Font { get; private set; } = new ExpandoObject();
    public dynamic UI { get; private set; } = new ExpandoObject();
    #endregion

    #region Shape dictionaries
    [SerializeField, SerializedDictionary("Name", "Texture")]
    private SerializedDictionary<string, Texture2D> _conwayStillLifes;

    [SerializeField, SerializedDictionary("Name", "Texture")]
    private SerializedDictionary<string, Texture2D> _conwayOscillators;

    [SerializeField, SerializedDictionary("Name", "Texture")]
    private SerializedDictionary<string, Texture2D> _conwaySpaceships;

    [SerializeField, SerializedDictionary("Name", "Texture")]
    private SerializedDictionary<string, Texture2D> _conwayMetuselahs;

    [SerializeField, SerializedDictionary("Name", "Texture")]
    private SerializedDictionary<string, Texture2D> _font;

    [SerializeField, SerializedDictionary("Name", "Texture")]
    private SerializedDictionary<string, Texture2D> _helpers;

    [SerializeField, SerializedDictionary("Name", "Texture")]
    private SerializedDictionary<string, Texture2D> _ui;
    #endregion

    private void LoadTextures()
    {
        var tmp = Conway as IDictionary<string, object>;
        foreach (KeyValuePair<string, Texture2D> item in _conwayStillLifes)
            tmp[item.Key] = new Shape(item.Value);
        foreach (KeyValuePair<string, Texture2D> item in _conwayOscillators)
            tmp[item.Key] = new Shape(item.Value);
        foreach (KeyValuePair<string, Texture2D> item in _conwaySpaceships)
            tmp[item.Key] = new Shape(item.Value);
        foreach (KeyValuePair<string, Texture2D> item in _conwayMetuselahs)
            tmp[item.Key] = new Shape(item.Value);

        tmp = Font as IDictionary<string, object>;
        foreach (KeyValuePair<string, Texture2D> item in _font)
            tmp[item.Key] = new Shape(item.Value);

        tmp = Helpers as IDictionary<string, object>;
        foreach (KeyValuePair<string, Texture2D> item in _helpers)
            tmp[item.Key] = new Shape(item.Value);

        tmp = UI as IDictionary<string, object>;
        foreach (KeyValuePair<string, Texture2D> item in _ui)
            tmp[item.Key] = new Shape(item.Value);
    }
}

public class Shape
{
    #region Properties
    public int[,] Data { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Vector2Int[] Positions { get; private set; }
    #endregion

    #region Constructors and initialisation
    public Shape(int[,] shape)
    {
        Init(shape);
    }

    public Shape(Texture2D texture)
    {
        var colors = texture.GetPixels();
        int[,] shape = new int[texture.width, texture.height];
        for (int x = 0; x < texture.width; x++)
            for (int y = 0; y < texture.height; y++)
                shape[x, y] = colors[x + texture.width * y].r > 0.5f ? 1 : 0;

        Init(shape);
    }

    private void Init(int[,] shape)
    {
        Data = shape;
        Width = shape.GetLength(0);
        Height = shape.GetLength(1);
        int halfW = Width / 2;
        int halfH = Height / 2;

        List<Vector2Int> positions = new();
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (Data[x, y] == 1)
                    positions.Add(new Vector2Int(x - halfW, y - halfH));
        Positions = positions.ToArray();
    }
    #endregion

    #region Permutations

    #region Base
    public Shape FlipX
    {
        get
        {
            int[,] temp = new int[Width, Height];

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    temp[x, y] = Data[Width - x - 1, y];

            return new Shape(temp);
        }
    }

    public Shape FlipY
    {
        get
        {
            int[,] temp = new int[Width, Height];

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    temp[x, y] = Data[x, Height - y - 1];

            return new Shape(temp);
        }
    }

    public Shape Transpose
    {
        get
        {
            int[,] temp = new int[Height, Width];

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    temp[y, x] = Data[x, y];

            return new Shape(temp);
        }
    }
    #endregion

    #region Compound
    public Shape AntiTranspose { get => Transpose.FlipX.FlipY; }

    public Shape RotateLeft { get => Transpose.FlipX; }

    public Shape RotateRight { get => FlipX.Transpose; }

    public Shape RotateTwice { get => FlipX.FlipY; }
    #endregion

    #endregion
}