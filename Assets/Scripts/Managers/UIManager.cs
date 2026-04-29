using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviourPunCallbacks
{
    private Dictionary<string, RoomInfo> _rooms = new Dictionary<string, RoomInfo>();

    [Header("Match List")]
    [SerializeField] private Transform _matchItemContainer;
    [SerializeField] private GameObject _matchItemPrefab;

    [Header("Panels")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _nicknamePanel;
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private GameObject _matchListPanel;
    [SerializeField] private GameObject _playerPanelsParent;
    [SerializeField] private GameObject _topButtonsPanel;

    [Header("Main Menu")]
    [SerializeField] private Button _playButton;

    [Header("Match End Panel")]
    [SerializeField] private GameObject _matchEndPanel;
    [SerializeField] private TextMeshProUGUI _matchResult;
    [SerializeField] private GameObject _matchResultEmoji;

    [Header("Top Buttons")]
    [SerializeField] private Button _debugButton;
    [SerializeField] private Color _offColor;
    [SerializeField] private Color _onColor;

    [Header("Player Texts")]
    [SerializeField] private TextMeshProUGUI _selfNickname;
    [SerializeField] private TextMeshProUGUI _selfElo;
    [SerializeField] private TextMeshProUGUI _opponentNickname;
    [SerializeField] private TextMeshProUGUI _opponentElo;

    [Header("Timers")]
    [SerializeField] private TextMeshProUGUI _selfTimer;
    [SerializeField] private TextMeshProUGUI _opponentTimer;

    [Header("Debug - Network")]
    [SerializeField] private GameObject _debugTextsPanel;
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

    void Start()
    {
        ShowMenuPanel();
        if (!PhotonNetwork.InLobby) _playButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
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

    public override void OnJoinedLobby()
    {
        _playButton.interactable = true;
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
        }
    }

    void Update()
    {
        UpdateDebugNetworkTexts();
        UpdateRoomPropertiesTexts();
    }

    public void ShowMenuPanel()
    {
        if (SceneManager.GetActiveScene().name == "MenuScene") _mainMenuPanel.SetActive(true);
    }

    public void HideMenuPanel() => _mainMenuPanel.SetActive(false);

    public void ShowNicknamePanel() => _nicknamePanel.SetActive(true);
    public void HideNicknamePanel() => _nicknamePanel.SetActive(false);

    public void ShowSettingsPanel() => _settingsPanel.SetActive(true);
    public void HideSettingsPanel() => _settingsPanel.SetActive(false);

    public void ShowLoadingPanel() => _loadingPanel.SetActive(true);
    public void HideLoadingPanel() => _loadingPanel.SetActive(false);

    public void ShowDebugTextsPanel() => _debugTextsPanel.SetActive(true);
    public void HideDebugTextsPanel() => _debugTextsPanel.SetActive(false);
    // Panel must start disabled
    public void ToggleDebugTextsPanel()
    {
        bool status = !_debugTextsPanel.activeSelf;
        _debugTextsPanel.SetActive(status);
        var colors = _debugButton.colors;
        colors.normalColor = status ? _onColor : _offColor;
        _debugButton.colors = colors;
    }

    public void ShowPlayerPanelsParent() => _playerPanelsParent.SetActive(true);
    public void HidePlayerPanelsParent() => _playerPanelsParent.SetActive(false);

    public void ShowMatchEndPanel() => _matchEndPanel.SetActive(true);
    public void HideMatchEndPanel() => _matchEndPanel.SetActive(false);

    public void ShowTopButtonsPanel() => _topButtonsPanel.SetActive(true);
    public void HideTopButtonsPanel() => _topButtonsPanel.SetActive(false);

    public void ShowMatchList()
    {
        foreach (Transform child in _matchItemContainer)
            Destroy(child.gameObject);

        _matchListPanel.SetActive(true);

        foreach (var room in _rooms)
        {
            if (room.Value.PlayerCount <= 2) // Should change conditional to something like "matchHasStarted" later
            {
                var item = Instantiate(_matchItemPrefab, Vector3.zero, Quaternion.identity, _matchItemContainer);
                item.GetComponent<MatchItem>().SetData(room.Value);
            }
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

    public void UpdatePlayerPanels(string selfText, string opponentText)
    {
        _selfNickname.text = selfText;
        _opponentNickname.text = opponentText;

        LayoutRebuilder.ForceRebuildLayoutImmediate(_selfNickname.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_opponentNickname.rectTransform);
    }

    public void UpdatePlayerPanels(string selfName, string selfElo, string opponentName, string opponentElo)
    {
        _selfNickname.text = selfName;
        _selfElo.text = selfElo;
        _opponentNickname.text = opponentName;
        _opponentElo.text = opponentElo;

        LayoutRebuilder.ForceRebuildLayoutImmediate(_selfNickname.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_opponentNickname.rectTransform);
    }

    public void UpdateTimers(double self, double opponent)
    {
        if (_selfTimer != null && _opponentTimer != null)
        {
            _selfTimer.text = FormatTime(self);
            _opponentTimer.text = FormatTime(opponent);
        }
    }

    string FormatTime(double time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void SetResultText(GameResult result)
    {
        _matchResultEmoji.SetActive(false);
        switch (result)
        {
            case GameResult.Win:
                _matchResult.text = "You Win!";
                _matchResultEmoji.SetActive(true);
                break;
            case GameResult.Lose:
                string color = PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Color", out object colorObj) ? (string)colorObj : "undefined";
                _matchResult.text = $"{color} Won";
                break;
            case GameResult.Draw:
                _matchResult.text = "Draw";
                break;
            case GameResult.Stalemate:
                _matchResult.text = "Stalemate";
                break;
        }
    }
}
