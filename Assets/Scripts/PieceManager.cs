using System;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    public static PieceManager Instance { get; private set; }

    [SerializeField] private MoveHighlighter _moveHighlighter;
    private GameObject _selectedPiece;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void TryMovePiece(GameObject piece, Vector2Int from, Vector2Int to)
    {
        // Return piece to initial square if isn't legal move
        if (!IsLegalMove(to))
        {
            piece.transform.position = new(from.x, from.y, 0);
            return;
        }

        if (!BoardUtils.SquareIsEmpty(to) || BoardUtils.CanCaptureAt(to, piece))
        {
            var target = BoardUtils.GetPieceAt(to);
            Debug.Log("Trying to destroy " + target);
            if (target) Destroy(target);

            BoardGenerator.Instance.PiecesOnBoard[piece] = to;
            BoardGenerator.Instance.PositionToPiece[to] = piece;
        }

        piece.GetComponent<Draggable>().SnapToGrid();
        _moveHighlighter.ClearHighlights();
    }

    // Check if this is a highlighted and legal square for the piece to move
    public bool IsLegalMove(Vector2Int targetPosition)
    {
        return _moveHighlighter.LegalPositions.Contains(targetPosition);
    }

    // Select and Deselect
    public void SelectPiece(GameObject piece)
    {
        Debug.Log("SelectPiece");
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
