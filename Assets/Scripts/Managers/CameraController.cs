using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private Camera _cam;
    private ScreenOrientation _lastOrientation;

    void Awake()
    {
        DontDestroyOnLoad(this);

        _cam = GetComponent<Camera>();
    }

    void Start()
    {
        AdjustForOrientation();
    }

    void Update()
    {
        if (Screen.orientation != _lastOrientation)
        {
            AdjustForOrientation();
            _lastOrientation = Screen.orientation;
        }
    }

    // UI units to pixels: = UI units * scale factor
    void AdjustForOrientation()
    {
        if (BoardGenerator.Instance == null) return;

        float scaleX = BoardGenerator.Instance.transform.lossyScale.x;
        float scaleY = BoardGenerator.Instance.transform.lossyScale.y;

        float boardWidth = BoardGenerator.Instance.TileSize.x * 8f * scaleX;
        float boardHeight = BoardGenerator.Instance.TileSize.y * 8f * scaleY;

        float screenAspect = (float)Screen.width / Screen.height;

        float sizeByHeight = boardHeight / 2f;
        float sizeByWidth = boardWidth / (2f * screenAspect);

        float finalSize = Mathf.Max(sizeByHeight, sizeByWidth);

        // float sideOffsetPercent = 0.1f; // 10%
        // finalSize *= 1f + sideOffsetPercent;

        _cam.orthographicSize = sizeByHeight;
    }
}

