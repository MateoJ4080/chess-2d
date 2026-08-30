using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    public static bool Initialized { get; private set; }

    private void Awake()
    {
        Initialized = true;
        SceneManager.LoadScene("Menu");
    }
}