using UnityEngine;

[CreateAssetMenu(menuName = "Chess/PieceData")]
public class PieceData : ScriptableObject
{
    [SerializeField] private string pieceName;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector2Int[] initialPositions;
    [SerializeField] private bool isWhite;

    // Getters
    public string PieceName => pieceName;
    public GameObject Prefab => prefab;
    public Vector2Int[] InitialPositions => initialPositions;
    public bool IsWhite => isWhite;
}
