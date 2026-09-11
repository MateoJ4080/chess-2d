using UnityEngine;

public static class BoardUtils
{
    public static bool SquareIsEmpty(Vector2Int position)
    {
        return BoardGenerator.Instance.Squares.ContainsKey(position) && !BoardGenerator.Instance.PiecesOnBoard.ContainsValue(position);
    }

    public static bool PieceIsOpponent(Vector2Int position, GameObject movedPiece)
    {
        if (!SquareIsEmpty(position))
        {
            GameObject pieceToCapture = GetPieceAt(position);
            if (movedPiece != null && pieceToCapture != null)
            {
                var movedPieceColor = movedPiece.GetComponent<ChessPiece>().PieceData.Color;
                var capturedPieceColor = pieceToCapture.GetComponent<ChessPiece>().PieceData.Color;

                return movedPieceColor != capturedPieceColor;
            }
        }
        return false;
    }

    public static GameObject GetPieceAt(Vector2Int position)
    {
        if (BoardGenerator.Instance.PositionToPiece.TryGetValue(position, out GameObject piece)) return piece;
        return null;
    }

    public static bool PlayerIsThisColor(GameObject piece)
    {
        var pieceData = piece.GetComponent<ChessPiece>().PieceData;
        return pieceData.Color == PlayerManager.Instance.SelfColor;
    }

    public static void RefreshBoardState(Vector2Int from, Vector2Int to, GameObject piece)
    {
        var data = piece.GetComponent<ChessPiece>().PieceData;

        BoardGenerator.Instance.PiecesOnBoard[piece] = to;
        BoardGenerator.Instance.PositionToPiece.Remove(from);
        BoardGenerator.Instance.PositionToPiece[to] = piece;

        BoardState.Instance.HandleEnPassant(from, to, data);
        BoardState.Instance.UpdateThreatenedSquares();
        CalculateMoves.Instance.CalculateAllMoves();

        var colorToEvaluate = data.Color == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
        BoardState.Instance.CheckGameOver(colorToEvaluate);
    }

    public static GameObject GetSquareAt(Vector2Int pos)
    {
        BoardGenerator.Instance.Squares.TryGetValue(pos, out GameObject square);
        return square;
    }

    public static bool IsKingInCheck(PlayerColor color)
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
            return BoardState.Instance.IsSquareAttackedBy(kingPos, enemyColor);
        }

        return false;
    }

    public static bool IsAnyKingInCheck()
    {
        return IsKingInCheck(PlayerManager.Instance.SelfColor) ||
               IsKingInCheck(PlayerManager.Instance.EnemyColor);
    }
}
