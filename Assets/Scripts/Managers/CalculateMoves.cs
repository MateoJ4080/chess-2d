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

        foreach (var piece in new List<GameObject>(BoardGenerator.Instance.PiecesOnBoard.Keys))
        {
            var data = piece.GetComponent<ChessPiece>().PieceData;
            PlayerColor color = data.Color;
            switch (data.PieceType)
            {
                case "Pawn":
                    CalculatePawnMoves(piece);
                    break;
                case "Knight":
                    CalculateKnightMoves(piece);
                    break;
                case "Bishop":
                    CalculateBishopMoves(piece);
                    break;
                case "Rook":
                    CalculateRookMoves(piece);
                    break;
                case "Queen":
                    CalculateQueenMoves(piece);
                    break;
                case "King":
                    CalculateKingMoves(piece);
                    break;
            }
        }
    }

    void CalculatePawnMoves(GameObject pawnGO)
    {
        bool isWhite = PlayerManager.Instance.SelfColor == PlayerColor.White;
        int direction = (isWhite ^ BoardState.Instance.IsBoardInverted) ? 1 : -1;
        int initialRow = (isWhite ^ BoardState.Instance.IsBoardInverted) ? 1 : 6;

        Vector2Int currentPos = Vector2Int.RoundToInt(pawnGO.transform.position);

        Vector2Int forward = currentPos + new Vector2Int(0, direction);
        Vector2Int doubleForward = currentPos + new Vector2Int(0, 2 * direction);
        Vector2Int topRight = currentPos + new Vector2Int(1, direction);
        Vector2Int topLeft = currentPos + new Vector2Int(-1, direction);

        List<Vector2Int> pieceLegalMoves = new();

        if (BoardUtils.SquareIsEmpty(forward))
            AddIfLegal(pawnGO, currentPos, forward, pieceLegalMoves);

        if (currentPos.y == initialRow && BoardUtils.SquareIsEmpty(forward) && BoardUtils.SquareIsEmpty(doubleForward))
            AddIfLegal(pawnGO, currentPos, doubleForward, pieceLegalMoves);

        if (BoardUtils.PieceIsOpponent(topRight, pawnGO) || BoardState.Instance.EnPassantTarget == topRight)
            AddIfLegal(pawnGO, currentPos, topRight, pieceLegalMoves);

        if (BoardUtils.PieceIsOpponent(topLeft, pawnGO) || BoardState.Instance.EnPassantTarget == topLeft)
            AddIfLegal(pawnGO, currentPos, topLeft, pieceLegalMoves);

        _legalMovesByPiece[pawnGO] = pieceLegalMoves;
    }

    void CalculateKnightMoves(GameObject knightGO)
    {
        List<Vector2Int> pieceLegalMoves = new();
        Vector2Int from = Vector2Int.RoundToInt(knightGO.transform.position);

        foreach (Vector2Int move in _movementData.knightMoves)
        {
            Vector2Int to = from + move;

            if (BoardUtils.SquareIsEmpty(to) || BoardUtils.PieceIsOpponent(to, knightGO))
                AddIfLegal(knightGO, from, to, pieceLegalMoves);
        }
        _legalMovesByPiece[knightGO] = pieceLegalMoves;
    }

    void CalculateBishopMoves(GameObject bishopGO)
    {
        List<Vector2Int> pieceLegalMoves = new();
        Vector2Int from = Vector2Int.RoundToInt(bishopGO.transform.position);

        foreach (Vector2Int direction in _movementData.bishopDirections)
        {
            Vector2Int to = from + direction;

            while (BoardUtils.SquareIsEmpty(to) || BoardUtils.PieceIsOpponent(to, bishopGO))
            {
                AddIfLegal(bishopGO, from, to, pieceLegalMoves);
                if (BoardUtils.PieceIsOpponent(to, bishopGO)) break;

                to += direction;
            }
        }
        _legalMovesByPiece[bishopGO] = pieceLegalMoves;
    }

    void CalculateRookMoves(GameObject rookGO)
    {
        List<Vector2Int> pieceLegalMoves = new();
        Vector2Int from = Vector2Int.RoundToInt(rookGO.transform.position);

        foreach (Vector2Int direction in _movementData.rookDirections)
        {
            Vector2Int to = from + direction;
            while (BoardUtils.SquareIsEmpty(to) || BoardUtils.PieceIsOpponent(to, rookGO))
            {
                AddIfLegal(rookGO, from, to, pieceLegalMoves);
                if (BoardUtils.PieceIsOpponent(to, rookGO)) break;

                to += direction;
            }
        }
        _legalMovesByPiece[rookGO] = pieceLegalMoves;
    }

    void CalculateQueenMoves(GameObject queenGO)
    {
        List<Vector2Int> pieceLegalMoves = new();
        Vector2Int from = Vector2Int.RoundToInt(queenGO.transform.position);

        foreach (Vector2Int direction in _movementData.queenDirections)
        {
            Vector2Int to = from + direction;
            while (BoardUtils.SquareIsEmpty(to) || BoardUtils.PieceIsOpponent(to, queenGO))
            {
                AddIfLegal(queenGO, from, to, pieceLegalMoves);
                if (BoardUtils.PieceIsOpponent(to, queenGO)) break;

                to += direction;
            }
        }
        _legalMovesByPiece[queenGO] = pieceLegalMoves;
    }

    void CalculateKingMoves(GameObject kingGO)
    {
        List<Vector2Int> pieceLegalMoves = new(); ;
        Vector2Int from = Vector2Int.RoundToInt(kingGO.transform.position);

        foreach (Vector2Int move in _movementData.kingMoves)
        {
            Vector2Int to = from + move;
            if (BoardUtils.SquareIsEmpty(to) || BoardUtils.PieceIsOpponent(to, kingGO))
                AddIfLegal(kingGO, from, to, pieceLegalMoves);

        }

        // Castling
        int direction = PlayerManager.Instance.SelfColor == PlayerColor.White ? 1 : -1;

        bool canCastleKingSide = GameManager.Instance.CanCastle(PieceData.RookSide.King, kingGO);
        bool canCastleQueenSide = GameManager.Instance.CanCastle(PieceData.RookSide.Queen, kingGO);

        if (canCastleKingSide)
            pieceLegalMoves.Add(from + new Vector2Int(2 * direction, 0));

        if (canCastleQueenSide)
            pieceLegalMoves.Add(from + new Vector2Int(-2 * direction, 0));

        _legalMovesByPiece[kingGO] = pieceLegalMoves;
    }

    private GameObject SimulateMove(GameObject piece, Vector2Int from, Vector2Int to)
    {
        BoardGenerator.Instance.PositionToPiece.TryGetValue(to, out GameObject capturedPiece);
        BoardGenerator.Instance.PositionToPiece.Remove(from);
        if (capturedPiece != null) BoardGenerator.Instance.PiecesOnBoard.Remove(capturedPiece);

        BoardGenerator.Instance.PositionToPiece[to] = piece;
        BoardGenerator.Instance.PiecesOnBoard[piece] = to;

        return capturedPiece;
    }

    private void UndoSimulatedMove(GameObject piece, Vector2Int from, Vector2Int to, GameObject capturedPiece)
    {
        BoardGenerator.Instance.PositionToPiece.Remove(to);
        BoardGenerator.Instance.PositionToPiece[from] = piece;
        BoardGenerator.Instance.PiecesOnBoard[piece] = from;

        if (capturedPiece != null)
        {
            BoardGenerator.Instance.PiecesOnBoard[capturedPiece] = to;
            BoardGenerator.Instance.PositionToPiece[to] = capturedPiece;
        }
    }

    private void AddIfLegal(GameObject piece, Vector2Int from, Vector2Int to, List<Vector2Int> legalMoves)
    {
        GameObject captured = SimulateMove(piece, from, to);

        if (!BoardState.Instance.IsKingInCheck(piece.GetComponent<ChessPiece>().PieceData.Color))
            legalMoves.Add(to);

        UndoSimulatedMove(piece, from, to, captured);
    }
}
