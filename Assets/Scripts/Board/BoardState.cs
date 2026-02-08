using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BoardState : MonoBehaviour
{
    public static BoardState Instance { get; private set; }

    public Dictionary<Vector2Int, GameObject> WhiteThreatenedSquares { get; private set; } = new();
    public Dictionary<Vector2Int, GameObject> BlackThreatenedSquares { get; private set; } = new();
    [SerializeField] private GameObject _greenSquare;
    [SerializeField] private GameObject _redSquare;
    [SerializeField] private GameObject _yellowSquare;

    [SerializeField] private BoardManager _boardManager;
    private PieceMovementData _movementData;

    private Dictionary<GameObject, List<Vector2Int>> _whiteCheckPaths = new();
    private Dictionary<GameObject, List<Vector2Int>> _blackCheckPaths = new();

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
        Instance._whiteCheckPaths.Clear();
        Instance._blackCheckPaths.Clear();


        foreach (var piece in BoardGenerator.Instance.PiecesOnBoard.Keys)
        {
            if (piece.TryGetComponent<ChessPiece>(out var chessPiece))
            {
                Vector2Int pos = Vector2Int.RoundToInt(piece.transform.position);

                var data = chessPiece.PieceData;
                var pieceType = data.PieceType;
                var direction = (data.IsWhite ^ Instance._boardManager.BoardIsInverted) ? 1 : -1;
                var isWhite = data.IsWhite;
                var targetDict = isWhite ? Instance.WhiteThreatenedSquares : Instance.BlackThreatenedSquares;

                switch (pieceType)
                {
                    case "Pawn":
                        foreach (var move in Instance._movementData.pawnMoves)
                        {
                            if (move == new Vector2Int(0, 1)) continue;
                            Vector2Int targetPos = pos + move * direction;

                            if (!BoardUtils.GetSquareAt(targetPos)) continue;
                            if (BoardUtils.SquareIsEmpty(targetPos))
                            {
                                targetDict[targetPos] = piece;
                            }
                            else if (BoardUtils.GetPieceAt(targetPos, out GameObject targetPiece))
                            {
                                targetDict[targetPos] = piece;
                                Instance.LookForCheck(piece, targetPiece);
                            }
                        }
                        break;

                    case "Knight":
                        foreach (var move in Instance._movementData.knightMoves)
                        {
                            Vector2Int targetPos = pos + move;
                            if (!BoardUtils.GetSquareAt(targetPos)) continue;
                            if (BoardUtils.SquareIsEmpty(targetPos))
                            {
                                targetDict[targetPos] = piece;
                            }
                            else if (BoardUtils.GetPieceAt(targetPos, out GameObject targetPiece))
                            {
                                targetDict[targetPos] = piece;
                                Instance.LookForCheck(piece, targetPiece);
                            }
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
                                else if (BoardUtils.GetPieceAt(targetPos, out GameObject targetPiece))
                                {
                                    targetDict[targetPos] = piece;
                                    Instance.LookForCheck(piece, targetPiece);

                                    break;
                                }
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
                                else if (BoardUtils.GetPieceAt(targetPos, out GameObject targetPiece))
                                {
                                    targetDict[targetPos] = piece;
                                    Instance.LookForCheck(piece, targetPiece);

                                    break;
                                }
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
                                else if (BoardUtils.GetPieceAt(targetPos, out GameObject targetPiece))
                                {
                                    targetDict[targetPos] = piece;
                                    Instance.LookForCheck(piece, targetPiece);

                                    break;
                                }
                            }
                        }
                        break;

                    case "King":
                        List<Vector2Int> kingCaptures = new List<Vector2Int>();
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
        }
        ColorThreatenedSquares();
    }

    // *Debug purposes*
    public static void ColorThreatenedSquares()
    {
        ClearColorSquares();

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

        foreach (var path in Instance._whiteCheckPaths)
        {
            Debug.Log($"Creating check path for {path.Key.name}");
            foreach (var move in path.Value)
            {
                if (BoardUtils.GetSquareAt(move))
                {
                    GameObject colorSquare = Instantiate(Instance._yellowSquare, new Vector3(move.x, move.y, 0), Quaternion.identity);
                    SpriteRenderer sr = colorSquare.GetComponent<SpriteRenderer>();
                    sr.sortingOrder = 2;
                }
            }
        }

        foreach (var path in Instance._blackCheckPaths)
        {
            Debug.Log($"Creating check path for {path.Key.name}");
            foreach (var move in path.Value)
            {
                Debug.Log($"Path tiles count: {path.Value.Count}");
                if (BoardUtils.GetSquareAt(move))
                {
                    GameObject colorSquare = Instantiate(Instance._yellowSquare, new Vector3(move.x, move.y, 0), Quaternion.identity);
                    SpriteRenderer sr = colorSquare.GetComponent<SpriteRenderer>();
                    sr.sortingOrder = 2;
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

    // *Might want to move it to BoardUtils later*
    public static bool SquareIsThreatened(Vector2Int pos, GameObject pieceToMove)
    {
        ChessPiece pieceData = pieceToMove.GetComponent<ChessPiece>();
        bool isWhite = pieceData.PieceData.IsWhite;
        Dictionary<Vector2Int, GameObject> targetDict = isWhite ? Instance.BlackThreatenedSquares : Instance.WhiteThreatenedSquares;

        return targetDict.ContainsKey(pos);
    }

    private void LookForCheck(GameObject activePiece, GameObject targetPiece)
    {
        Debug.Log($"<color=cyan>LookForCheck running... {activePiece.GetComponent<ChessPiece>().PieceData.PieceType}");

        var activeData = activePiece.GetComponent<ChessPiece>().PieceData;
        var targetData = targetPiece.GetComponent<ChessPiece>().PieceData;

        Debug.Log($"<color=cyan> PieceType == King: {targetData.PieceType == "King"}");
        Debug.Log($"<color=cyan> Its enemy's color: {targetData.IsWhite != activeData.IsWhite}");

        if (targetData.PieceType == "King" && targetData.IsWhite != activeData.IsWhite)
        {
            Debug.Log("<color=cyan>Check detected");
            var from = Vector2Int.RoundToInt(activePiece.transform.position);
            var to = Vector2Int.RoundToInt(targetPiece.transform.position);

            BuildCheckPath(from, to, activeData.IsWhite);
        }
    }

    private void BuildCheckPath(Vector2Int from, Vector2Int to, bool isWhite)
    {
        var activePiece = BoardUtils.GetPieceAt(from);
        var activeData = activePiece.GetComponent<ChessPiece>().PieceData;

        Vector2Int delta = to - from;
        Vector2Int direction;

        if (activeData.PieceType != "Knight")
        {
            direction = new(
               Math.Sign(delta.x),
               Math.Sign(delta.y)
           );
        }
        else direction = delta;

        var targetDict = isWhite ? _whiteCheckPaths : _blackCheckPaths;

        List<Vector2Int> checkPath = new();

        for (int j = 0; j < 8; j++)
        {
            Vector2Int pos = from + direction * j;
            if (BoardUtils.SquareIsEmpty(pos) || j == 0)
            {
                checkPath.Add(pos);
            }
            else break;
        }
        Debug.Log($"{checkPath.Count} tiles added to path");
        targetDict.Add(activePiece, checkPath);
    }

    public void SetCheckStatus(bool isWhite, bool status)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        var props = new ExitGames.Client.Photon.Hashtable();

        var kingInCheck = isWhite ? "whiteInCheck" : "blackInCheck";

        props[kingInCheck] = status;

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    public bool IsKingInCheck(bool isWhite)
    {
        var key = isWhite ? "whiteInCheck" : "blackInCheck";

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(key, out var value))
            return (bool)value;

        Debug.Log("IsKingInCheck: King isn't registered");
        return false;
    }
}