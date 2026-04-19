using Photon.Pun;
using UnityEngine;

public class TimerManager : MonoBehaviourPun
{
    public static TimerManager Instance { get; private set; }

    private double _selfTime;
    private double _opponentTime;

    private double _lastTurnStartTime;
    private double _lastTurnDuration;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        double matchTime;
        _lastTurnStartTime = PhotonNetwork.Time;

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(RoomProps.MatchTime, out object value)) matchTime = (double)value;
        else matchTime = 180.0;

        _selfTime = matchTime;
        _opponentTime = matchTime;

        UIManager.Instance.UpdateTimers(_selfTime, _opponentTime);
    }

    void Update()
    {
        UpdateTimersAndUI();
    }

    public void UpdateTimersAndUI()
    {
        if (PhotonNetwork.CurrentRoom == null) return;
        if (GameManager.Instance.State != GameManager.GameState.InGame) return;

        _lastTurnDuration = PhotonNetwork.Time - _lastTurnStartTime;

        double currentSelf = _selfTime;
        double currentOpponent = _opponentTime;

        if (GameManager.Instance.ItsMyTurn())
            currentSelf = _selfTime - _lastTurnDuration;
        else
            currentOpponent = _opponentTime - _lastTurnDuration;

        UIManager.Instance.UpdateTimers(currentSelf, currentOpponent);
    }

    public void OnPieceMovedBySelf()
    {
        _selfTime -= _lastTurnDuration;
        _lastTurnStartTime = PhotonNetwork.Time;

        GameManager.Instance.SyncTurnTimerRPC(_lastTurnDuration);
    }

    public void OnRemoteTurn(double duration)
    {
        _opponentTime -= duration;
        _lastTurnStartTime = PhotonNetwork.Time;
    }
}
