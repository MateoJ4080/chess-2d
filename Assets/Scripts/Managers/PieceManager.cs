using Photon.Pun;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    private PhotonView _photonView;

    public static PieceManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _photonView = GetComponent<PhotonView>();
    }

    public void TryMovePiece(GameObject pieceGO, Vector2Int from, Vector2Int to)
    {
        // If isn't legal move or isn't player's turn, return piece to original position
        if (!IsLegalMove(pieceGO, to) || !GameManager.Instance.ItsMyTurn() || !BoardUtils.PlayerIsThisColor(pieceGO))
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
        bool isWhite = data.IsWhite;


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

                _photonView.RPC("SyncMove", RpcTarget.OthersBuffered, 7, 0, 5, 0, pieceID, isWhite);

            }

            // Castle left
            if (from.x - to.x == 2)
            {
                Vector2Int leftRookPos = new(0, 0);
                GameObject leftRook = BoardUtils.GetPieceAt(leftRookPos);
                leftRook.transform.position = new(3, 0, 0);
                MovePiece(leftRookPos, new(3, 0), leftRook);

                _photonView.RPC("SyncMove", RpcTarget.OthersBuffered, 0, 0, 3, 0, pieceID, isWhite);
            }
        }

        HighlightMoves.Instance.ClearHighlights();
        MovePiece(from, to, pieceGO);
        TimeManager.Instance.OnPieceMovedBySelf();
        GameManager.Instance.OnPieceMovedBySelf(pieceGO, from);
        GameManager.Instance.SwitchTurn();

        _photonView.RPC("SyncMove", RpcTarget.OthersBuffered, from.x, from.y, to.x, to.y, pieceID, isWhite);
    }

    void MovePiece(Vector2Int from, Vector2Int to, GameObject piece)
    {
        piece.GetComponent<Draggable>().SnapToGrid();
        BoardUtils.RefreshBoardState(from, to, piece);
    }

    // Synchronize a piece move across the network, depending on the color/point of view of the local player
    [PunRPC]
    public void SyncMove(int fromX, int fromY, int toX, int toY, int pieceID, bool isMoveFromWhite)
    {
        Vector2Int from = TransformPos(new(fromX, fromY), isMoveFromWhite);
        Vector2Int to = TransformPos(new(toX, toY), isMoveFromWhite);

        var view = PhotonView.Find(pieceID);
        if (view == null)
        {
            Debug.LogError("[PunRPC] SyncMove: PhotonView not found for ID " + pieceID);
            return;
        }

        // If piece on target square, destroy and remove from dictionary
        GameObject pieceToCapture = BoardUtils.GetPieceAt(to);
        if (pieceToCapture != null)
        {
            BoardGenerator.Instance.PiecesOnBoard.Remove(pieceToCapture);
            Destroy(pieceToCapture);
        }

        var piece = view.gameObject;
        piece.transform.position = new(to.x, to.y);
        BoardUtils.RefreshBoardState(from, to, piece);
    }

    // Check if this is a highlighted and legal square for the piece to move
    public bool IsLegalMove(GameObject pieceGO, Vector2Int targetPosition)
    {
        if (CalculateMoves.Instance.LegalMovesByPiece.TryGetValue(pieceGO, out var legalMoves))
            return legalMoves.Contains(targetPosition);

        return false;
    }

    private Vector2Int TransformPos(Vector2Int pos, bool moveFromWhite)
    {
        return BoardState.Instance.IsBoardInverted == moveFromWhite ? new Vector2Int(pos.x, 7 - pos.y) : pos;
    }
}
