using UnityEngine;
using Photon.Pun;

public class PieceSetup : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    private Transform _piecesContainer;

    private readonly float tilePadding = 0.4f;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        PieceDataManager.Initialize();
        SetupPiece();
    }

    void SetupPiece()
    {
        if (_piecesContainer == null) _piecesContainer = GameObject.FindGameObjectWithTag("PiecesContainer").transform;
        string pieceDataName = (string)photonView.InstantiationData[0];
        if (pieceDataName == null) Debug.Log("pieceDataName is null");

        PieceData pieceData = PieceDataManager.Instance.GetPieceDataByName(pieceDataName);
        Vector2 tileSize = BoardGenerator.Instance.TileSize;

        Transform visual = transform.Find("Visual");
        if (visual == null)
        {
            Debug.LogError("Visual child not found on piece prefab");
            return;
        }

        SpriteRenderer sr = visual.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("SpriteRenderer not found on Visual child");
            return;
        }

        // Scale sprite to have the same size of the tile independently of its size, and then substract _tilePadding to that
        Vector2 pieceSize = sr.sprite.bounds.size;
        float scaleX = tileSize.x / pieceSize.x;
        float scaleY = tileSize.y / pieceSize.y;
        float uniformScale = Mathf.Min(scaleX, scaleY);
        visual.localScale = Vector2.one * (uniformScale - tilePadding);

        // Allow data retrieval from other scripts
        var chessPiece = gameObject.AddComponent<ChessPiece>();
        chessPiece.PieceData = pieceData;

        // Set parent and add dragging
        transform.SetParent(_piecesContainer, false);
        gameObject.AddComponent<Draggable>();

        // Position and rotation
        int x = (int)photonView.InstantiationData[1];
        int y = (int)photonView.InstantiationData[2];
        Vector2Int piecePos = new(x, y);
        gameObject.transform.localPosition = (Vector3Int)piecePos;

        var isBlackPlayer = PlayerManager.Instance.SelfColor == PlayerColor.Black;
        if (isBlackPlayer) transform.rotation = Quaternion.Euler(0, 0, 180);

        // Set the collider to match the tile size (unaffected by visual scaling)
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = tileSize;
        collider.offset = Vector2.zero;

        BoardGenerator.Instance.PiecesOnBoard[gameObject] = piecePos;
        BoardGenerator.Instance.PositionToPiece[piecePos] = gameObject;

        BoardState.Instance.UpdateThreatenedSquares();
        CalculateMoves.Instance.CalculateAllMoves();
    }
}