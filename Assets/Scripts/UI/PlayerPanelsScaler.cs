using UnityEngine;

public class PlayerPanelsScaler : MonoBehaviour
{
    [SerializeField] private GameObject _boardContainer;
    private RectTransform _topPanel;
    private RectTransform _bottomPanel;

    void Awake()
    {
        UIManager.Instance.ShowPlayerPanelsParent();
        _topPanel = GameObject.FindGameObjectWithTag("TopPanel").GetComponent<RectTransform>();
        _bottomPanel = GameObject.FindGameObjectWithTag("BottomPanel").GetComponent<RectTransform>();
    }

    void Start()
    {
        CameraController.Instance.AdjustForOrientation();
    }

    void Update()
    {
        ScalePanels();
    }

    void ScalePanels()
    {
        if (_boardContainer != null)
        {
            float boardPercentage = CameraController.Instance.finalOrthographicSize == 4 ? 1 : CameraController.Instance.boardHeightPercentage;
            float boardHeightInScreen = Screen.width > Screen.height ? ((RectTransform)_topPanel.parent).rect.height * boardPercentage : ((RectTransform)_topPanel.parent).rect.width;

            // Position
            float topPanelHeight = _topPanel.rect.height;
            float bottomPanelHeight = _bottomPanel.rect.height;
            float topY = ((RectTransform)_topPanel.parent).anchoredPosition.y + (boardHeightInScreen / 2) + (topPanelHeight / 2);
            float bottomY = ((RectTransform)_bottomPanel.parent).anchoredPosition.y - (boardHeightInScreen / 2) - (bottomPanelHeight / 2);
            _topPanel.anchoredPosition = new(0, topY);
            _bottomPanel.anchoredPosition = new(0, bottomY);

            // Height
            float desiredHeightTop = ((RectTransform)_topPanel.parent).rect.height * (1 - boardPercentage) / 2;
            float desiredHeightBottom = ((RectTransform)_bottomPanel.parent).rect.height * (1 - boardPercentage) / 2;
            _topPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, desiredHeightTop);
            _bottomPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, desiredHeightBottom);

            // Width
            float desiredWidth = Screen.width > Screen.height ? ((RectTransform)_topPanel.parent).rect.height * boardPercentage : ((RectTransform)_topPanel.parent).rect.width;
            _topPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, desiredWidth);
            _bottomPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, desiredWidth);
        }
        else Debug.LogError("_boardContainer not assigned");
    }
}
