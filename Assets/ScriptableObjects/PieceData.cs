using UnityEngine;

[CreateAssetMenu(menuName = "Chess/PieceData")]
public class PieceData : ScriptableObject
{
    [SerializeField] private PieceType _pieceType;
    [SerializeField] private string _pieceName;
    [SerializeField] private PlayerColor _color;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Vector2Int[] _initialPositions;
    public enum RookSide { King, Queen }

    public PieceType PieceType => _pieceType;
    public string PieceName => _pieceName;
    public PlayerColor Color => _color;
    public GameObject Prefab => _prefab;
    public Vector2Int[] InitialPositions => _initialPositions;
}
