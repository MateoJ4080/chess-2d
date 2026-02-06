using UnityEngine;
using Photon.Pun;
using TMPro;

public class PieceSetup : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    private BoardManager _boardManager;
    private Transform _piecesContainer;

    private readonly float tilePadding = 0.4f;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _boardManager = FindFirstObjectByType<BoardManager>();
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Color", out object colorObj))
        {
            string color = colorObj.ToString();
            if (color == "Black")
                _boardManager.BoardIsInverted = true;

            TextMeshProUGUI colorInfoText = GameObject.FindGameObjectWithTag("ColorInfoText").GetComponent<TextMeshProUGUI>();
            colorInfoText.text = "Color: " + color;
        }
        else Debug.LogError("PlayerManager: Player color not set!");

        PieceDataManager.Initialize();
        SetupPiece();
    }

    void SetupPiece()
    {
        Debug.Log($"BoardIsInverted: {_boardManager.BoardIsInverted}");

        if (_boardManager == null) _boardManager = FindFirstObjectByType<BoardManager>();
        if (_piecesContainer == null) _piecesContainer = GameObject.FindGameObjectWithTag("PiecesContainer").transform; string pieceDataName = (string)photonView.InstantiationData[0];
        if (pieceDataName == null) Debug.Log("pieceDataName is null");

        PieceData pieceData = PieceDataManager.Instance.GetPieceDataByName(pieceDataName);
        Vector2 tileSize = BoardGenerator.Instance.TileSize;

        Transform visual = transform.Find("Visual");
        if (visual == null)
        {
            Debug.LogError("Visual child not found on piece prefab!");
            return;
        }

        SpriteRenderer sr = visual.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("SpriteRenderer not found on Visual child!");
            return;
        }

        // Scale sprite to have the same size of the tile independently of its size, and then substract _tilePadding to that
        Vector2 pieceSize = sr.sprite.bounds.size;
        float scaleX = tileSize.x / pieceSize.x;
        float scaleY = tileSize.y / pieceSize.y;
        float uniformScale = Mathf.Min(scaleX, scaleY);
        visual.localScale = Vector2.one * (uniformScale - tilePadding);

        // Add ChessPiece component and assign the PieceData. This is to use "PieceType" string and set the ActivePiece in MoveHighlighter
        var chessPiece = gameObject.AddComponent<ChessPiece>();
        chessPiece.PieceData = pieceData;

        // Set parent and sorting order
        transform.SetParent(_piecesContainer, false);
        sr.sortingOrder = 4;

        // Allow mouse dragging on piece
        gameObject.AddComponent<Draggable>();

        // Position and direction
        int x = (int)photonView.InstantiationData[1];
        int y = (int)photonView.InstantiationData[2];
        Vector2Int pos = new(x, y);

        int posY = _boardManager.BoardIsInverted ? 7 - pos.y : pos.y;

        Vector2Int piecePos = new(pos.x, posY);
        gameObject.transform.localPosition = new Vector3(x, posY, 0f);

        // Set the collider to match the tile size (unaffected by visual scaling)
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = tileSize;
        collider.offset = Vector2.zero;

        BoardGenerator.Instance.PiecesOnBoard[gameObject] = piecePos;
        BoardGenerator.Instance.PositionToPiece[piecePos] = gameObject;
        BoardState.UpdateThreatenedSquares();


        Debug.Log($"<color=yellow> {pieceData.PieceName} added to ({piecePos.x}, {piecePos.y}). PositionToPiece count: {BoardGenerator.Instance.PositionToPiece.Count}");
    }
}