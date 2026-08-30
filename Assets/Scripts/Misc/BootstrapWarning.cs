using UnityEngine;
using UnityEngine.SceneManagement;

public static class BootstrapWarning
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CheckEntryPoint()
    {
        if (SceneManager.GetActiveScene().name != "Bootstrap")
            Debug.LogWarning("Game started without Bootstrap. Start from the Bootstrap scene for proper initialization.");
    }
}