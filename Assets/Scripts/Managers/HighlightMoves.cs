using System.Collections.Generic;
using UnityEngine;

public class HighlightMoves : MonoBehaviour
{
    private List<GameObject> activeHighlights = new();
    public List<GameObject> ActiveHighlights => activeHighlights;

    [SerializeField] private GameObject _highlightPrefab;
    [SerializeField] private GameObject _highlightCapturePrefab;

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

    // Overload to match Action<GameObject> delegate
    public void ShowMoves(GameObject pieceGO)
    {
        Debug.Log("ShowMoves");

        if (pieceGO == null) return;

        if (BoardUtils.PlayerIsThisColor(pieceGO) && GameManager.Instance.ItsMyTurn())
            ShowPieceLegalMoves(pieceGO);
    }

    void ShowPieceLegalMoves(GameObject pieceGO)
    {
        if (CalculateMoves.Instance.LegalMovesByPiece.TryGetValue(pieceGO, out var pieceMoves))
        {
            Debug.Log($"ShowPieceLegalMoves: Piece found in hashmap");

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
}
