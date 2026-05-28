using Photon.Pun;
using UnityEngine;

public class TimerManager : MonoBehaviourPun
{
    public static TimerManager Instance { get; private set; }

    private double _selfTime;
    private double _opponentTime;

    private double _lastTurnStartTime;
    private double _lastTurnDuration;

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        double matchTime;
        _lastTurnStartTime = PhotonNetwork.Time;

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(RoomProps.MatchTime, out object value)) matchTime = (double)value;
        else
        {
            Debug.LogError("MatchTime room property not found, assigning default value (180)");
            matchTime = 180.0;
        }

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
        if (!PhotonNetwork.InRoom) return;
        if (GameManager.Instance.State != GameManager.GameState.InGame) return;

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
