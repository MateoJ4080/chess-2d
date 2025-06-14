using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector3 _offset;
    private bool _isDragging = false;
    private Camera _cam;

    [SerializeField] private MoveHighlighter _moveHighlighter;

    private Vector3 _firstPosition;

    void Start()
    {
        _cam = Camera.main;
        if (_moveHighlighter == null)
            _moveHighlighter = FindAnyObjectByType<MoveHighlighter>();
    }

    void OnMouseDown()
    {
        _firstPosition = transform.position;
        _offset = transform.position - GetMouseWorldPos();
        _isDragging = true;

        PieceManager.InvokeOnPieceSelected(gameObject);
        if (gameObject.tag == "Piece") PieceManager.Instance.SelectPiece(gameObject);
    }

    void OnMouseUp()
    {
        _isDragging = false;
        BoardGenerator.Instance.PiecesOnBoard[gameObject] = Vector2Int.RoundToInt(transform.position);

        if (transform.position != _firstPosition) PieceManager.InvokeOnPieceMoved(gameObject, Vector2Int.RoundToInt(_firstPosition), Vector2Int.RoundToInt(transform.position));
        SnapToGrid();
    }

    void Update()
    {
        if (_isDragging)
            transform.position = GetMouseWorldPos() + _offset;
    }


    void SnapToGrid()
    {
        transform.position = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePoint.z = 0;
        return mousePoint;
    }

    void ConfirmLegalMove()
    {

    }
}
