using UnityEngine;

[CreateAssetMenu(menuName = "Chess/PieceData")]
public class PieceData : ScriptableObject
{
    public string pieceName;
    public GameObject prefab;
    public Vector2Int[] initialPositions; // Position where each piece spawns
    public bool isWhite;
}