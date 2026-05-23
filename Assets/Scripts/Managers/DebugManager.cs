using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance { get; private set; }
    public static bool IsDebugMode { get; private set; }

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
    }
}
