using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _cam;

    private float _initialSize;
    private ScreenOrientation _lastOrientation;

    void Awake()
    {
        _cam = GetComponent<Camera>();
        _initialSize = _cam.orthographicSize;
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

        float sideOffsetPercent = 0.1f; // 10%
        finalSize *= 1f + sideOffsetPercent;

        Debug.Log($"AdjustForOrientation → boardWidth: {boardWidth}, boardHeight: {boardHeight}, aspect: {screenAspect}, finalSize: {finalSize}");

        _cam.orthographicSize = finalSize;
    }
}

