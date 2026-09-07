using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    // PieceData is asigned at PieceSetup.cs
    private PieceData pieceData;
    public PieceData PieceData { get => pieceData; set => pieceData = value; }
}
