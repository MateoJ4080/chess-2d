using System.Collections;
using UnityEngine;

public class BoardScaler : MonoBehaviour
{
    [SerializeField] private float boardPercentageOfScreen = 0.85f;
    [SerializeField] private GameObject _boardContainer;
    private RectTransform _topPanel;
    private RectTransform _bottomPanel;

    void Awake()
    {
        DontDestroyOnLoad(this);
        UIManager.Instance.ShowPlayerPanelsParent();
        _topPanel = GameObject.FindGameObjectWithTag("TopPanel").GetComponent<RectTransform>();
        _bottomPanel = GameObject.FindGameObjectWithTag("BottomPanel").GetComponent<RectTransform>();
        Camera.main.orthographicSize = Camera.main.orthographicSize / boardPercentageOfScreen;
    }

    void Update()
    {
        // _boardContainer.transform.localScale = new(boardPercentageOfScreen, boardPercentageOfScreen);

        if (_boardContainer != null)
        {
            float desiredHeightTop = ((RectTransform)_topPanel.parent).rect.height * (1 - boardPercentageOfScreen) / 2;
            float desiredHeightBottom = ((RectTransform)_bottomPanel.parent).rect.height * (1 - boardPercentageOfScreen) / 2;
            _topPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, desiredHeightTop);
            _bottomPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, desiredHeightBottom);

            _topPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ((RectTransform)_topPanel.parent).rect.height * 0.85f);
            _bottomPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ((RectTransform)_bottomPanel.parent).rect.height * 0.85f);

            var percentage = _topPanel.rect.height / ((RectTransform)_topPanel.parent).rect.height;
            Debug.Log($"<color=yellow>Percentage : {percentage}");
            Debug.Log($"<color=yellow>TopPanelHeight: {_topPanel.rect.height}");
            Debug.Log($"<color=yellow>BottomPanelHeight: {_bottomPanel.rect.height}");
            Debug.Log($"<color=yellow>ParentHeight: {((RectTransform)_topPanel.parent).rect.height}");
        }
        else Debug.LogError("_boardContainer not assigned");
    }
}
