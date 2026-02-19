using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _loadingMenu;

    // Debug - Network texts
    [SerializeField] private TextMeshProUGUI _networkStatusText;
    [SerializeField] private TextMeshProUGUI _debugTextState;
    [SerializeField] private TextMeshProUGUI _debugTextConnection;
    [SerializeField] private TextMeshProUGUI _debugTextReady;
    [SerializeField] private TextMeshProUGUI _debugTextLobby;
    [SerializeField] private TextMeshProUGUI _debugTextRoom;
    [SerializeField] private TextMeshProUGUI _debugTextMaster;
    [SerializeField] private TextMeshProUGUI _debugTextScene;

    // Debug - Room properties
    [SerializeField] private TextMeshProUGUI _isWhiteCheckOnceText;
    [SerializeField] private TextMeshProUGUI _isWhiteCheckTwiceText;
    [SerializeField] private TextMeshProUGUI _isBlackCheckOnceText;
    [SerializeField] private TextMeshProUGUI _isBlackCheckTwiceText;

    [SerializeField] private TextMeshProUGUI colorInfoText;

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
        if (scene.name == "MenuScene")
        {
            _mainMenu.SetActive(true);
            _networkStatusText.transform.parent.gameObject.SetActive(true);
        }

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
        UpdateRoomPropertiesTexts();
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
        _debugTextConnection.text = $"Connected: {PhotonNetwork.IsConnected}";
        _debugTextReady.text = $"Ready: {PhotonNetwork.IsConnectedAndReady}";
        _debugTextLobby.text = $"Lobby: {PhotonNetwork.InLobby}";
        _debugTextRoom.text = $"Room: {PhotonNetwork.InRoom}";
        _debugTextMaster.text = $"Master: {PhotonNetwork.IsMasterClient}";
        _debugTextScene.text = $"Scene: {SceneManager.GetActiveScene().name}";
    }

    void UpdateRoomPropertiesTexts()
    {
        if (!PhotonNetwork.InRoom)
            return;

        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (props.TryGetValue("whiteInCheckOnce", out var value))
            _isWhiteCheckOnceText.text = $"whiteCheckOnce: {value}";

        if (props.TryGetValue("whiteInCheckTwice", out value))
            _isWhiteCheckTwiceText.text = $"whiteCheckTwice: {value}";

        if (props.TryGetValue("blackInCheckOnce", out value))
            _isBlackCheckOnceText.text = $"blackCheckOnce: {value}";

        if (props.TryGetValue("blackInCheckTwice", out value))
            _isBlackCheckTwiceText.text = $"blackCheckTwice: {value}";
    }

    public void UpdateColorText(string color)
    {
        colorInfoText.text = "Color: " + color;
    }
}
