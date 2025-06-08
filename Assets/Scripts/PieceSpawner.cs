using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    public Transform pieceContainer; // Parent container
    public PieceData[] pieces;       // Array containing the scriptable object object of each piece
    public GameObject referenceTile; // Tile reference to get its size

    private Vector2 tileSize;
    private float _tilePadding = 0.4f;

    void Start()
    {
        tileSize = referenceTile.GetComponent<SpriteRenderer>().bounds.size;
        SpawnPieces();
    }

    public void SpawnPieces()
    {
        Debug.Log("Spawning pieces...");
        foreach (var pieceData in pieces)
        {
            foreach (var pos in pieceData.initialPositions)
            {
                GameObject piece = Instantiate(pieceData.prefab, pieceContainer);
                piece.GetComponent<SpriteRenderer>().sortingOrder = 1;
                piece.name = $"{(pieceData.isWhite ? "White" : "Black")}_{pieceData.pieceName}_{pos.x}_{pos.y}";
                piece.transform.localPosition = new Vector2(pos.x, pos.y);

                // Scale the piece to fit the tile size
                var sr = piece.GetComponent<SpriteRenderer>();
                Vector2 pieceSize = sr.bounds.size;

                // Search piece proper size by dividing the tile size by the piece size
                float scaleX = tileSize.x / pieceSize.x;
                float scaleY = tileSize.y / pieceSize.y;
                float uniformScale = Mathf.Min(scaleX, scaleY);
                piece.transform.localScale = new Vector2(uniformScale - _tilePadding, uniformScale - _tilePadding);
            }
        }
    }
}
