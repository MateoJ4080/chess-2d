using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject loadingMenu;
    [SerializeField] private GameObject NetworkMenu;
    [SerializeField] private TextMeshProUGUI NetworkStatusText;

    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public static void ShowMainMenu()
    {
        Instance.loadingMenu.SetActive(false);
        Instance.mainMenu.SetActive(true);
    }

    public static void ShowLoadingMenu()
    {
        Instance.mainMenu.SetActive(false);
        Instance.loadingMenu.SetActive(true);
    }

    void HideMenus()
    {
        Instance.mainMenu.SetActive(false);
        Instance.loadingMenu.SetActive(false);
    }

    public static void ChangeNetworkText(string text)
    {
        Instance.NetworkStatusText.text = text;
    }
}
