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
            float rawBoardPercentage = CameraController.Instance.boardHeightPercentage;

            float parentRectWidth = ((RectTransform)_topPanel.parent).rect.width;
            float parentRectHeight = ((RectTransform)_topPanel.parent).rect.height;

            // Position
            float boardLength = Screen.width > Screen.height ? parentRectHeight * rawBoardPercentage : parentRectWidth;
            float topPanelHeight = _topPanel.rect.height;
            float bottomPanelHeight = _bottomPanel.rect.height;
            float topY = ((RectTransform)_topPanel.parent).anchoredPosition.y + (boardLength / 2) + (topPanelHeight / 2);
            float bottomY = ((RectTransform)_bottomPanel.parent).anchoredPosition.y - (boardLength / 2) - (bottomPanelHeight / 2);
            _topPanel.anchoredPosition = new(0, topY);
            _bottomPanel.anchoredPosition = new(0, bottomY);

            // Height
            float desiredHeight = Screen.width > Screen.height ? parentRectHeight * (1 - rawBoardPercentage) / 2 : parentRectWidth * (1 - rawBoardPercentage) / 2;
            _topPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, desiredHeight);
            _bottomPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, desiredHeight);

            // Width
            float desiredWidth = Screen.width > Screen.height ? parentRectHeight * rawBoardPercentage : parentRectWidth;
            _topPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, desiredWidth);
            _bottomPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, desiredWidth);
        }
        else Debug.LogError("_boardContainer not assigned");
    }
}
