using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    [SerializeField] private Transform _pieceContainer; // Parent container
    [SerializeField] private PieceData[] _pieces;       // Array containing the scriptable object object of each piece
    [SerializeField] private GameObject _referenceTile; // Tile reference to get its size

    private Vector2 _tileSize;
    private float _tilePadding = 0.4f;

    void Start()
    {
        _tileSize = _referenceTile.GetComponent<SpriteRenderer>().bounds.size;
        SpawnPieces();
    }

    public void SpawnPieces()
    {
        Debug.Log("Spawning pieces...");
        foreach (var pieceData in _pieces)
        {
            foreach (var pos in pieceData.InitialPositions)
            {
                GameObject piece = Instantiate(pieceData.Prefab, _pieceContainer);
                piece.GetComponent<SpriteRenderer>().sortingOrder = 1;

                char column = (char)('A' + (int)pos.x);
                int row = (int)pos.y + 1;
                piece.name = $"{pieceData.PieceName}_{column}{row}";

                piece.transform.localPosition = new Vector2(pos.x, pos.y);

                // Scale the piece to fit the tile size
                var sr = piece.GetComponent<SpriteRenderer>();
                Vector2 pieceSize = sr.bounds.size;

                // Search piece proper size by dividing the tile size by the piece size
                float scaleX = _tileSize.x / pieceSize.x;
                float scaleY = _tileSize.y / pieceSize.y;
                float uniformScale = Mathf.Min(scaleX, scaleY);
                piece.transform.localScale = new Vector2(uniformScale - _tilePadding, uniformScale - _tilePadding);
            }
        }
    }
}
