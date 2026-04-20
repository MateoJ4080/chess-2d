using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance { get; private set; }

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

    [ContextMenu("Test Ending Win")]
    void TestEndingGameWin()
    {
        GameManager.Instance.OnGameEnded(GameResult.Win, true);
    }

    [ContextMenu("Test Ending Lose")]
    void TestEndingGameLose()
    {
        GameManager.Instance.OnGameEnded(GameResult.Lose, false);
    }
}
