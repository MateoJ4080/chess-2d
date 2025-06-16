using Unity.Mathematics;
using Unity.VisualScripting;
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

    void OnEnable()
    {
        PieceManager.OnPieceMoved += SnapToGrid;
    }

    void OnMouseDown()
    {
        _firstPosition = transform.position;
        _offset = transform.position - GetMouseWorldPos();
        _isDragging = true;

        _moveHighlighter.ClearHighlights();
        PieceManager.InvokeOnPieceSelected(gameObject);
    }

    void OnMouseUp()
    {
        _isDragging = false;
        BoardGenerator.Instance.PiecesOnBoard[gameObject] = Vector2Int.RoundToInt(transform.position);

        if (transform.position != _firstPosition && PieceManager.Instance.IsLegalMove(Vector2Int.RoundToInt(transform.position)))
        {
            PieceManager.InvokeOnPieceMoved(gameObject, Vector2Int.RoundToInt(_firstPosition), Vector2Int.RoundToInt(transform.position));
        }
        else
        {
            transform.position = _firstPosition;
        }
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

    // Handler matching Action<GameObject, Vector2Int, Vector2Int>
    void SnapToGrid(GameObject piece, Vector2Int from, Vector2Int to)
    {
        SnapToGrid();
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePoint.z = 0;
        return mousePoint;
    }
}
