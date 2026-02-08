using UnityEngine;

[CreateAssetMenu(fileName = "PieceMovementData", menuName = "Chess/PieceMovementData")]
public class PieceMovementData : ScriptableObject
{
    public Vector2Int[] pawnMoves = new Vector2Int[]
    {
        new (-1, 1), new (0, 1), new (1, 1)
    };

    public Vector2Int[] knightMoves = new Vector2Int[] {
        new (2, 1), new(1, 2),
        new (-1, 2), new (-2, 1),
        new (-2, -1), new (-1, -2),
        new (1, -2), new (2, -1)
    };

    public Vector2Int[] bishopDirections = new Vector2Int[] {
        new (1, 1), new (-1, 1),
        new (-1, -1), new (1, -1)
    };

    public Vector2Int[] rookDirections = new Vector2Int[] {
        new (1, 0), new (-1, 0),
        new (0, 1), new (0, -1)
    };

    public Vector2Int[] queenDirections = new Vector2Int[] {
        new (1, 1), new (-1, 1),
        new (-1, -1), new (1, -1),
        new (1, 0), new (-1, 0),
        new (0, 1), new (0, -1)
    };

    public Vector2Int[] kingMoves = new Vector2Int[] {
        new (1, 1), new (-1, 1),
        new (-1, -1), new (1, -1),
        new (1, 0), new (-1, 0),
        new (0, 1), new (0, -1)
    };
}
