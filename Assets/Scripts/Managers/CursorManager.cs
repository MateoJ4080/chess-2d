using UnityEngine;

public class CursorManager : MonoBehaviour
{
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
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        if (hit != null && hit.CompareTag("Piece"))
        {
            SetCursor(_handCursor);
        }
        else
        {
            SetCursor(_defaultCursor);
        }
    }

    private void SetCursor(Texture2D cursor)
    {
        if (_currentCursor == cursor) return;
        Cursor.SetCursor(cursor, _hotspot, CursorMode.Auto);
        _currentCursor = cursor;
    }
}
