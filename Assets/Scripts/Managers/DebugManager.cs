using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance { get; private set; }
    public static bool IsDebugMode { get; private set; }

    [SerializeField] private GameObject _greenSquare;
    [SerializeField] private GameObject _redSquare;
    private Transform _highlightContainer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [ContextMenu("Test Ending - Win")]
    void TestEndingGameWin()
    {
        GameManager.Instance.TriggerGameOver(GameResult.Win, GameOverReason.Checkmate);
    }

    [ContextMenu("Test Ending - Lose")]
    void TestEndingGameLose()
    {
        GameManager.Instance.TriggerGameOver(GameResult.Lose, GameOverReason.Checkmate);
    }

    public void ToggleDebugMode()
    {
        Debug.Log($"IsDebugMode set to {!IsDebugMode}");
        IsDebugMode = !IsDebugMode;

        ColorThreatenedSquares();
    }

    // Debug
    public void ColorThreatenedSquares()
    {
        ClearColorSquares();
        if (!IsDebugMode) return;

        if (Instance._highlightContainer == null)
        {
            var obj = GameObject.FindGameObjectWithTag("HighlightsContainer");
            if (obj != null) Instance._highlightContainer = obj.transform;
        }

        if (_highlightContainer == null)
        {
            Debug.LogWarning("HighlightContainer GameObject is missing");
            return;
        }

        foreach (var entry in BoardState.Instance.WhiteThreatenedSquares)
        {
            Vector2Int move = entry.Key;
            if (BoardUtils.GetSquareAt(move))
            {
                GameObject colorSquare = Instantiate(Instance._greenSquare, new Vector3(move.x, move.y, 0), Quaternion.identity, _highlightContainer);
                SpriteRenderer sr = colorSquare.GetComponent<SpriteRenderer>();
                sr.sortingOrder = 1;
            }
        }

        foreach (var entry in BoardState.Instance.BlackThreatenedSquares)
        {
            Vector2Int move = entry.Key;
            if (BoardUtils.GetSquareAt(move))
            {
                GameObject colorSquare = Instantiate(_redSquare, new Vector3(move.x, move.y, 0), Quaternion.identity, Instance._highlightContainer);
                SpriteRenderer sr = colorSquare.GetComponent<SpriteRenderer>();
                sr.sortingOrder = 1;
            }
        }
    }

    // Debug
    public void ClearColorSquares()
    {
        foreach (var obj in GameObject.FindGameObjectsWithTag("ColorSquare"))
        {
            Destroy(obj);
        }
    }
}
