using UnityEngine;

public class DebugManager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);
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
