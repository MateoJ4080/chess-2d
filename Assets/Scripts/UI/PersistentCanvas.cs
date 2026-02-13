using UnityEngine;

public class PersistentCanvas : MonoBehaviour
{
    public static PersistentCanvas Instance;

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
}
