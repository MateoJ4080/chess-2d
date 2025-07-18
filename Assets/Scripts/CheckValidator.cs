using UnityEngine;

public class CheckValidator : MonoBehaviour
{
    private GameObject _whiteKing;
    private GameObject _blackKing;
    private bool canBeCheckOnMove;
    private bool kingIsInCheck;
    private bool kingsAreInitialized;

    [SerializeField] PieceMovementData _movementData;

    public static CheckValidator Instance { get; private set; }

    public bool KingIsInCheck(GameObject king)
    {
        return kingIsInCheck;
    }

    private void InitializeKings()
    {
        foreach (var piece in BoardGenerator.Instance.PiecesOnBoard.Keys)
        {
            ChessPiece chessPiece = piece.GetComponent<ChessPiece>();

            if (chessPiece.PieceData.PieceType == "King")
            {
                if (chessPiece.PieceData.IsWhite) _whiteKing = piece;
                else _blackKing = piece;
            }
        }
        kingsAreInitialized = true;
    }

    public void LookForKingCheck(Vector2Int pos, GameObject attackingPiece)
    {
        attackingPiece.TryGetComponent(out ChessPiece attackingChessPiece);
        string pieceType = attackingChessPiece.PieceData.PieceType;

        Vector2Int[] pieceMoves;

        switch (pieceType)
        {
            case "Knight":
                pieceMoves = _movementData.knightMoves;
                break;
            case "Bishop":
                pieceMoves = _movementData.bishopDirections;
                break;
            case "Rook":
                pieceMoves = _movementData.rookDirections;
                break;
            case "Queen":
                pieceMoves = _movementData.queenDirections;
                break;
                // ------------------------------------
                // Do system to check which squares are being threatened, so king can't move those squares, and they don't show highlights
                // ------------------------------------
        }

        GameObject king = BoardUtils.GetPieceAt(pos);

        if (king == null || !king.TryGetComponent(out ChessPiece kingChessPiece))
            return;

        if (kingChessPiece.PieceData.PieceType != "King")
            return;

        if (attackingPiece == null || !attackingPiece.TryGetComponent(out ChessPiece attackerChessPiece))
            return;

        string kingColor = kingChessPiece.PieceData.IsWhite ? "White" : "Black";
        string attackingPieceColor = attackerChessPiece.PieceData.IsWhite ? "White" : "Black";

        if (attackingPieceColor != kingColor) canBeCheckOnMove = true;
    }
}
