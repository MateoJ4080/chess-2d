using Unity.VisualScripting;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _referenceTile;
    [SerializeField] private Transform _pieceContainer;
    [SerializeField] BoardManager _boardManager;
    [SerializeField] private PieceData[] _piecesData;      // Array containing the scriptable object of each piece. Set in the inspector

    private Vector2 _tileSize;
    private float _tilePadding = 0.4f;

    void Start()
    {
        _tileSize = _referenceTile.GetComponent<SpriteRenderer>().bounds.size;
        SpawnPieces();
    }

    public void SpawnPieces()
    {
        Debug.Log("Spawning pieces...");
        foreach (var pieceData in _piecesData)
        {
            foreach (var pos in pieceData.InitialPositions)
            {
                GameObject piece = Instantiate(pieceData.Prefab, _pieceContainer);

                piece.AddComponent<Draggable>();

                // Add ChessPiece component and assign the PieceData. This is to use "PieceType" string and set the ActivePiece in MoveHighlighter
                var chessPiece = piece.AddComponent<ChessPiece>();
                chessPiece.PieceData = pieceData;

                // Position and direction
                int posY = _boardManager.BoardIsInverted ? 7 - pos.y : pos.y;
                Vector2Int piecePos = new(pos.x, posY);
                piece.transform.localPosition = new Vector3(piecePos.x, posY, 0f);

                // Find the child object containing the sprite
                Transform visual = piece.transform.Find("Visual");
                SpriteRenderer sr = visual.GetComponent<SpriteRenderer>();
                sr.sortingOrder = 2;

                // Rename using chess notation (A1-H8)
                char column = (char)('A' + piecePos.x);
                int row = piecePos.y + 1;
                piece.name = $"{pieceData.PieceName}_{column}{row}";

                // Scale the child to fit the tile with padding
                Vector2 pieceSize = sr.bounds.size;
                float scaleX = _tileSize.x / pieceSize.x;
                float scaleY = _tileSize.y / pieceSize.y;
                float uniformScale = Mathf.Min(scaleX, scaleY);
                visual.localScale = new Vector2(uniformScale - _tilePadding, uniformScale - _tilePadding);

                // Set the collider to match the tile size (unaffected by visual scaling)
                BoxCollider2D collider = piece.GetComponent<BoxCollider2D>();
                collider.size = _tileSize;
                collider.offset = Vector2.zero;

                // Store the piece in dictionary
                BoardGenerator.Instance.PiecesOnBoard[piece] = piecePos;
                BoardGenerator.Instance.PositionToPiece[piecePos] = piece;
            }
        }
        BoardState.UpdateThreatenedSquares();
        BoardState.ColorThreatenedSquares();
    }
}
