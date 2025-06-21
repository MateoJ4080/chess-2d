using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private GameObject _squarePrefab; // Square sprite with SpriteRenderer
    [SerializeField] private Transform _squaresContainer;
    [SerializeField] private Color _lightColor = Color.white;
    [SerializeField] private Color _darkColor = Color.gray;

    [SerializeField] private int _boardWidth = 8;
    [SerializeField] private int _boardHeight = 8;

    public Dictionary<Vector2Int, GameObject> Squares { get; private set; } = new();
    public Dictionary<GameObject, Vector2Int> PiecesOnBoard { get; private set; } = new();
    public Dictionary<Vector2Int, GameObject> PositionToPiece { get; private set; } = new();

    public static BoardGenerator Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }
    void Start()
    {
        GenerateBoard();
    }

    void GenerateBoard()
    {
        for (int x = 0; x < _boardWidth; x++)
        {
            for (int y = 0; y < _boardHeight; y++)
            {
                GameObject square = Instantiate(_squarePrefab, _squaresContainer);
                square.name = $"Square_{x}_{y}";
                square.transform.localPosition = new Vector2(x, y);

                var sr = square.GetComponent<SpriteRenderer>();
                bool isLight = (x + y) % 2 == 0;
                sr.color = isLight ? _lightColor : _darkColor;

                Squares[new Vector2Int(x, y)] = square;
            }
        }
        // "0.5f" as offset to center the board, because the tiles are positioned by their left-bottom corner
        _mainCamera.transform.position = new Vector3(_boardWidth / 2f - 0.5f, _boardHeight / 2f - 0.5f, -10f);
        // OnBoardGenerated?.invoke();
    }
}
