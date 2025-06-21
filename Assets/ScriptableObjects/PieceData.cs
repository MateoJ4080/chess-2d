using UnityEngine;

[CreateAssetMenu(menuName = "Chess/PieceData")]
public class PieceData : ScriptableObject
{
    [SerializeField] private string pieceType; // e.g., "Pawn", "Knight", etc.
    [SerializeField] private string pieceName;
    [SerializeField] private bool isWhite;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector2Int[] initialPositions;

    // Getters
    public string PieceType => pieceType;
    public string PieceName => pieceName;
    public bool IsWhite => isWhite;
    public GameObject Prefab => prefab;
    public Vector2Int[] InitialPositions => initialPositions;
}
