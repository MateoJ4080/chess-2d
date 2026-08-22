using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector3 _offset;
    private bool _isDragging = false;
    private Camera _cam;
    private Vector2Int _firstPosition;
    private SpriteRenderer _renderer;

    void Awake()
    {
        _cam = Camera.main;
        _renderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (!_isDragging) return;

        transform.position = GetMouseWorldPos() + _offset;

        Vector2Int squarePos = Vector2Int.RoundToInt(transform.position);
        HighlightMoves.Instance.HighlightSelectedSquare(squarePos);
    }

    void OnMouseDown()
    {
        _renderer.sortingOrder = 50;

        _isDragging = true;
        CursorManager.Instance.SetCursor(CursorType.Grab);

        _firstPosition = Vector2Int.RoundToInt(transform.position);
        _offset = transform.position - GetMouseWorldPos();

        HighlightMoves.Instance.ClearHighlights();
        HighlightMoves.Instance.ShowMoves(gameObject);
    }

    void OnMouseUp()
    {
        _renderer.sortingOrder = 5;

        _isDragging = false;

        Vector2Int newPosition = Vector2Int.RoundToInt(transform.position);
        PieceManager.Instance.TryMovePiece(gameObject, _firstPosition, newPosition);
        HighlightMoves.Instance.ClearSelectedSquare();

    }

    public void SnapToGrid()
    {
        transform.position = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePoint.z = 0;
        return mousePoint;
    }
}
