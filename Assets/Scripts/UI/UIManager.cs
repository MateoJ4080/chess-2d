using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _loadingMenu;
    [SerializeField] private TextMeshProUGUI _networkStatusText;

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
        _loadingMenu.SetActive(false);
        _mainMenu.SetActive(true);
    }

    public void ShowLoadingMenu()
    {
        _loadingMenu.SetActive(true);
    }

    public void ChangeNetworkText(string text)
    {
        _networkStatusText.text = text;
    }
    }
}
