using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
public class BoardState : MonoBehaviourPunCallbacks
{
    public static BoardState Instance { get; private set; }

    public Dictionary<Vector2Int, GameObject> WhiteThreatenedSquares { get; private set; } = new();
    public Dictionary<Vector2Int, GameObject> BlackThreatenedSquares { get; private set; } = new();

    private PieceMovementData _movementData;

    public Vector2Int? EnPassantTarget { get; private set; } = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(this);
        _movementData = ScriptableObject.CreateInstance<PieceMovementData>();
    }

    public void UpdateThreatenedSquares()
    {
        Instance.WhiteThreatenedSquares.Clear();
        Instance.BlackThreatenedSquares.Clear();

        foreach (var piece in BoardGenerator.Instance.PiecesOnBoard.Keys)
        {
            var data = piece.GetComponent<ChessPiece>().PieceData;
            Vector2Int pos = Vector2Int.RoundToInt(piece.transform.position);

            bool isWhite = data.Color == PlayerColor.White;
            int direction = isWhite ? 1 : -1;
            var targetDict = isWhite ? Instance.WhiteThreatenedSquares : Instance.BlackThreatenedSquares;

            switch (data.PieceType)
            {
                case PieceType.Pawn:
                    foreach (var move in Instance._movementData.pawnMoves)
                    {
                        if (move == new Vector2Int(0, 1)) continue;
                        Vector2Int targetPos = pos + move * direction;

                        if (!BoardUtils.GetSquareAt(targetPos)) continue;
                        if (BoardUtils.SquareIsEmpty(targetPos))
                        {
                            targetDict[targetPos] = piece;
                        }
                        else if (BoardUtils.GetPieceAt(targetPos))
                        {
                            targetDict[targetPos] = piece;
                        }
                    }
                    break;

                case PieceType.Knight:
                    foreach (var move in Instance._movementData.knightMoves)
                    {
                        Vector2Int targetPos = pos + move;
                        if (!BoardUtils.GetSquareAt(targetPos)) continue;
                        if (BoardUtils.SquareIsEmpty(targetPos))
                        {
                            targetDict[targetPos] = piece;
                        }
                        else if (BoardUtils.GetPieceAt(targetPos))
                        {
                            targetDict[targetPos] = piece;
                        }
                    }
                    break;

                case PieceType.Bishop:
                    foreach (var move in Instance._movementData.bishopDirections)
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            Vector2Int targetPos = pos + move * i;

                            if (!BoardUtils.GetSquareAt(targetPos))
                                break;

                            if (BoardUtils.SquareIsEmpty(targetPos))
                            {
                                targetDict[targetPos] = piece;
                            }
                            else if (BoardUtils.GetPieceAt(targetPos))
                            {
                                targetDict[targetPos] = piece;
                                break;
                            }
                        }
                    }
                    break;

                case PieceType.Rook:
                    foreach (var move in Instance._movementData.rookDirections)
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            Vector2Int targetPos = pos + move * i;

                            if (!BoardUtils.GetSquareAt(targetPos)) break;
                            if (BoardUtils.SquareIsEmpty(targetPos))
                            {
                                targetDict[targetPos] = piece;
                            }
                            else if (BoardUtils.GetPieceAt(targetPos))
                            {
                                targetDict[targetPos] = piece;
                                break;
                            }
                        }
                    }
                    break;

                case PieceType.Queen:
                    foreach (var move in Instance._movementData.queenDirections)
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            Vector2Int targetPos = pos + move * i;

                            if (!BoardUtils.GetSquareAt(targetPos)) break;
                            if (BoardUtils.SquareIsEmpty(targetPos))
                            {
                                targetDict[targetPos] = piece;
                            }
                            else if (BoardUtils.GetPieceAt(targetPos))
                            {
                                targetDict[targetPos] = piece;
                                break;
                            }
                        }
                    }
                    break;

                case PieceType.King:
                    foreach (var move in Instance._movementData.kingMoves)
                    {
                        Vector2Int targetPos = pos + move;
                        if (!BoardUtils.GetSquareAt(targetPos)) continue;
                        if (BoardUtils.SquareIsEmpty(targetPos) || BoardUtils.GetPieceAt(pos))
                        {
                            targetDict[targetPos] = piece;
                        }
                    }
                    break;
            }
        }
        DebugManager.Instance.ColorThreatenedSquares();
    }

    public bool IsKingInCheck(PlayerColor color)
    {
        foreach (var piece in BoardGenerator.Instance.PiecesOnBoard.Keys)
        {
            if (piece == null)
                continue;

            var data = piece.GetComponent<ChessPiece>().PieceData;

            if (data.PieceType != PieceType.King || data.Color != color)
                continue;

            Vector2Int kingPos = BoardGenerator.Instance.PiecesOnBoard[piece];

            var enemyColor = color == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
            return IsSquareAttackedBy(kingPos, enemyColor);
        }

        return false;
    }

    public bool IsAnyKingInCheck()
    {
        return IsKingInCheck(PlayerManager.Instance.SelfColor) ||
               IsKingInCheck(PlayerManager.Instance.EnemyColor);
    }

    public void CheckGameOver(PlayerColor turnColor)
    {
        foreach (var legalMoves in CalculateMoves.Instance.LegalMovesByPiece)
        {
            if (legalMoves.Key == null)
                continue;

            var pieceData = legalMoves.Key.GetComponent<ChessPiece>().PieceData;

            if (pieceData.Color != turnColor)
                continue;

            // If color evaluated has any legal move return, since it means the game's not over yet
            if (legalMoves.Value.Count > 0)
            {
                return;
            }
        }

        // If all conditions passed, it's game over because 'turnColor' can't move
        bool inCheck = IsKingInCheck(turnColor);
        if (inCheck)
        {
            var selfResult = turnColor == PlayerManager.Instance.SelfColor ? GameResult.Lose : GameResult.Win;
            GameManager.Instance.TriggerGameOver(selfResult, GameOverReason.Checkmate);
        }
        else GameManager.Instance.TriggerGameOver(GameResult.Draw, GameOverReason.Stalemate);

        AudioManager.Instance.PlaySFX(AudioManager.Instance.GameEnd);
    }

    public void HandleEnPassant(Vector2Int from, Vector2Int to, PieceData data)
    {
        if (to == EnPassantTarget)
        {
            var capturedPos = new Vector2Int(to.x, from.y);
            var capturedPiece = BoardUtils.GetPieceAt(capturedPos);
            PieceManager.CapturePiece(capturedPiece, capturedPos);
        }

        EnPassantTarget = null;

        if (data.PieceType == PieceType.Pawn && Mathf.Abs(from.y - to.y) == 2)
            EnPassantTarget = new(from.x, (from.y + to.y) / 2);
    }

    public bool IsSquareAttackedBy(Vector2Int target, PlayerColor attackerColor)
    {
        foreach (var piece in BoardGenerator.Instance.PiecesOnBoard.Keys)
        {
            if (piece == null)
                continue;

            var data = piece.GetComponent<ChessPiece>().PieceData;

            if (data.Color != attackerColor)
                continue;

            Vector2Int from = BoardGenerator.Instance.PiecesOnBoard[piece];

            switch (data.PieceType)
            {
                case PieceType.Pawn:
                    if (PawnAttacks(from, target, attackerColor))
                        return true;
                    break;

                case PieceType.Knight:
                    if (KnightAttacks(from, target))
                        return true;
                    break;

                case PieceType.Bishop:
                    if (DiagonalAttacks(from, target))
                        return true;
                    break;

                case PieceType.Rook:
                    if (StraightAttacks(from, target))
                        return true;
                    break;

                case PieceType.Queen:
                    if (DiagonalAttacks(from, target) || StraightAttacks(from, target))
                        return true;
                    break;

                case PieceType.King:
                    if (KingAttacks(from, target))
                        return true;
                    break;
            }
        }

        return false;
    }

    private bool PawnAttacks(Vector2Int from, Vector2Int target, PlayerColor color)
    {
        int direction = color == PlayerColor.White ? 1 : -1;

        return target == from + new Vector2Int(1, direction) ||
               target == from + new Vector2Int(-1, direction);
    }

    private bool KnightAttacks(Vector2Int from, Vector2Int target)
    {
        foreach (var direction in _movementData.knightMoves)
        {
            var pos = from + direction;
            if (pos == target) return true;
        }
        return false;
    }

    private bool DiagonalAttacks(Vector2Int from, Vector2Int target)
    {
        foreach (Vector2Int direction in _movementData.bishopDirections)
        {
            Vector2Int pos = from + direction;

            while (BoardUtils.SquareIsEmpty(pos) || pos == target)
            {
                if (pos == target) return true;
                pos += direction;
            }
        }
        return false;
    }

    private bool StraightAttacks(Vector2Int from, Vector2Int target)
    {
        foreach (var direction in _movementData.rookDirections)
        {
            Vector2Int pos = from + direction;

            while (BoardUtils.SquareIsEmpty(pos) || pos == target)
            {
                if (pos == target) return true;
                pos += direction;
            }
        }
        return false;
    }

    private bool KingAttacks(Vector2Int from, Vector2Int target)
    {
        foreach (var direction in _movementData.kingMoves)
        {
            var pos = from + direction;
            if (pos == target) return true;
        }
        return false;
    }
}