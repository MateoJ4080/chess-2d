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

    public void TryMovePiece(GameObject piece, Vector2Int from, Vector2Int to)
    {
        var data = piece.GetComponent<ChessPiece>().PieceData;
        int pieceID = piece.GetComponent<PhotonView>().ViewID;
        bool isWhite = data.Color == PlayerColor.White;

        // If illegal move, return to original position
        if (!CanMovePiece(piece, to))
        {
            if (GameManager.Instance.IsMyTurn() && from != to)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxIllegal);
            piece.transform.position = new(from.x, from.y, 0);
            return;
        }

        GameObject target = BoardUtils.GetPieceAt(to);
        CapturePiece(target);

        bool isCastling = data.PieceType == "King" && Mathf.Abs(to.x - from.x) == 2;
        if (isCastling) HandleCastling(from, to, isWhite);

        // Important: must go before MovePiece so EnPeassant is registered before CalculateAllMoves
        GameManager.Instance.OnPieceMovedBySelf(piece, from, to, target);

        MovePiece(from, to, piece);
        _photonView.RPC("SyncMove", RpcTarget.OthersBuffered, from.x, from.y, to.x, to.y, pieceID, isWhite);
    }

    void MovePiece(Vector2Int from, Vector2Int to, GameObject piece)
    {
        piece.transform.position = new(to.x, to.y);
        BoardUtils.RefreshBoardState(from, to, piece);
    }

    public static void CapturePiece(GameObject piece)
    {
        if (piece == null) return;

        BoardGenerator.Instance.PiecesOnBoard.Remove(piece);
        Destroy(piece);
    }

    [PunRPC]
    public void SyncMove(int fromX, int fromY, int toX, int toY, int pieceID, bool isMoveFromWhite)
    {
        Vector2Int from = InvertPos(new(fromX, fromY), isMoveFromWhite);
        Vector2Int to = InvertPos(new(toX, toY), isMoveFromWhite);

        var view = PhotonView.Find(pieceID);
        if (view == null)
        {
            Debug.LogError("[PunRPC] SyncMove: PhotonView not found for ID " + pieceID);
            return;
        }

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

    private void HandleCastling(Vector2Int from, Vector2Int to, bool isWhite)
    {
        bool kingSide = to.x > from.x;
        Vector2Int rookFrom = new(kingSide ? 7 : 0, from.y);
        Vector2Int rookTo = new(kingSide ? 5 : 3, from.y);

        GameObject rook = BoardUtils.GetPieceAt(rookFrom);
        int rookID = rook.GetComponent<PhotonView>().ViewID;

        MovePiece(rookFrom, rookTo, rook);
        _photonView.RPC("SyncMove", RpcTarget.OthersBuffered, rookFrom.x, rookFrom.y, rookTo.x, rookTo.y, rookID, isWhite);
    }

    private bool CanMovePiece(GameObject piece, Vector2Int to)
    {
        return GameManager.Instance.IsMyTurn() &&
               BoardUtils.PlayerIsThisColor(piece) &&
               IsLegalMove(piece, to);
    }

    // Check if this is a highlighted and legal square for the piece to move
    public bool IsLegalMove(GameObject pieceGO, Vector2Int targetPosition)
    {
        if (CalculateMoves.Instance.LegalMovesByPiece.TryGetValue(pieceGO, out var legalMoves))
            return legalMoves.Contains(targetPosition);

        return false;
    }

    private Vector2Int InvertPos(Vector2Int pos, bool moveFromWhite)
    {
        return BoardState.Instance.IsBoardInverted == moveFromWhite ? new Vector2Int(pos.x, 7 - pos.y) : pos;
    }
}
