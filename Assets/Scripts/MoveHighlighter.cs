using System.Collections.Generic;
using Mono.Cecil.Cil;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

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
    [SerializeField] private GameObject _highlightPrefab;

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
        PieceData pieceData = pieceGO.GetComponent<ChessPiece>().PieceData;
        bool isWhite = pieceData.IsWhite;

        switch (pieceName)
        {
            case "Pawn":
                ShowPawnMoves(pieceGO, isWhite);
                break;
            case "Knight":
                ShowKnightMoves(pieceGO);
                break;
            case "Bishop":
                ShowBishopMoves(pieceGO);
                break;
            case "Rook":
                ShowRookMoves(pieceGO);
                break;
            case "Queen":
                ShowQueenMoves(pieceGO);
                break;
            case "King":
                ShowKingMoves(pieceGO);
                break;
        }
    }

    #region ShowMoves Methods

    void ShowPawnMoves(GameObject pieceGO, bool isWhite)
    {
        int direction = isWhite ? 1 : -1;
        int initialRow = isWhite ? 1 : 6;

        Vector2Int currentPos = Vector2Int.RoundToInt(pieceGO.transform.position);
        Vector2Int forward = currentPos + new Vector2Int(0, direction);
        Vector2Int doubleForward = currentPos + new Vector2Int(0, 2 * direction);

        if (BoardUtils.SquareIsEmpty(forward))
        {
            // Highlight for one square forward
            ShowHighlight(_highlightPrefab, forward);

            // Can move two squares forward if on initial row
            if (currentPos.y == initialRow &&
                BoardUtils.SquareIsEmpty(forward) &&
                BoardUtils.SquareIsEmpty(doubleForward))
            {
                ShowHighlight(_highlightPrefab, doubleForward);
            }
        }
    }

    void ShowKnightMoves(GameObject pieceGO)
    {
        Vector2Int[] knightMoves = _movementData.knightMoves;
        foreach (Vector2Int move in knightMoves)
        {
            Vector2Int pos = Vector2Int.RoundToInt(pieceGO.transform.position) + move;
            if (BoardUtils.SquareIsEmpty(pos))
            {
                ShowHighlight(_highlightPrefab, pos);
            }
        }
    }

    void ShowBishopMoves(GameObject pieceGO)
    {
        Vector2Int[] bishopDirections = _movementData.bishopDirections;
        foreach (Vector2Int direction in bishopDirections)
        {
            Vector2Int pos = Vector2Int.RoundToInt(pieceGO.transform.position) + direction;
            while (BoardUtils.SquareIsEmpty(pos))
            {
                ShowHighlight(_highlightPrefab, pos);
                pos += direction;
            }
        }
    }

    void ShowRookMoves(GameObject pieceGO)
    {
        Vector2Int[] rookDirections = _movementData.rookDirections;
        foreach (Vector2Int direction in rookDirections)
        {
            Vector2Int pos = Vector2Int.RoundToInt(pieceGO.transform.position) + direction;
            while (BoardUtils.SquareIsEmpty(pos))
            {
                ShowHighlight(_highlightPrefab, pos);
                pos += direction;
            }
        }
    }

    void ShowQueenMoves(GameObject pieceGO)


    {
        Vector2Int[] queenDirections = _movementData.queenDirections;
        foreach (Vector2Int direction in queenDirections)
        {
            Vector2Int pos = Vector2Int.RoundToInt(pieceGO.transform.position) + direction;
            while (BoardUtils.SquareIsEmpty(pos))
            {
                ShowHighlight(_highlightPrefab, pos);
                pos += direction;
            }
        }
    }

    void ShowKingMoves(GameObject gameObject)
    {
        Vector2Int[] kingMoves = _movementData.kingMoves;
        Vector2Int currentPos = Vector2Int.RoundToInt(gameObject.transform.position);
        foreach (Vector2Int move in kingMoves)
        {
            Vector2Int pos = currentPos + move;
            if (BoardUtils.SquareIsEmpty(pos))
            {
                ShowHighlight(_highlightPrefab, pos);
            }
        }
    }

    #endregion

    void ShowHighlight(GameObject highlightPrefab, Vector2Int pos)
    {
        GameObject highlight = Instantiate(highlightPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        activeHighlights.Add(highlight);
        legalPositions.Add(pos);
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

    // Overload to match Action<GameObject> delegate
    public void ClearHighlights(GameObject pieceGO)
    {
        ClearHighlights();
    }

    // Overload to match Action<GameObject, Vector2Int, Vector2Int> delegate
    public void ClearHighlights(GameObject piece, Vector2Int from, Vector2Int to)
    {
        ClearHighlights();
    }
}
