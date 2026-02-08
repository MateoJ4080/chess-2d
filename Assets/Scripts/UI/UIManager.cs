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

    void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        Instance.loadingMenu.SetActive(false);
        Instance.mainMenu.SetActive(true);
    }

    public void ShowLoadingMenu()
    {
        Instance.mainMenu.SetActive(false);
        Instance.loadingMenu.SetActive(true);
    }

    public void ChangeNetworkText(string text)
    {
        Instance.NetworkStatusText.text = text;
    }
}
