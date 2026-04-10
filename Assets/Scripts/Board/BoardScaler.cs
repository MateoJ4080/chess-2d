using System.Collections;
using UnityEngine;

public class BoardScaler : MonoBehaviour
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
        ScalePanelsAndBoard();
    }

    void ScalePanelsAndBoard()
    {
        if (_boardContainer != null)
        {
            float boardPercentage = CameraController.Instance.boardHeightPercentage;
            float desiredHeightTop = ((RectTransform)_topPanel.parent).rect.height * (1 - boardPercentage) / 2;
            float desiredHeightBottom = ((RectTransform)_bottomPanel.parent).rect.height * (1 - boardPercentage) / 2;

            _topPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, desiredHeightTop);
            _bottomPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, desiredHeightBottom);
            _topPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ((RectTransform)_topPanel.parent).rect.height * boardPercentage);
            _bottomPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ((RectTransform)_bottomPanel.parent).rect.height * boardPercentage);

            // Debug
            Debug.Log($"<color=yellow>TopPanelHeight: {_topPanel.rect.height}");
            Debug.Log($"<color=yellow>BottomPanelHeight: {_bottomPanel.rect.height}");
            Debug.Log($"<color=yellow>ParentHeight: {((RectTransform)_topPanel.parent).rect.height}");
        }
        else Debug.LogError("_boardContainer not assigned");
    }
}
