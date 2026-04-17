using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public float boardHeightPercentage = 0.85f; // "Height" percentage because it's only meant to make top and bottom panels fit in wide resolutions. Not necessary in tall resolutions as the panels will easily fit
    private Camera _cam;
    private ScreenOrientation _lastOrientation;
    public float finalOrthographicSize;

    public static CameraController Instance;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _cam = GetComponent<Camera>();
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
    public void AdjustForOrientation()
    {
        if (BoardGenerator.Instance == null) return;

        float scaleX = BoardGenerator.Instance.transform.lossyScale.x;
        float scaleY = BoardGenerator.Instance.transform.lossyScale.y;
        float boardWidth = BoardGenerator.Instance.TileSize.x * 8f * scaleX;
        float boardHeight = BoardGenerator.Instance.TileSize.y * 8f * scaleY;

        float screenAspect = (float)Screen.width / Screen.height;

        float sizeByHeight = boardHeight / 2f / boardHeightPercentage;
        float sizeByWidth = boardWidth / (2f * screenAspect);

        finalOrthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);

        // float sideOffset = 0.1f; // 10%
        // finalSize *= 1f + sideOffset;

        _cam.orthographicSize = finalOrthographicSize;
    }
}

