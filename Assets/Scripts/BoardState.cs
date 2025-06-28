using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardState : MonoBehaviour
{
    public static BoardState Instance { get; private set; }

    public Dictionary<GameObject, Vector2Int[]> WhiteThreatenedSquares { get; private set; } = new();
    public Dictionary<GameObject, Vector2Int[]> BlackThreatenedSquares { get; private set; } = new();
    private PieceMovementData _movementData;
    [SerializeField] private GameObject _greenSquare;
    [SerializeField] private GameObject _redSquare;

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
                int direction = chessPiece.PieceData.IsWhite ? 1 : -1;
                bool isWhite = chessPiece.PieceData.IsWhite;
                var targetDict = isWhite ? Instance.WhiteThreatenedSquares : Instance.BlackThreatenedSquares;

                switch (pieceType)
                {
                    case "Pawn":
                        Vector2Int[] pawnCaptures = new[] {
                            pos + new Vector2Int(1, 1 * direction),
                            pos + new Vector2Int(-1, 1 * direction)
                         };

                        targetDict[piece] = pawnCaptures;
                        break;

                    case "Knight":
                        List<Vector2Int> knightCaptures = new List<Vector2Int>();
                        foreach (var targetPos in Instance._movementData.knightMoves)
                        {
                            if (BoardUtils.SquareIsEmpty(targetPos) || BoardUtils.PieceIsOpponent(targetPos, piece))
                            {
                                knightCaptures.Add(pos + targetPos);
                            }
                        }
                        targetDict[piece] = knightCaptures.ToArray();
                        break;

                    case "Bishop":
                        List<Vector2Int> bishopCaptures = new List<Vector2Int>();
                        foreach (var move in Instance._movementData.bishopDirections)
                        {
                            for (int i = 1; i < 8; i++)
                            {
                                Vector2Int targetPos = pos + move * i;
                                if (!BoardUtils.GetSquareAt(targetPos)) break;

                                if (BoardUtils.SquareIsEmpty(targetPos))
                                {
                                    bishopCaptures.Add(targetPos);
                                }
                                else if (BoardUtils.PieceIsOpponent(targetPos, piece))
                                {
                                    bishopCaptures.Add(targetPos);
                                    break;
                                }
                                else break;
                            }
                        }
                        targetDict[piece] = bishopCaptures.ToArray();
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
                                    rookCaptures.Add(targetPos);
                                }
                                else if (BoardUtils.PieceIsOpponent(targetPos, piece))
                                {
                                    rookCaptures.Add(targetPos);
                                    break;
                                }
                                else break;
                            }
                        }
                        targetDict[piece] = rookCaptures.ToArray();
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
                                    queenCaptures.Add(targetPos);
                                }
                                else if (BoardUtils.PieceIsOpponent(targetPos, piece))
                                {
                                    queenCaptures.Add(targetPos);
                                    break;
                                }
                                else break;
                            }
                        }
                        targetDict[piece] = queenCaptures.ToArray();
                        break;

                    case "King":
                        List<Vector2Int> kingCaptures = new List<Vector2Int>();
                        foreach (var targetPos in Instance._movementData.kingMoves)
                        {
                            Vector2Int newPos = pos + targetPos;
                            if (BoardUtils.GetSquareAt(newPos))
                            {
                                if (BoardUtils.SquareIsEmpty(newPos) || BoardUtils.PieceIsOpponent(newPos, piece))
                                {
                                    kingCaptures.Add(newPos);
                                }
                            }
                        }
                        targetDict[piece] = kingCaptures.ToArray();
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
            foreach (var move in entry.Value)
            {
                if (BoardUtils.GetSquareAt(move))
                {
                    GameObject colorSquare = Instantiate(Instance._greenSquare, new Vector3(move.x, move.y, 0), Quaternion.identity);
                    SpriteRenderer sr = colorSquare.GetComponent<SpriteRenderer>();
                    sr.sortingOrder = 1;
                }
            }
        }

        foreach (var entry in Instance.BlackThreatenedSquares)
        {
            foreach (var move in entry.Value)
            {
                if (BoardUtils.GetSquareAt(move))
                {
                    GameObject colorSquare = Instantiate(Instance._redSquare, new Vector3(move.x, move.y, 0), Quaternion.identity);
                    SpriteRenderer sr = colorSquare.GetComponent<SpriteRenderer>();
                    sr.sortingOrder = 1;
                }
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
}
