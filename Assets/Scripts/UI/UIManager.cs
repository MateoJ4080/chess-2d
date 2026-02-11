using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _loadingMenu;

    // Debug texts
    [SerializeField] private TextMeshProUGUI _networkStatusText;
    [SerializeField] private TextMeshProUGUI _debugTextState;
    [SerializeField] private TextMeshProUGUI _debugTextConnection;
    [SerializeField] private TextMeshProUGUI _debugTextReady;
    [SerializeField] private TextMeshProUGUI _debugTextLobby;
    [SerializeField] private TextMeshProUGUI _debugTextRoom;
    [SerializeField] private TextMeshProUGUI _debugTextMaster;
    [SerializeField] private TextMeshProUGUI _debugTextScene;


    public static UIManager Instance { get; private set; }

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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            _loadingMenu.SetActive(false);
            _networkStatusText.transform.parent.gameObject.SetActive(false);
        }
    }


    void Start()
    {
        ShowMainMenu();
    }

    void Update()
    {
        UpdateDebugNetworkTexts();
    }

    public void ShowMainMenu()
    {
        _loadingMenu.SetActive(false);
        _mainMenu.SetActive(true);
    }

    public void ShowLoadingMenu()
    {
        _mainMenu.SetActive(false);
        _loadingMenu.SetActive(true);
    }

    public void ChangeNetworkText(string text)
    {
        _networkStatusText.text = text;
    }

    void UpdateDebugNetworkTexts()
    {
        _debugTextState.text = $"State: {PhotonNetwork.NetworkClientState}";
        _debugTextConnection.text = $"Connection: {PhotonNetwork.IsConnected}";
        _debugTextReady.text = $"Ready: {PhotonNetwork.IsConnectedAndReady}";
        _debugTextLobby.text = $"Lobby: {PhotonNetwork.InLobby}";
        _debugTextRoom.text = $"Room: {PhotonNetwork.InRoom}";
        _debugTextMaster.text = $"Master: {PhotonNetwork.IsMasterClient}";
        _debugTextScene.text = $"Scene: {SceneManager.GetActiveScene().name}";
    }
}
