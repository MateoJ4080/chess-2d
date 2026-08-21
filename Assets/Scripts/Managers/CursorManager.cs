using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [SerializeField] private Texture2D _defaultCursor;
    [SerializeField] private Texture2D _handCursor;
    [SerializeField] private Vector2 _defaultHotspot;
    [SerializeField] private Vector2 _handHotspot;

    private Camera _cam;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _cam = Camera.main;
        SetCursor(CursorType.Default);
    }

    private void Update()
    {
        if (IsHoveringButton() || IsHoveringPiece())
            SetCursor(CursorType.Hand);
        else
            SetCursor(CursorType.Default);
    }

    private bool IsHoveringButton()
    {
        PointerEventData pointerData = new(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.TryGetComponent<Button>(out _))
                return true;
        }

        return false;
    }

    private bool IsHoveringPiece()
    {
        Vector2 mousePosition = _cam.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePosition);

        return hit != null && hit.CompareTag("Piece");
    }

    public void SetCursor(CursorType type)
    {
        switch (type)
        {
            case CursorType.Hand:
                Cursor.SetCursor(_handCursor, _handHotspot, CursorMode.Auto);
                break;

            default:
                Cursor.SetCursor(_defaultCursor, _defaultHotspot, CursorMode.Auto);
                break;
        }
    }
}

public enum CursorType
{
    Default,
    Hand
}