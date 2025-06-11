using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector3 _offset;
    private bool _isDragging = false;
    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
    }

    void OnMouseDown()
    {
        _offset = transform.position - GetMouseWorldPos();
        _isDragging = true;
    }

    void OnMouseUp()
    {
        _isDragging = false;
        SnapToGrid();
    }

    void Update()
    {
        if (_isDragging)
            transform.position = GetMouseWorldPos() + _offset;
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePoint.z = 0;
        return mousePoint;
    }

    void SnapToGrid()
    {
        transform.position = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
    }
}
