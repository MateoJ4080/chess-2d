using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    // PieceData is asigned at PieceSpawner.cs
    private PieceData pieceData;
    public PieceData PieceData { get => pieceData; set => pieceData = value; }
}
