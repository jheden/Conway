using System.Collections.Generic;
using UnityEngine;

public sealed class Shapes : MonoBehaviour
{
    public static Shapes Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        LoadTextures();
    }

    #region Shape definitions

    #region Still lifes
    [Header("Still Lifes")]
    [SerializeField]
    private Texture2D _beehive, _block, _boat, _loaf, _tub;
    public Shape Beehive { get; private set; }
    public Shape Block { get; private set; }
    public Shape Boat { get; private set; }
    public Shape Loaf { get; private set; }
    public Shape Tub { get; private set; }
    #endregion

    #region Oscillators
    [Header("Oscillators")]
    [SerializeField]
    private Texture2D _beacon, _blinker, _pulsar, _toad;
    public Shape Beacon { get; private set; }
    public Shape Blinker { get; private set; }
    public Shape Pulsar { get; private set; }
    public Shape Toad { get; private set; }
    #endregion

    #region Spaceships
    [Header("Spaceships")]
    [SerializeField]
    private Texture2D _glider;
    public Shape Glider { get; private set; }
    #endregion

    #region Metuselahs
    [Header("Metuselahs")]
    [SerializeField]
    private Texture2D _acorn;
    public Shape Acorn { get; private set; }
    #endregion

    #region Helpers
    [Header("Helpers")]
    [SerializeField]
    private Texture2D _neighbourhood;
    public Shape Neighbourhood { get; private set; }
    #endregion

    #endregion

    private void LoadTextures()
    {
        #region Still Lifes
        Beehive = new Shape(_beehive);
        Block = new Shape(_block);
        Boat = new Shape(_boat);
        Loaf = new Shape(_loaf);
        Tub = new Shape(_tub);
        #endregion

        #region Oscillators
        Beacon = new Shape(_beacon);
        Blinker = new Shape(_blinker);
        Pulsar = new Shape(_pulsar);
        Toad = new Shape(_toad);
        #endregion

        #region Spaceships
        Glider = new Shape(_glider);
        #endregion

        #region Methuselahs
        Acorn = new Shape(_acorn);
        #endregion

        #region Helpers
        Neighbourhood = new Shape(_neighbourhood);
        #endregion
    }
}

public class Shape
{
    public int[,] Data { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Vector2Int[] Positions { get; private set; }

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