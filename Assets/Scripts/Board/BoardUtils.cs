using Photon.Pun;
using UnityEngine;

public static class BoardUtils
{
    public static bool SquareIsEmpty(Vector2Int position)
    {
        // Debug.Log($"{position}: {BoardGenerator.Instance.Squares.ContainsKey(position)} and {!BoardGenerator.Instance.PositionToPiece.ContainsKey(position)}");
        return BoardGenerator.Instance.Squares.ContainsKey(position) && !BoardGenerator.Instance.PiecesOnBoard.ContainsValue(position);
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
        return false;
    }

    public static GameObject GetPieceAt(Vector2Int position)
    {
        if (BoardGenerator.Instance.PositionToPiece.TryGetValue(position, out GameObject piece)) return piece;
        return null;
    }

    public static bool GetPieceAt(Vector2Int position, out GameObject piece)
    {
        if (BoardGenerator.Instance.PositionToPiece.TryGetValue(position, out piece))
            return true;

        return false;
    }

    public static bool PlayerIsThisColor(GameObject piece)
    {
        if (piece == null) return false;

        string playerColor = PhotonNetwork.LocalPlayer.CustomProperties["Color"] as string;
        bool isWhite = playerColor == "White";

        var pieceData = piece.GetComponent<ChessPiece>().PieceData;

        Debug.Log($"PlayerIsThisColor: {pieceData.IsWhite == isWhite}");

        return pieceData.IsWhite == isWhite;
    }

    public static void RefreshBoardState(Vector2Int from, Vector2Int to, GameObject piece)
    {
        BoardGenerator.Instance.PiecesOnBoard.Remove(piece);
        BoardGenerator.Instance.PiecesOnBoard[piece] = to;

        BoardGenerator.Instance.PositionToPiece.Remove(from);
        BoardGenerator.Instance.PositionToPiece[to] = piece;

        BoardState.UpdateThreatenedSquares();
        CalculateMoves.Instance.CalculateAllMoves();
    }

    public static GameObject GetSquareAt(Vector2Int pos)
    {
        BoardGenerator.Instance.Squares.TryGetValue(pos, out GameObject square);
        return square;
    }
}
