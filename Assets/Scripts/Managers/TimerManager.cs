using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TimerManager : MonoBehaviourPunCallbacks
{
    public static TimerManager Instance { get; private set; }

    private double _selfTime;
    private double _opponentTime;

    private double _lastTurnStartTime;
    private double _lastTurnDuration;

    double _matchTime = 180;
    bool _started;
    bool _startTimeAssigned;

    void Awake()
    {
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
        if (!_started || PhotonNetwork.Time == 0) return;

        if (!_startTimeAssigned)
        {
            _lastTurnStartTime = PhotonNetwork.Time;
            _startTimeAssigned = true;
        }

        _lastTurnDuration = PhotonNetwork.Time - _lastTurnStartTime;

        double currentSelf = _selfTime;
        double currentOpponent = _opponentTime;

        if (GameManager.Instance.IsMyTurn())
            currentSelf = _selfTime - _lastTurnDuration;
        else
            currentOpponent = _opponentTime - _lastTurnDuration;

        currentSelf = System.Math.Max(0, currentSelf);
        currentOpponent = System.Math.Max(0, currentOpponent);

        if (currentSelf <= 0 || currentOpponent <= 0)
        {
            GameManager.Instance.UpdateGameState(GameManager.GameState.GameOver);
            var selfResult = currentSelf <= 0 ? GameResult.Lose : GameResult.Win;
            UIManager.Instance.ShowGameOverPanel(selfResult, GameOverReason.Timeout);
        }

        UIManager.Instance.UpdateTimers(currentSelf, currentOpponent);
    }

    public void OnPieceMovedBySelf()
    {
        _selfTime -= _lastTurnDuration;
        _lastTurnStartTime = PhotonNetwork.Time;

        photonView.RPC("SyncTimer", RpcTarget.Others, _lastTurnDuration);
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
}
