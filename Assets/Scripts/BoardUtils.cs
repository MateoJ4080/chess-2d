using UnityEngine;

public static class BoardUtils
{
    public static bool SquareIsAvailable(Vector2Int position)
    {
        return BoardGenerator.Instance.Squares.ContainsKey(position) &&
                !BoardGenerator.Instance.PiecesOnBoard.ContainsValue(position);
    }
}