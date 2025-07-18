using Photon.Pun;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    [SerializeField] private HighlightMoves _highlightMoves;
    [SerializeField] private BoardManager _boardManager;
    private GameObject _selectedPiece;
    private PhotonView photonView;

    public static PieceManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    public void TryMovePiece(GameObject piece, Vector2Int from, Vector2Int to)
    {
        // Return piece to initial square if isn't legal move
        if (!IsLegalMove(to))
        {
            piece.transform.position = new(from.x, from.y, 0);
            return;
        }

        MovePiece(from, to, piece);

        int pieceID = piece.GetComponent<PhotonView>().ViewID;
        photonView.RPC("SynqSquare", RpcTarget.OthersBuffered, from.x, from.y, to.x, to.y, pieceID);
    }

    void MovePiece(Vector2Int from, Vector2Int to, GameObject piece)
    {
        piece.GetComponent<Draggable>().SnapToGrid();
        _highlightMoves.ClearHighlights();

        BoardUtils.RefreshBoardState(from, to, piece);
    }

    // Synchronize the square movement across the network, depending on the color/point of view of the local player
    [PunRPC]
    public void SynqSquare(int fromX, int fromY, int toX, int toY, int pieceID)
    {
        Vector2Int from = new(fromX, 7 - fromY);
        Vector2Int to = new(toX, 7 - toY);

        var view = PhotonView.Find(pieceID);
        if (view == null)
        {
            Debug.LogError("SynqSquare: PhotonView not found for ID " + pieceID);
            return;
        }

        // If there's a piece there, we remove it from the dictionary and destroy it
        if (!BoardUtils.SquareIsEmpty(to))
        {
            var target = BoardUtils.GetPieceAt(to);
            if (target != null)
            {
                BoardGenerator.Instance.PiecesOnBoard.Remove(target);
                Destroy(target);
            }
        }

        var piece = view.gameObject;

        piece.transform.position = new(to.x, to.y);
        BoardUtils.RefreshBoardState(from, to, piece);
    }

    // Check if this is a highlighted and legal square for the piece to move
    public bool IsLegalMove(Vector2Int targetPosition)
    {
        return _highlightMoves.LegalPositions.Contains(targetPosition);
    }

    // Select and Deselect
    public void SelectPiece(GameObject piece)
    {
        if (_selectedPiece == piece || !piece.CompareTag("Piece")) return;

        DeselectCurrentPiece();
        _selectedPiece = piece;
    }

    public void DeselectCurrentPiece()
    {
        if (_selectedPiece != null)
        {
            _selectedPiece = null;
        }
    }
}
