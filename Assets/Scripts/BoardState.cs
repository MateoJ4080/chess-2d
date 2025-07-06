using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardState : MonoBehaviour
{
    public static BoardState Instance { get; private set; }

    public Dictionary<Vector2Int, GameObject> WhiteThreatenedSquares { get; private set; } = new();
    public Dictionary<Vector2Int, GameObject> BlackThreatenedSquares { get; private set; } = new();
    [SerializeField] private GameObject _greenSquare;
    [SerializeField] private GameObject _redSquare;

    [SerializeField] private BoardManager _boardManager;
    private PieceMovementData _movementData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _movementData = ScriptableObject.CreateInstance<PieceMovementData>();
    }

    public static void UpdateThreatenedSquares()
    {
        Instance.WhiteThreatenedSquares.Clear();
        Instance.BlackThreatenedSquares.Clear();
        foreach (var piece in BoardGenerator.Instance.PiecesOnBoard.Keys)
        {
            if (piece.TryGetComponent<ChessPiece>(out var chessPiece))
            {
                Vector2Int pos = Vector2Int.RoundToInt(piece.transform.position);
                string pieceType = chessPiece.PieceData.PieceType;
                int direction = (chessPiece.PieceData.IsWhite ^ Instance._boardManager.BoardIsInverted) ? 1 : -1;
                bool isWhite = chessPiece.PieceData.IsWhite;
                var targetDict = isWhite ? Instance.WhiteThreatenedSquares : Instance.BlackThreatenedSquares;

                switch (pieceType)
                {
                    case "Pawn":
                        Vector2Int[] pawnCaptures = new[] {
                            pos + new Vector2Int(1, 1 * direction),
                            pos + new Vector2Int(-1, 1 * direction)
                         };

                        targetDict[pawnCaptures[0]] = piece;
                        targetDict[pawnCaptures[1]] = piece;
                        break;

                    case "Knight":
                        foreach (var move in Instance._movementData.knightMoves)
                        {
                            Vector2Int targetPos = pos + move;
                            targetDict[targetPos] = piece;
                        }
                        break;

                    case "Bishop":
                        foreach (var move in Instance._movementData.bishopDirections)
                        {
                            for (int i = 1; i < 8; i++)
                            {
                                Vector2Int targetPos = pos + move * i;
                                if (!BoardUtils.GetSquareAt(targetPos)) break;

                                if (BoardUtils.SquareIsEmpty(targetPos))
                                {

                                    targetDict[targetPos] = piece;
                                }
                                else if (BoardUtils.GetPieceAt(pos))
                                {

                                    targetDict[targetPos] = piece;
                                    break;
                                }
                                else break;
                            }
                        }
                        break;

                    case "Rook":
                        List<Vector2Int> rookCaptures = new List<Vector2Int>();
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
                                else if (BoardUtils.GetPieceAt(pos))
                                {
                                    targetDict[targetPos] = piece;
                                    break;
                                }
                                else break;
                            }
                        }
                        break;

                    case "Queen":
                        List<Vector2Int> queenCaptures = new List<Vector2Int>();
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
                                else if (BoardUtils.GetPieceAt(pos))
                                {
                                    targetDict[targetPos] = piece;
                                    break;
                                }
                                else break;
                            }
                        }
                        break;

                    case "King":
                        List<Vector2Int> kingCaptures = new List<Vector2Int>();
                        foreach (var move in Instance._movementData.kingMoves)
                        {
                            Vector2Int targetPos = pos + move;
                            if (BoardUtils.GetSquareAt(targetPos))
                            {
                                if (BoardUtils.SquareIsEmpty(targetPos) || BoardUtils.GetPieceAt(pos))
                                {
                                    targetDict[targetPos] = piece;
                                }
                            }
                        }
                        break;
                }
            }
        }
        ClearColorSquares();
        ColorThreatenedSquares();
    }

    public static void ColorThreatenedSquares()
    {
        foreach (var entry in Instance.WhiteThreatenedSquares)
        {
            Vector2Int move = entry.Key;
            if (BoardUtils.GetSquareAt(move))
            {
                GameObject colorSquare = Instantiate(Instance._greenSquare, new Vector3(move.x, move.y, 0), Quaternion.identity);
                SpriteRenderer sr = colorSquare.GetComponent<SpriteRenderer>();
                sr.sortingOrder = 1;
            }
        }

        foreach (var entry in Instance.BlackThreatenedSquares)
        {
            Vector2Int move = entry.Key;
            if (BoardUtils.GetSquareAt(move))
            {
                GameObject colorSquare = Instantiate(Instance._redSquare, new Vector3(move.x, move.y, 0), Quaternion.identity);
                SpriteRenderer sr = colorSquare.GetComponent<SpriteRenderer>();
                sr.sortingOrder = 1;
            }
        }
    }

    public static void ClearColorSquares()
    {
        foreach (var obj in GameObject.FindGameObjectsWithTag("ColorSquare"))
        {
            Destroy(obj);
        }
    }

    public static bool SquareIsThreatened(Vector2Int pos, GameObject pieceToMove)
    {

        ChessPiece pieceData = pieceToMove.GetComponent<ChessPiece>();
        bool isWhite = pieceData.PieceData.IsWhite;
        Dictionary<Vector2Int, GameObject> targetDict = isWhite ? Instance.BlackThreatenedSquares : Instance.WhiteThreatenedSquares;

        return targetDict.ContainsKey(pos);
    }
}
