using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CalculateMoves : MonoBehaviourPunCallbacks
{
    private Dictionary<GameObject, List<Vector2Int>> _legalMovesByPiece = new();
    public Dictionary<GameObject, List<Vector2Int>> LegalMovesByPiece => _legalMovesByPiece;

    [SerializeField] private PieceMovementData _movementData;

    public static CalculateMoves Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void CalculateAllMoves()
    {
        _legalMovesByPiece.Clear();

        foreach (var piece in BoardGenerator.Instance.PiecesOnBoard.Keys)
        {
            var data = piece.GetComponent<ChessPiece>().PieceData;
            PlayerColor color = data.Color;
            switch (data.PieceType)
            {
                case "Pawn":
                    CalculatePawnMoves(piece, color);
                    break;
                case "Knight":
                    CalculateKnightMoves(piece, color);
                    break;
                case "Bishop":
                    CalculateBishopMoves(piece, color);
                    break;
                case "Rook":
                    CalculateRookMoves(piece, color);
                    break;
                case "Queen":
                    CalculateQueenMoves(piece, color);
                    break;
                case "King":
                    CalculateKingMoves(piece);
                    break;
            }
        }
    }

    void CalculatePawnMoves(GameObject pawnGO, PlayerColor color)
    {
        if (BoardState.Instance.IsKingInDoubleCheck(color)) return;

        bool isWhite = color == PlayerColor.White;
        int direction = (isWhite ^ BoardState.Instance.IsBoardInverted) ? 1 : -1;
        int initialRow = (isWhite ^ BoardState.Instance.IsBoardInverted) ? 1 : 6;

        Vector2Int currentPos = Vector2Int.RoundToInt(pawnGO.transform.position);
        Vector2Int forward = currentPos + new Vector2Int(0, direction);
        Vector2Int doubleForward = currentPos + new Vector2Int(0, 2 * direction);

        Vector2Int topRight = currentPos + new Vector2Int(1, direction);
        Vector2Int topLeft = currentPos + new Vector2Int(-1, direction);

        List<Vector2Int> pieceLegalMoves = new();

        if (!BoardState.Instance.IsKingInCheck(color))
        {
            if (BoardUtils.SquareIsEmpty(forward))
                pieceLegalMoves.Add(forward);

            if (currentPos.y == initialRow && BoardUtils.SquareIsEmpty(forward) && BoardUtils.SquareIsEmpty(doubleForward))
                pieceLegalMoves.Add(doubleForward);

            if (BoardUtils.PieceIsOpponent(topRight, pawnGO))
                pieceLegalMoves.Add(topRight);

            if (BoardUtils.PieceIsOpponent(topLeft, pawnGO))
                pieceLegalMoves.Add(topLeft);

            if (BoardState.Instance.EnPassantTarget == topRight)
                pieceLegalMoves.Add(topRight);

            if (BoardState.Instance.EnPassantTarget == topLeft)
                pieceLegalMoves.Add(topLeft);
        }

        else
        {
            var targetDict = color == PlayerColor.White ? BoardState.Instance.BlackCheckPaths : BoardState.Instance.WhiteCheckPaths;
            foreach (var array in targetDict)
            {
                if (BoardUtils.SquareIsEmpty(forward) && array.Value.Contains(forward))
                    pieceLegalMoves.Add(forward);

                if (currentPos.y == initialRow && BoardUtils.SquareIsEmpty(forward) && BoardUtils.SquareIsEmpty(doubleForward) && array.Value.Contains(doubleForward))
                    pieceLegalMoves.Add(doubleForward);

                if (BoardUtils.PieceIsOpponent(topRight, pawnGO) && array.Value.Contains(topRight))
                    pieceLegalMoves.Add(topRight);

                if (BoardUtils.PieceIsOpponent(topLeft, pawnGO) && array.Value.Contains(topLeft))
                    pieceLegalMoves.Add(topLeft);

            }
        }

        _legalMovesByPiece[pawnGO] = pieceLegalMoves;
    }

    void CalculateKnightMoves(GameObject knightGO, PlayerColor color)
    {
        if (BoardState.Instance.IsKingInDoubleCheck(color)) return;

        List<Vector2Int> pieceLegalMoves = new();
        Vector2Int[] knightMoves = _movementData.knightMoves;
        foreach (Vector2Int move in knightMoves)
        {
            Vector2Int pos = Vector2Int.RoundToInt(knightGO.transform.position) + move;
            if (BoardUtils.SquareIsEmpty(pos) || BoardUtils.PieceIsOpponent(pos, knightGO))
            {
                if (!BoardState.Instance.IsKingInCheck(color))
                {
                    pieceLegalMoves.Add(pos);
                }

                else
                {
                    var targetDict = color == PlayerColor.White ? BoardState.Instance.BlackCheckPaths : BoardState.Instance.WhiteCheckPaths;
                    foreach (var array in targetDict)
                    {
                        if (!array.Value.Contains(pos)) continue;
                        else pieceLegalMoves.Add(pos);
                    }
                }
            }
        }
        _legalMovesByPiece[knightGO] = pieceLegalMoves;
    }

    void CalculateBishopMoves(GameObject bishopGO, PlayerColor color)
    {
        if (BoardState.Instance.IsKingInDoubleCheck(color)) return;

        List<Vector2Int> pieceLegalMoves = new();
        Vector2Int[] bishopDirections = _movementData.bishopDirections;
        foreach (Vector2Int direction in bishopDirections)
        {
            Vector2Int pos = Vector2Int.RoundToInt(bishopGO.transform.position) + direction;
            while (BoardUtils.SquareIsEmpty(pos) || BoardUtils.PieceIsOpponent(pos, bishopGO))
            {
                if (!BoardState.Instance.IsKingInCheck(color))
                {
                    pieceLegalMoves.Add(pos);
                    if (BoardUtils.PieceIsOpponent(pos, bishopGO)) break;
                }

                else
                {
                    var targetDict = color == PlayerColor.White ? BoardState.Instance.BlackCheckPaths : BoardState.Instance.WhiteCheckPaths;
                    foreach (var array in targetDict)
                    {
                        if (!array.Value.Contains(pos)) continue;
                        else pieceLegalMoves.Add(pos);
                    }
                }

                pos += direction;
            }
        }
        _legalMovesByPiece[bishopGO] = pieceLegalMoves;
    }

    void CalculateRookMoves(GameObject rookGO, PlayerColor color)
    {
        if (BoardState.Instance.IsKingInDoubleCheck(color)) return;

        List<Vector2Int> pieceLegalMoves = new();
        Vector2Int[] rookDirections = _movementData.rookDirections;
        foreach (Vector2Int direction in rookDirections)
        {
            Vector2Int pos = Vector2Int.RoundToInt(rookGO.transform.position) + direction;
            while (BoardUtils.SquareIsEmpty(pos) || BoardUtils.PieceIsOpponent(pos, rookGO))
            {
                if (!BoardState.Instance.IsKingInCheck(color))
                {
                    pieceLegalMoves.Add(pos);
                    if (BoardUtils.PieceIsOpponent(pos, rookGO)) break;
                }

                else
                {
                    var targetDict = color == PlayerColor.White ? BoardState.Instance.BlackCheckPaths : BoardState.Instance.WhiteCheckPaths;
                    foreach (var array in targetDict)
                    {
                        if (!array.Value.Contains(pos)) continue;
                        else pieceLegalMoves.Add(pos);
                    }
                }

                pos += direction;
            }
        }
        _legalMovesByPiece[rookGO] = pieceLegalMoves;
    }

    void CalculateQueenMoves(GameObject queenGO, PlayerColor color)
    {
        if (BoardState.Instance.IsKingInDoubleCheck(color)) return;

        List<Vector2Int> pieceLegalMoves = new();
        Vector2Int[] queenDirections = _movementData.queenDirections;
        foreach (Vector2Int direction in queenDirections)
        {
            Vector2Int pos = Vector2Int.RoundToInt(queenGO.transform.position) + direction;
            while (BoardUtils.SquareIsEmpty(pos) || BoardUtils.PieceIsOpponent(pos, queenGO))
            {
                if (!BoardState.Instance.IsKingInCheck(color))
                {
                    pieceLegalMoves.Add(pos);
                    if (BoardUtils.PieceIsOpponent(pos, queenGO)) break;
                }

                else
                {
                    var targetDict = color == PlayerColor.White ? BoardState.Instance.BlackCheckPaths : BoardState.Instance.WhiteCheckPaths;
                    foreach (var array in targetDict)
                    {
                        if (!array.Value.Contains(pos)) continue;
                        else pieceLegalMoves.Add(pos);
                    }
                }

                pos += direction;
            }
        }
        _legalMovesByPiece[queenGO] = pieceLegalMoves;
    }

    void CalculateKingMoves(GameObject kingGO)
    {
        var data = kingGO.GetComponent<ChessPiece>().PieceData;

        List<Vector2Int> pieceLegalMoves = new();
        Vector2Int[] kingMoves = _movementData.kingMoves;
        Vector2Int currentPos = Vector2Int.RoundToInt(kingGO.transform.position);

        foreach (Vector2Int move in kingMoves)
        {
            Vector2Int pos = currentPos + move;
            if (BoardUtils.SquareIsEmpty(pos) && !BoardState.Instance.SquareIsThreatened(pos, data.Color))
            {
                pieceLegalMoves.Add(pos);
            }
            else if (BoardUtils.PieceIsOpponent(pos, kingGO) && !BoardState.Instance.SquareIsThreatened(pos, data.Color))
            {
                pieceLegalMoves.Add(pos);
            }
        }

        // Castling
        int direction = data.Color == PlayerColor.White ? 1 : -1;

        bool canCastleKingSide = GameManager.Instance.CanCastle(PieceData.RookSide.King, kingGO);
        bool canCastleQueenSide = GameManager.Instance.CanCastle(PieceData.RookSide.Queen, kingGO);

        if (canCastleKingSide)
            pieceLegalMoves.Add(currentPos + new Vector2Int(2 * direction, 0));

        if (canCastleQueenSide)
            pieceLegalMoves.Add(currentPos + new Vector2Int(-2 * direction, 0));

        _legalMovesByPiece[kingGO] = pieceLegalMoves;
    }
}
