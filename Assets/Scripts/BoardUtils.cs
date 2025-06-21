using UnityEngine;
using UnityEngine.UIElements;

public static class BoardUtils
{
    public static bool SquareIsEmpty(Vector2Int position)
    {
        return BoardGenerator.Instance.Squares.ContainsKey(position) &&
                !BoardGenerator.Instance.PiecesOnBoard.ContainsValue(position);
    }

    public static bool CanCaptureAt(Vector2Int position, GameObject movedPiece)
    {
        if (!SquareIsEmpty(position))
        {
            GameObject capturedPiece = GetPieceAt(position);
            if (movedPiece != null && capturedPiece != null)
            {
                Debug.Log($"Pieces: {movedPiece}, {capturedPiece}");
                bool movedPieceColor = movedPiece.GetComponent<ChessPiece>().PieceData.IsWhite;
                bool capturedPieceColor = capturedPiece.GetComponent<ChessPiece>().PieceData.IsWhite;
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
}
