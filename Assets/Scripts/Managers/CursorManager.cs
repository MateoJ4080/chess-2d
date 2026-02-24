using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private Texture2D _defaultCursor;
    [SerializeField] private Texture2D _handCursor;

    [SerializeField] private Vector2 _hotspot;
    private Texture2D _currentCursor;

    void Start()
    {
        SetCursor(_defaultCursor);
    }

    void Update()
    {
        Vector3 mouse = Input.mousePosition;

        if (mouse.x < 0 || mouse.y < 0 ||
            mouse.x > Screen.width || mouse.y > Screen.height)
            return;

        Vector2 mousePos = _cam.ScreenToWorldPoint(mouse);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        if (hit != null && hit.CompareTag("Piece"))
            SetCursor(_handCursor);
        else
            SetCursor(_defaultCursor);
    }

    private void SetCursor(Texture2D cursor)
    {
        if (_currentCursor == cursor) return;
        Cursor.SetCursor(cursor, _hotspot, CursorMode.Auto);
        _currentCursor = cursor;
    }
}
