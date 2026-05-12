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
            PieceData pieceData = piece.GetComponent<ChessPiece>().PieceData;
            bool isWhite = pieceData.IsWhite;

            switch (pieceData.PieceType)
            {
                case "Pawn":
                    CalculatePawnMoves(piece, isWhite);
                    break;
                case "Knight":
                    CalculateKnightMoves(piece, isWhite);
                    break;
                case "Bishop":
                    CalculateBishopMoves(piece, isWhite);
                    break;
                case "Rook":
                    CalculateRookMoves(piece, isWhite);
                    break;
                case "Queen":
                    CalculateQueenMoves(piece, isWhite);
                    break;
                case "King":
                    CalculateKingMoves(piece);
                    break;
            }
        }
    }

    void CalculatePawnMoves(GameObject pieceGO, bool isWhite)
    {
        if (BoardState.Instance.IsKingInDoubleCheck(isWhite)) return;

        int direction = (isWhite ^ BoardState.Instance.IsBoardInverted) ? 1 : -1;
        int initialRow = (isWhite ^ BoardState.Instance.IsBoardInverted) ? 1 : 6;

        Vector2Int currentPos = Vector2Int.RoundToInt(pieceGO.transform.position);
        Vector2Int forward = currentPos + new Vector2Int(0, direction);
        Vector2Int doubleForward = currentPos + new Vector2Int(0, 2 * direction);

        Vector2Int topRight = currentPos + new Vector2Int(1, direction);
        Vector2Int topLeft = currentPos + new Vector2Int(-1, direction);

        List<Vector2Int> pieceLegalMoves = new();

        if (!BoardState.Instance.IsKingInCheck(isWhite))
        {
            if (BoardUtils.SquareIsEmpty(forward))
                pieceLegalMoves.Add(forward);

            if (currentPos.y == initialRow && BoardUtils.SquareIsEmpty(forward) && BoardUtils.SquareIsEmpty(doubleForward))
                pieceLegalMoves.Add(doubleForward);

            if (BoardUtils.PieceIsOpponent(topRight, pieceGO))
                pieceLegalMoves.Add(topRight);

            if (BoardUtils.PieceIsOpponent(topLeft, pieceGO))
                pieceLegalMoves.Add(topLeft);
        }

        else
        {
            var targetDict = isWhite ? BoardState.Instance.BlackCheckPaths : BoardState.Instance.WhiteCheckPaths;
            foreach (var array in targetDict)
            {
                if (BoardUtils.SquareIsEmpty(forward) && array.Value.Contains(forward))
                    pieceLegalMoves.Add(forward);

                if (currentPos.y == initialRow && BoardUtils.SquareIsEmpty(forward) && BoardUtils.SquareIsEmpty(doubleForward) && array.Value.Contains(doubleForward))
                    pieceLegalMoves.Add(doubleForward);

                if (BoardUtils.PieceIsOpponent(topRight, pieceGO) && array.Value.Contains(topRight))
                    pieceLegalMoves.Add(topRight);

                if (BoardUtils.PieceIsOpponent(topLeft, pieceGO) && array.Value.Contains(topLeft))
                    pieceLegalMoves.Add(topLeft);

            }
        }

        _legalMovesByPiece[pieceGO] = pieceLegalMoves;
    }

    void CalculateKnightMoves(GameObject pieceGO, bool isWhite)
    {
        if (BoardState.Instance.IsKingInDoubleCheck(isWhite)) return;

        List<Vector2Int> pieceLegalMoves = new();
        Vector2Int[] knightMoves = _movementData.knightMoves;
        foreach (Vector2Int move in knightMoves)
        {
            Vector2Int pos = Vector2Int.RoundToInt(pieceGO.transform.position) + move;
            if (BoardUtils.SquareIsEmpty(pos) || BoardUtils.PieceIsOpponent(pos, pieceGO))
            {
                if (!BoardState.Instance.IsKingInCheck(isWhite))
                {
                    pieceLegalMoves.Add(pos);
                }

                else
                {
                    var targetDict = isWhite ? BoardState.Instance.BlackCheckPaths : BoardState.Instance.WhiteCheckPaths;
                    foreach (var array in targetDict)
                    {
                        if (!array.Value.Contains(pos)) continue;
                        else pieceLegalMoves.Add(pos);
                    }
                }
            }
        }
        _legalMovesByPiece[pieceGO] = pieceLegalMoves;
    }

    void CalculateBishopMoves(GameObject pieceGO, bool isWhite)
    {
        if (BoardState.Instance.IsKingInDoubleCheck(isWhite)) return;

        List<Vector2Int> pieceLegalMoves = new();
        Vector2Int[] bishopDirections = _movementData.bishopDirections;
        foreach (Vector2Int direction in bishopDirections)
        {
            Vector2Int pos = Vector2Int.RoundToInt(pieceGO.transform.position) + direction;
            while (BoardUtils.SquareIsEmpty(pos) || BoardUtils.PieceIsOpponent(pos, pieceGO))
            {
                if (!BoardState.Instance.IsKingInCheck(isWhite))
                {
                    pieceLegalMoves.Add(pos);
                    if (BoardUtils.PieceIsOpponent(pos, pieceGO)) break;
                }

                else
                {
                    var targetDict = isWhite ? BoardState.Instance.BlackCheckPaths : BoardState.Instance.WhiteCheckPaths;
                    foreach (var array in targetDict)
                    {
                        if (!array.Value.Contains(pos)) continue;
                        else pieceLegalMoves.Add(pos);
                    }
                }

                pos += direction;
            }
        }
        _legalMovesByPiece[pieceGO] = pieceLegalMoves;
    }

    void CalculateRookMoves(GameObject pieceGO, bool isWhite)
    {
        if (BoardState.Instance.IsKingInDoubleCheck(isWhite)) return;

        List<Vector2Int> pieceLegalMoves = new();
        Vector2Int[] rookDirections = _movementData.rookDirections;
        foreach (Vector2Int direction in rookDirections)
        {
            Vector2Int pos = Vector2Int.RoundToInt(pieceGO.transform.position) + direction;
            while (BoardUtils.SquareIsEmpty(pos) || BoardUtils.PieceIsOpponent(pos, pieceGO))
            {
                if (!BoardState.Instance.IsKingInCheck(isWhite))
                {
                    pieceLegalMoves.Add(pos);
                    if (BoardUtils.PieceIsOpponent(pos, pieceGO)) break;
                }

                else
                {
                    var targetDict = isWhite ? BoardState.Instance.BlackCheckPaths : BoardState.Instance.WhiteCheckPaths;
                    foreach (var array in targetDict)
                    {
                        if (!array.Value.Contains(pos)) continue;
                        else pieceLegalMoves.Add(pos);
                    }
                }

                pos += direction;
            }
        }
        _legalMovesByPiece[pieceGO] = pieceLegalMoves;
    }

    void CalculateQueenMoves(GameObject pieceGO, bool isWhite)
    {
        if (BoardState.Instance.IsKingInDoubleCheck(isWhite)) return;

        List<Vector2Int> pieceLegalMoves = new();
        Vector2Int[] queenDirections = _movementData.queenDirections;
        foreach (Vector2Int direction in queenDirections)
        {
            Vector2Int pos = Vector2Int.RoundToInt(pieceGO.transform.position) + direction;
            while (BoardUtils.SquareIsEmpty(pos) || BoardUtils.PieceIsOpponent(pos, pieceGO))
            {
                if (!BoardState.Instance.IsKingInCheck(isWhite))
                {
                    pieceLegalMoves.Add(pos);
                    if (BoardUtils.PieceIsOpponent(pos, pieceGO)) break;
                }

                else
                {
                    var targetDict = isWhite ? BoardState.Instance.BlackCheckPaths : BoardState.Instance.WhiteCheckPaths;
                    foreach (var array in targetDict)
                    {
                        if (!array.Value.Contains(pos)) continue;
                        else pieceLegalMoves.Add(pos);
                    }
                }

                pos += direction;
            }
        }
        _legalMovesByPiece[pieceGO] = pieceLegalMoves;
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
            if (BoardUtils.SquareIsEmpty(pos) && !BoardState.Instance.SquareIsThreatened(pos, kingGO))
            {
                pieceLegalMoves.Add(pos);
            }
            else if (BoardUtils.PieceIsOpponent(pos, kingGO) && !BoardState.Instance.SquareIsThreatened(pos, kingGO))
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
