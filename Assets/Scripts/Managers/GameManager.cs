using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    // State
    public enum GameState
    {
        MainMenu,
        Loading,
        InGame,
        GameOver
    }

    public GameState State { get; private set; }
    public PlayerColor CurrentTurn { get; private set; } = PlayerColor.White;

    public bool IsGameActive => State == GameState.InGame;
    public bool IsGameOver => State == GameState.GameOver;

    // Pieces
    private bool _piecesAreSpawned;

    public bool PiecesAreSpawned
    {
        get => _piecesAreSpawned;
        set => _piecesAreSpawned = value;
    }

    // References
    private PhotonView _photonView;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Application.runInBackground = true;

        _photonView = GetComponent<PhotonView>();
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (State)
        {
            case GameState.MainMenu:
                break;
            case GameState.Loading:
                break;
            case GameState.InGame:
                break;
            case GameState.GameOver:
                UIManager.Instance.HideResignTopButton();
                break;
        }
    }

    public void SetGameStateNetwork(GameState newState)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Hashtable prop = new()
        {
            { "GameState", newState.ToString() }
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
    }

    public bool IsMyTurn()
    {
        var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
        var playerProps = PhotonNetwork.LocalPlayer.CustomProperties;

        if (!roomProps.ContainsKey("Turn") || !playerProps.ContainsKey("Color") || Instance.IsGameOver)
            return false;

        PlayerColor currentTurn = (PlayerColor)(int)roomProps["Turn"];
        PlayerColor selfColor = (PlayerColor)(int)playerProps["Color"];

        return currentTurn == selfColor;
    }

    public void SwitchTurn()
    {
        PlayerColor currentTurn = (PlayerColor)(int)PhotonNetwork.CurrentRoom.CustomProperties["Turn"];
        CurrentTurn = currentTurn == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;

        Hashtable turnProps = new() { { "Turn", (int)CurrentTurn } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);
    }

    public void AssignFirstTurn(PlayerColor color)
    {
        Hashtable turnProps = new() { { "Turn", (int)color } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);
    }

    public void OnPieceMovedBySelf(GameObject piece, Vector2Int from, Vector2Int to, GameObject target)
    {
        var data = piece.GetComponent<ChessPiece>().PieceData;
        var moveIsCastle = false;

        // Castling   
        if (data.PieceType == PieceType.King)
        {
            moveIsCastle = Mathf.Abs(from.x - to.x) == 2;
            BoardState.Instance.DisableCastling();
        }

        if (data.PieceType == PieceType.Rook)
        {
            var selfColor = PlayerManager.Instance.SelfColor;

            if (from.x == 7) BoardState.Instance.DisableCastlingSide(PieceData.RookSide.King, selfColor);
            if (from.x == 0) BoardState.Instance.DisableCastlingSide(PieceData.RookSide.Queen, selfColor);
        }

        // SFX
        if (BoardUtils.IsAnyKingInCheck())
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Check);
            _photonView.RPC("PlayCheckSFX", RpcTarget.Others);
        }
        else if (moveIsCastle)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Castling);
            _photonView.RPC("PlayCastlingSFX", RpcTarget.Others);
        }
        else if (target != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Capture);
            _photonView.RPC("PlayCaptureSFX", RpcTarget.Others);
        }
        else
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.SelfMove);
            _photonView.RPC("PlayOpponentMoveSFX", RpcTarget.Others);
        }

        SwitchTurn();
        HighlightMoves.Instance.ClearHighlights();
        TimerManager.Instance.OnPieceMovedBySelf();
    }

    [PunRPC] void PlayCheckSFX() => AudioManager.Instance.PlaySFX(AudioManager.Instance.Check);
    [PunRPC] void PlayCaptureSFX() => AudioManager.Instance.PlaySFX(AudioManager.Instance.Capture);
    [PunRPC] void PlayCastlingSFX() => AudioManager.Instance.PlaySFX(AudioManager.Instance.Castling);
    [PunRPC] void PlayOpponentMoveSFX() => AudioManager.Instance.PlaySFX(AudioManager.Instance.OpponentMove);

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        {
            if (!PhotonNetwork.IsMasterClient) return;
            var props = PhotonNetwork.CurrentRoom.CustomProperties;

            if (PhotonNetwork.PlayerList.Length == 2 &&
                !props.ContainsKey(RoomProps.P1Name) &&
                !props.ContainsKey(RoomProps.P2Name))
            {
                var p1 = PhotonNetwork.PlayerList[0];
                var p2 = PhotonNetwork.PlayerList[1];

                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable
                {
                    { RoomProps.P1Name, p1.NickName },
                    { RoomProps.P2Name, p2.NickName }
                });
            }
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable properties)
    {
        if (properties.ContainsKey("GameState"))
        {
            var state = (string)properties["GameState"];
            UpdateGameState((GameState)System.Enum.Parse(typeof(GameState), state));
        }
        if (properties.ContainsKey("Turn"))
        {
            CurrentTurn = (PlayerColor)properties["Turn"];
        }
    }

    public void TriggerGameOver(GameResult selfResult, GameOverReason reason)
    {
        UpdateGameState(GameState.GameOver);
        SetGameStateNetwork(GameState.GameOver);
        UIManager.Instance.ShowGameOverPanel(selfResult, reason);
    }

    public void Resign()
    {
        TriggerGameOver(GameResult.Lose, GameOverReason.Resign);
        _photonView.RPC("OnOpponentResigned", RpcTarget.Others);
    }

    [PunRPC]
    void OnOpponentResigned() => TriggerGameOver(GameResult.Win, GameOverReason.Resign);
}
