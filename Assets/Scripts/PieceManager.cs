using System;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    public static PieceManager Instance { get; private set; }

    public static event Action<GameObject> OnPieceSelected;
    public static event Action<GameObject> OnPieceDeselected;
    public static event Action<GameObject, Vector2Int, Vector2Int> OnPieceMoved;

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

    public void SelectPiece(GameObject piece)
    {
        Debug.Log("SelectPiece");
        if (_selectedPiece == piece || !piece.CompareTag("Piece")) return;

        DeselectCurrentPiece();
        _selectedPiece = piece;
        OnPieceSelected?.Invoke(piece);
    }

    public void DeselectCurrentPiece()
    {
        if (_selectedPiece != null)
        {
            OnPieceDeselected?.Invoke(_selectedPiece);
            _selectedPiece = null;
        }
    }

    public static void InvokeOnPieceSelected(GameObject piece)
    {
        OnPieceSelected?.Invoke(piece);
    }

    public static void InvokeOnPieceMoved(GameObject piece, Vector2Int oldPosition, Vector2Int newPosition)
    {
        OnPieceMoved?.Invoke(piece, oldPosition, newPosition);
    }

    public bool IsLegalMove(Vector2Int targetPosition)
    {
        // Mueve la pieza
        return _moveHighlighter.LegalPositions.Contains(targetPosition);
    }
}
