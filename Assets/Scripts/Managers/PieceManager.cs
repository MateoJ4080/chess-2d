using Photon.Pun;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
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

    public void TryMovePiece(GameObject pieceGO, Vector2Int from, Vector2Int to)
    {
        // If isn't legal move or isn't player's turn, return piece to original position
        if (!IsLegalMove(pieceGO, to) || !GameManager.Instance.ItsMyTurn())
        {
            pieceGO.transform.position = new(from.x, from.y, 0);
            return;
        }

        // If there's a piece on target square, destroy and remove from dictionary
        GameObject pieceToCapture = BoardUtils.GetPieceAt(to);
        if (pieceToCapture != null)
        {
            Destroy(pieceToCapture);
            BoardGenerator.Instance.PiecesOnBoard.Remove(pieceToCapture);
        }

        var data = pieceGO.GetComponent<ChessPiece>().PieceData;
        int pieceID = pieceGO.GetComponent<PhotonView>().ViewID;


        // Castling
        if (data.PieceType == "King")
        {
            // Castle right
            if (to.x - from.x == 2)
            {
                Vector2Int rightRookPos = new(7, 0);
                GameObject rightRook = BoardUtils.GetPieceAt(rightRookPos);
                rightRook.transform.position = new(5, 0, 0);
                MovePiece(rightRookPos, new(5, 0), rightRook);

                photonView.RPC("SyncMove", RpcTarget.OthersBuffered, 7, 0, 5, 0, pieceID);

            }

            // Castle left
            if (from.x - to.x == 2)
            {
                Vector2Int leftRookPos = new(0, 0);
                GameObject leftRook = BoardUtils.GetPieceAt(leftRookPos);
                leftRook.transform.position = new(3, 0, 0);
                MovePiece(leftRookPos, new(3, 0), leftRook);

                photonView.RPC("SyncMove", RpcTarget.OthersBuffered, 0, 0, 3, 0, pieceID);

            }
        }

        HighlightMoves.Instance.ClearHighlights();
        MovePiece(from, to, pieceGO);
        GameManager.Instance.OnPieceMoved(pieceGO, from);
        GameManager.Instance.SwitchTurn();

        photonView.RPC("SyncMove", RpcTarget.OthersBuffered, from.x, from.y, to.x, to.y, pieceID);
    }

    void MovePiece(Vector2Int from, Vector2Int to, GameObject piece)
    {
        piece.GetComponent<Draggable>().SnapToGrid();
        BoardUtils.RefreshBoardState(from, to, piece);
    }

    // Synchronize the square movement across the network, depending on the color/point of view of the local player
    [PunRPC]
    public void SyncMove(int fromX, int fromY, int toX, int toY, int pieceID)
    {
        // The conversion depending on the color of the player is still to be done
        // For now, we assume the board will be inverted
        Vector2Int from = new(fromX, 7 - fromY);
        Vector2Int to = new(toX, 7 - toY);

        var view = PhotonView.Find(pieceID);
        if (view == null)
        {
            Debug.LogError("SynqSquare: PhotonView not found for ID " + pieceID);
            return;
        }

        // If piece on target square, destroy and remove from dictionary
        GameObject pieceToCapture = BoardUtils.GetPieceAt(to);
        if (pieceToCapture != null)
        {
            Destroy(pieceToCapture);
            BoardGenerator.Instance.PiecesOnBoard.Remove(pieceToCapture);
        }

        var piece = view.gameObject;
        piece.transform.position = new(to.x, to.y);
        BoardUtils.RefreshBoardState(from, to, piece);
    }

    // Check if this is a highlighted and legal square for the piece to move
    public bool IsLegalMove(GameObject pieceGO, Vector2Int targetPosition)
    {
        if (CalculateMoves.Instance.LegalMovesByPiece.TryGetValue(pieceGO, out var pieceMoves))
            return pieceMoves.Contains(targetPosition);

        return false;
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
