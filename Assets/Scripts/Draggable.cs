using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector3 _offset;
    private bool _isDragging = false;
    private Camera _cam;

    [SerializeField] private HighlightMoves _highlightMoves;

    private Vector2Int _firstPosition;

    void Start()
    {
        _cam = Camera.main;
        if (_highlightMoves == null)
            _highlightMoves = FindAnyObjectByType<HighlightMoves>();
    }

    void Update()
    {
        if (_isDragging)
            transform.position = GetMouseWorldPos() + _offset;
    }

    void OnMouseDown()
    {
        _isDragging = true;
        _firstPosition = Vector2Int.RoundToInt(transform.position);
        _offset = transform.position - GetMouseWorldPos();

        _highlightMoves.ClearHighlights();
        _highlightMoves.ShowMoves(gameObject);
    }

    void OnMouseUp()
    {
        _isDragging = false;

        Vector2Int newPosition = Vector2Int.RoundToInt(transform.position);
        PieceManager.Instance.TryMovePiece(gameObject, _firstPosition, newPosition);
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
