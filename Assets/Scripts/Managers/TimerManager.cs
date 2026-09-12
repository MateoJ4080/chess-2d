using System.Collections;
using Photon.Pun;
using UnityEngine;

public class TimerManager : MonoBehaviourPunCallbacks
{
    public static TimerManager Instance { get; private set; }

    [SerializeField] private double _demoSelfTime = 600;
    [SerializeField] private double _demoOpponentTime = 600;

    private double _selfTime;
    private double _opponentTime;

    private double _lastTurnStartTime;
    private double _turnElapsedTime;

    private double _matchTime = 180;
    private bool _started;
    private bool _startTimeAssigned;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(RoomProps.MatchTime, out object value))
            Debug.LogError("OnJoinedRoom: MatchTime room property not found, assigning default value (180)");
        else
            _matchTime = (double)value;

        UIManager.Instance.UpdateTimers(_matchTime, _matchTime);
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() =>
            PhotonNetwork.InRoom &&
            GameManager.Instance.State == GameManager.GameState.InGame);

        _selfTime = _matchTime;
        _opponentTime = _matchTime;

        _started = true;
    }

    void Update()
    {
        UpdateTimersAndUI();
    }

    public void UpdateTimersAndUI()
    {
        if (!GameManager.Instance.IsGameActive) return;

        if (!_started || PhotonNetwork.Time == 0) return;

        if (!_startTimeAssigned)
        {
            _lastTurnStartTime = PhotonNetwork.Time;
            _startTimeAssigned = true;
        }

        _turnElapsedTime = PhotonNetwork.Time - _lastTurnStartTime;

        double currentSelf = _selfTime;
        double currentOpponent = _opponentTime;

        if (GameManager.Instance.IsMyTurn())
            currentSelf = _selfTime - _turnElapsedTime;
        else
            currentOpponent = _opponentTime - _turnElapsedTime;

        currentSelf = System.Math.Max(0, currentSelf);
        currentOpponent = System.Math.Max(0, currentOpponent);

        if (currentSelf <= 0 || currentOpponent <= 0)
        {
            var selfResult = currentSelf <= 0 ? GameResult.Lose : GameResult.Win;
            GameManager.Instance.TriggerGameOver(selfResult, GameOverReason.Timeout);
        }

        UIManager.Instance.UpdateTimers(currentSelf, currentOpponent);
    }

    public void OnPieceMovedBySelf()
    {
        _selfTime -= _turnElapsedTime;
        _lastTurnStartTime = PhotonNetwork.Time;

        photonView.RPC(nameof(SyncTimer), RpcTarget.Others, _turnElapsedTime);
    }

    public void OnRemoteTurn(double turnDuration)
    {
        _opponentTime -= turnDuration;
        _lastTurnStartTime = PhotonNetwork.Time;
    }

    [PunRPC]
    void SyncTimer(double duration)
    {
        OnRemoteTurn(duration);
    }

    [ContextMenu("Set self timer to 5s")]
    public void SetSelfTimerToFive()
    {
        _selfTime = 5;
        _lastTurnStartTime = PhotonNetwork.Time;

        photonView.RPC("SyncTimer", RpcTarget.Others, (double)175);
    }

    [ContextMenu("Set debug timers")]
    public void SetDebugTimers()
    {
        ApplyDebugTimers();
        photonView.RPC(nameof(SyncDebugTimers), RpcTarget.Others);
    }

    [PunRPC]
    void SyncDebugTimers()
    {
        ApplyDebugTimers();
    }

    private void ApplyDebugTimers()
    {
        _lastTurnStartTime = PhotonNetwork.Time;
        _selfTime = _demoSelfTime;
        _opponentTime = _demoOpponentTime;

        UIManager.Instance.UpdateTimers(_selfTime, _opponentTime);
    }
}
