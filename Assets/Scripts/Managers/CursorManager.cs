using UnityEngine;
using System.Runtime.InteropServices;

public class CursorManager : MonoBehaviour
{
    private Camera _cam;

    [SerializeField] private Texture2D _defaultCursor;
    [SerializeField] private Texture2D _handCursor;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void SetCursorPointer();
    [DllImport("__Internal")] private static extern void SetCursorDefault();
#endif

    void Awake()
    {
        _cam = Camera.main;
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
        {
            Debug.Log("Hand");
            SetHand();
        }
        else
        {
            Debug.Log("Default");
            SetDefault();
        }
    }

    void SetHand()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SetCursorPointer();
#else
        Cursor.SetCursor(_handCursor, Vector2.zero, CursorMode.Auto);
#endif
    }

    void SetDefault()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SetCursorDefault();
#else
        Cursor.SetCursor(_defaultCursor, Vector2.zero, CursorMode.Auto);
#endif
    }
}