using System.Collections.Generic;
using Mono.Cecil.Cil;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MoveHighlighter : MonoBehaviour
{
    private GameObject activePiece;
    public GameObject ActivePiece
    {
        get => activePiece;
        set
        {
            activePiece = value;
            if (activePiece != null)
            {
                ShowMoves(activePiece, activePiece.GetComponent<ChessPiece>().PieceData.PieceType);
            }
        }
    }

    private List<GameObject> activeHighlights = new();
    public List<GameObject> ActiveHighlights => activeHighlights;
    private List<Vector2Int> legalPositions = new();
    public List<Vector2Int> LegalPositions => legalPositions;

    [SerializeField] private PieceMovementData _movementData;
    [SerializeField] private SpriteRenderer _highlightPrefab;

    void OnEnable()
    {
        PieceManager.OnPieceSelected += ShowMoves;
        PieceManager.OnPieceMoved += ClearHighlights;
        PieceManager.OnPieceDeselected += ClearHighlights;
    }

    // Overload to match Action<GameObject> delegate
    public void ShowMoves(GameObject pieceGO)
    {
        if (pieceGO == null) return;
        var piece = pieceGO.GetComponent<ChessPiece>();
        if (piece != null)
        {
            ShowMoves(pieceGO, piece.PieceData.PieceType);
        }
    }

    public void ShowMoves(GameObject pieceGO, string pieceName)
    {
        Vector2Int[] directions = pieceName switch
        {
            "Pawn" => _movementData.pawnDirections,
            "Knight" => _movementData.knightMoves,
            "Bishop" => _movementData.bishopDirections,
            "Rook" => _movementData.rookDirections,
            "Queen" => _movementData.queenDirections,
            "King" => _movementData.kingMoves,
            _ => null
        };


        foreach (Vector2Int direction in directions)
        {
            Vector3Int position = Vector3Int.RoundToInt(pieceGO.transform.position) + new Vector3Int(direction.x, direction.y, 0);
            Vector2Int boardPosition = new Vector2Int(position.x, position.y);

            if (BoardGenerator.Instance.Squares.ContainsKey(boardPosition) && !BoardGenerator.Instance.PiecesOnBoard.ContainsValue(boardPosition))
            {
                SpriteRenderer highlight = Instantiate(_highlightPrefab, position, Quaternion.identity);
                activeHighlights.Add(highlight.gameObject);
                legalPositions.Add(boardPosition);
            }
        }
    }

    public void ClearHighlights()
    {
        foreach (var highlight in activeHighlights)
        {
            if (highlight != null)
                Destroy(highlight);
        }
        activeHighlights.Clear();
        legalPositions.Clear();
    }

    // Overload to match Action<GameObject, Vector2Int, Vector2Int> delegate
    public void ClearHighlights(GameObject piece, Vector2Int from, Vector2Int to)
    {
        ClearHighlights();
    }

    // Overload to match Action<GameObject> delegate
    public void ClearHighlights(GameObject pieceGO)
    {
        ClearHighlights();
    }
}
