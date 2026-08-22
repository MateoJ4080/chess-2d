using System.Collections.Generic;
using UnityEngine;

public class HighlightMoves : MonoBehaviour
{
    private List<GameObject> activeHighlights = new();
    public List<GameObject> ActiveHighlights => activeHighlights;

    [SerializeField] private GameObject _highlightPrefab;
    [SerializeField] private GameObject _highlightCapturePrefab;
    [SerializeField] private GameObject _selectedSquareHighlight;

    public static HighlightMoves Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowMoves(GameObject pieceGO)
    {
        if (pieceGO == null) return;

        if (BoardUtils.PlayerIsThisColor(pieceGO) && GameManager.Instance.IsMyTurn())
            ShowPieceLegalMoves(pieceGO);
    }

    void ShowPieceLegalMoves(GameObject pieceGO)
    {
        if (CalculateMoves.Instance.LegalMovesByPiece.TryGetValue(pieceGO, out var pieceMoves))
        {
            foreach (var pos in pieceMoves)
            {
                ShowHighlight(pos);
            }
        }
    }

    void ShowHighlight(Vector2Int pos)
    {
        GameObject piece = BoardUtils.GetPieceAt(pos);
        GameObject prefab = piece == null ? _highlightPrefab : _highlightCapturePrefab;

        GameObject highlight = Instantiate(prefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        activeHighlights.Add(highlight);
    }

    public void ClearHighlights()
    {
        foreach (var highlight in activeHighlights)
        {
            if (highlight != null)
                Destroy(highlight);
        }
        activeHighlights.Clear();
    }

    public void HighlightSelectedSquare(Vector2Int position)
    {
        _selectedSquareHighlight.transform.position = (Vector3Int)position;
        _selectedSquareHighlight.SetActive(true);

    }

    public void ClearSelectedSquare()
    {
        _selectedSquareHighlight.SetActive(false);
    }
}
