using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class HighlightMoves : MonoBehaviourPunCallbacks
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

    [SerializeField] private BoardManager _boardManager;
    [SerializeField] private PieceMovementData _movementData;
    [SerializeField] private GameObject _highlightPrefab;
    [SerializeField] private GameObject _highlightCapturePrefab;

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
        int direction = (isWhite ^ _boardManager.BoardIsInverted) ? 1 : -1;
        int initialRow = (isWhite ^ _boardManager.BoardIsInverted) ? 1 : 6;

        Vector2Int currentPos = Vector2Int.RoundToInt(pieceGO.transform.position);
        Vector2Int forward = currentPos + new Vector2Int(0, direction);
        Vector2Int doubleForward = currentPos + new Vector2Int(0, 2 * direction);
        Debug.Log($"IsWhite: {isWhite}. BoardInverted: {_boardManager.BoardIsInverted}");

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

        Vector2Int topRight = currentPos + new Vector2Int(1, direction);
        if (BoardUtils.PieceIsOpponent(topRight, pieceGO))
            ShowHighlight(_highlightCapturePrefab, topRight);

        Vector2Int topLeft = currentPos + new Vector2Int(-1, direction);
        if (BoardUtils.PieceIsOpponent(topLeft, pieceGO))
            ShowHighlight(_highlightCapturePrefab, topLeft);
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
            if (BoardUtils.PieceIsOpponent(pos, pieceGO))
            {
                ShowHighlight(_highlightCapturePrefab, pos);
            }
        }
    }

    void ShowBishopMoves(GameObject pieceGO)
    {
        Vector2Int[] bishopDirections = _movementData.bishopDirections;
        foreach (Vector2Int direction in bishopDirections)
        {
            Vector2Int pos = Vector2Int.RoundToInt(pieceGO.transform.position) + direction;
            while (BoardUtils.SquareIsEmpty(pos) || BoardUtils.PieceIsOpponent(pos, pieceGO))
            {
                if (BoardUtils.PieceIsOpponent(pos, pieceGO))
                {
                    ShowHighlight(_highlightCapturePrefab, pos);
                    break;
                }
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
            while (BoardUtils.SquareIsEmpty(pos) || BoardUtils.PieceIsOpponent(pos, pieceGO))
            {
                if (BoardUtils.PieceIsOpponent(pos, pieceGO))
                {
                    ShowHighlight(_highlightCapturePrefab, pos);
                    break;
                }
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
            while (BoardUtils.SquareIsEmpty(pos) || BoardUtils.PieceIsOpponent(pos, pieceGO))
            {
                if (BoardUtils.PieceIsOpponent(pos, pieceGO))
                {
                    ShowHighlight(_highlightCapturePrefab, pos);
                    break;
                }
                ShowHighlight(_highlightPrefab, pos);
                pos += direction;
            }
        }
    }

    void ShowKingMoves(GameObject pieceGO)
    {
        var data = pieceGO.GetComponent<ChessPiece>().PieceData;

        Vector2Int[] kingMoves = _movementData.kingMoves;
        Vector2Int currentPos = Vector2Int.RoundToInt(pieceGO.transform.position);
        foreach (Vector2Int move in kingMoves)
        {
            Vector2Int pos = currentPos + move;
            if (BoardUtils.SquareIsEmpty(pos) && !BoardState.SquareIsThreatened(pos, pieceGO))
            {
                ShowHighlight(_highlightPrefab, pos);
            }
            if (BoardUtils.PieceIsOpponent(pos, pieceGO) && !BoardState.SquareIsThreatened(pos, pieceGO))
            {
                ShowHighlight(_highlightCapturePrefab, pos);
            }
        }

        // Castling
        bool canCastleKingSide = GameManager.Instance.CanCastle(PieceData.RookSide.King, pieceGO);
        bool canCastleQueenSide = GameManager.Instance.CanCastle(PieceData.RookSide.Queen, pieceGO);

        if (canCastleKingSide)
        {
            if (data.IsWhite)
                ShowHighlight(_highlightPrefab, currentPos + new Vector2Int(2, 0));
            if (!data.IsWhite)
                ShowHighlight(_highlightPrefab, currentPos + new Vector2Int(-2, 0));
        }

        if (canCastleQueenSide)
        {
            if (data.IsWhite)
                ShowHighlight(_highlightPrefab, currentPos + new Vector2Int(-2, 0));
            if (!data.IsWhite)
                ShowHighlight(_highlightPrefab, currentPos + new Vector2Int(2, 0));
        }
    }

    #endregion

    void ShowHighlight(Vector2Int pos)
    {
        GameObject piece = BoardUtils.GetPieceAt(pos);
        GameObject prefab = piece == null ? _highlightPrefab : _highlightCapturePrefab;

        GameObject highlight = Instantiate(prefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
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
}
