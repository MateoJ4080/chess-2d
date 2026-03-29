using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviourPunCallbacks
{
    private Dictionary<string, RoomInfo> _rooms = new Dictionary<string, RoomInfo>();

    [Header("Match List")]
    [SerializeField] private Transform _matchItemContainer;
    [SerializeField] private GameObject _matchItemPrefab;

    [Header("Panels")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private GameObject _matchListPanel;

    [Header("Debug - Network")]
    [SerializeField] private GameObject _networkStatusPanel;
    [SerializeField] private TextMeshProUGUI _networkStatusText;
    [SerializeField] private TextMeshProUGUI _debugTextState;
    [SerializeField] private TextMeshProUGUI _debugTextConnection;
    [SerializeField] private TextMeshProUGUI _debugTextReady;
    [SerializeField] private TextMeshProUGUI _debugTextLobby;
    [SerializeField] private TextMeshProUGUI _debugTextRoom;
    [SerializeField] private TextMeshProUGUI _debugTextMaster;
    [SerializeField] private TextMeshProUGUI _debugTextScene;

    [Header("Debug - Room Properties")]
    [SerializeField] private TextMeshProUGUI _isWhiteCheckOnceText;
    [SerializeField] private TextMeshProUGUI _isWhiteCheckTwiceText;
    [SerializeField] private TextMeshProUGUI _isBlackCheckOnceText;
    [SerializeField] private TextMeshProUGUI _isBlackCheckTwiceText;
    [SerializeField] private TextMeshProUGUI _colorInfoText;
    [SerializeField] private Transform _networkTextRefMainMenu;
    [SerializeField] private Transform _networkTextRefLoading;

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

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MenuScene")
        {
            ShowMenuPanel();
        }

        if (scene.name == "GameScene")
        {
            _loadingPanel.SetActive(false);
            _networkStatusPanel.SetActive(false);
        }
    }

    void Start()
    {
        ShowMenuPanel();
    }

    void Update()
    {
        UpdateDebugNetworkTexts();
        UpdateRoomPropertiesTexts();
    }

    public void ShowMenuPanel() => _mainMenuPanel.SetActive(true);
    public void HideMenuPanel() => _mainMenuPanel.SetActive(false);

    public void ShowOptionsPanel() => _optionsPanel.SetActive(true);
    public void HideOptionsPanel() => _optionsPanel.SetActive(false);

    public void ShowLoadingPanel() => _loadingPanel.SetActive(true);
    public void HideLoadingPanel() => _loadingPanel.SetActive(false);

    public void ShowPanel() => _networkStatusPanel.SetActive(true);
    public void HidePanel() => _networkStatusPanel.SetActive(false);

    public void ShowMatchList()
    {
        foreach (Transform child in _matchItemContainer)
            Destroy(child.gameObject);

        _matchListPanel.SetActive(true);

        foreach (var room in _rooms)
        {
            var item = Instantiate(_matchItemPrefab, Vector3.zero, Quaternion.identity, _matchItemContainer);
            item.GetComponent<MatchItem>().SetData(room.Value);
        }
    }
    public void HideMatchList() => _matchListPanel.SetActive(false);

    [ContextMenu("TestShowMatchList")]
    private void TestShowMatchList()
    {
        for (int i = 1; i <= 5; i++)
        {
            var item = Instantiate(_matchItemPrefab, _matchItemContainer);
            item.GetComponent<MatchItem>().SetData("A" + i, "B" + i, i * 250, i * 250);
        }
    }

    public void ChangeNetworkText(string text) => _networkStatusText.text = text;

    public void UpdateColorText(string color) => _colorInfoText.text = "Color: " + color;

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


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                _rooms.Remove(room.Name);
            }
            else
            {
                _rooms[room.Name] = room;
            }
        }
    }
}
