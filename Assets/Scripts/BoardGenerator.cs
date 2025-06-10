using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _squarePrefab; // Square sprite with SpriteRenderer
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _squaresContainer;
    [SerializeField] private Transform _piecesContainer;
    [SerializeField] private Color _lightColor = Color.white;
    [SerializeField] private Color _darkColor = Color.gray;

    [SerializeField] private int _boardWidth = 8;
    [SerializeField] private int _boardHeight = 8;

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
            }
        }
        // "0.5f" as offset to center the board, because the tiles are positioned by their left-bottom corner
        _mainCamera.transform.position = new Vector3(_boardWidth / 2f - 0.5f, _boardHeight / 2f - 0.5f, -10f);
        _piecesContainer.transform.position = _squaresContainer.transform.position;
        // OnBoardGenerated?.invoke();
    }
}
