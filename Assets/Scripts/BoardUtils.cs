using UnityEngine;
using UnityEngine.UIElements;

public static class BoardUtils
{
    public static bool SquareIsEmpty(Vector2Int position)
    {
        Debug.Log($"{position}: {BoardGenerator.Instance.Squares.ContainsKey(position)} and {!BoardGenerator.Instance.PositionToPiece.ContainsKey(position)}");
        return BoardGenerator.Instance.Squares.ContainsKey(position) &&
                !BoardGenerator.Instance.PiecesOnBoard.ContainsValue(position);
    }

    public static bool PieceIsOpponent(Vector2Int position, GameObject movedPiece)
    {
        if (!SquareIsEmpty(position))
        {
            GameObject pieceToCapture = GetPieceAt(position);
            if (movedPiece != null && pieceToCapture != null)
            {
                bool movedPieceColor = movedPiece.GetComponent<ChessPiece>().PieceData.IsWhite;
                bool capturedPieceColor = pieceToCapture.GetComponent<ChessPiece>().PieceData.IsWhite;

                return movedPieceColor != capturedPieceColor;
            }
        }

        // return PieceManager.Instance.IsLegalMove(position) &&
        return false;
    }

    public static GameObject GetPieceAt(Vector2Int position)
    {
        if (BoardGenerator.Instance.PositionToPiece.TryGetValue(position, out GameObject piece)) return piece;
        return null;
    }


    public static void RefreshBoardState(Vector2Int from, Vector2Int to, GameObject piece)
    {
        BoardGenerator.Instance.PositionToPiece.Remove(from);
        BoardGenerator.Instance.PiecesOnBoard.Remove(piece);

        BoardGenerator.Instance.PiecesOnBoard[piece] = to;
        BoardGenerator.Instance.PositionToPiece[to] = piece;
    }
}
