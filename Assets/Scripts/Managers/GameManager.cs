using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public enum GameState
    {
        MainMenu,
        Loading,
        InGame,
        GameOver
    }

    public PlayerColor CurrentTurn { get; private set; } = PlayerColor.White;
    public GameState State { get; private set; }

    private bool piecesAreSpawned = false;
    public bool PiecesAreSpawned
    {
        get => piecesAreSpawned;
        set => piecesAreSpawned = value;
    }

    public static GameManager Instance { get; private set; }

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

    public override void OnCreatedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        var props = new Hashtable {
            {"whiteCK", true},
            {"whiteCQ", true},
            {"blackCK", true},
            {"blackCQ", true}
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    public bool IsMyTurn()
    {
        var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
        var playerProps = PhotonNetwork.LocalPlayer.CustomProperties;

        if (!roomProps.ContainsKey("Turn") || !playerProps.ContainsKey("Color") || Instance.IsGameOver())
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

    public void AssignFirstTurnWhite()
    {
        Hashtable turnProps = new() { { "Turn", (int)PlayerColor.White } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);
    }

    public void OnPieceMovedBySelf(GameObject piece, Vector2Int from, Vector2Int to)
    {
        var data = piece.GetComponent<ChessPiece>().PieceData;

        // Castling   
        if (data.PieceType == "King")
        {
            DisableSelfCastling();
        }

        if (data.PieceType == "Rook")
        {
            bool isWhite = data.Color == PlayerColor.White;

            if (isWhite && from == new Vector2Int(7, 0)) DisableRookSide(PieceData.RookSide.King);
            if (isWhite && from == new Vector2Int(0, 0)) DisableRookSide(PieceData.RookSide.Queen);

            if (!isWhite && from == new Vector2Int(7, 0)) DisableRookSide(PieceData.RookSide.Queen);
            if (!isWhite && from == new Vector2Int(0, 0)) DisableRookSide(PieceData.RookSide.King);
        }

        BoardState.Instance.HandleEnPassant(from, to, data);
    }

    void DisableSelfCastling()
    {
        var p = new Hashtable();
        var selfColor = PlayerManager.Instance.SelfColor;

        if (selfColor == PlayerColor.White)
        {
            p["whiteCK"] = false;
            p["whiteCQ"] = false;
        }
        else
        {
            p["blackCK"] = false;
            p["blackCQ"] = false;
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(p);
    }

    void DisableRookSide(PieceData.RookSide side)
    {
        var p = new Hashtable();
        var selfColor = PlayerColor.White;

        if (selfColor == PlayerColor.White) p[side == PieceData.RookSide.King ? "whiteCK" : "whiteCQ"] = false;
        else p[side == PieceData.RookSide.King ? "blackCK" : "blackCQ"] = false;

        PhotonNetwork.CurrentRoom.SetCustomProperties(p);
    }

    public bool CanCastle(PieceData.RookSide side, GameObject pieceGO)
    {
        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        Vector2Int piecePos = Vector2Int.RoundToInt(pieceGO.transform.position);

        // These variables are turn to false when a king or a rook moves
        bool whiteCK = props.ContainsKey("whiteCK") && (bool)props["whiteCK"];
        bool whiteCQ = props.ContainsKey("whiteCQ") && (bool)props["whiteCQ"];
        bool blackCK = props.ContainsKey("blackCK") && (bool)props["blackCK"];
        bool blackCQ = props.ContainsKey("blackCQ") && (bool)props["blackCQ"];

        PlayerColor selfColor = PlayerManager.Instance.SelfColor;
        PlayerColor enemyColor = PlayerManager.Instance.EnemyColor;

        var direction = selfColor == PlayerColor.White ? 1 : -1;
        var availableKingside = selfColor == PlayerColor.White ? whiteCK : blackCK;
        var availableQueenside = selfColor == PlayerColor.White ? whiteCQ : blackCQ;

        if (side == PieceData.RookSide.King)
        {
            Vector2Int firstTile = piecePos + new Vector2Int(1, 0) * direction;
            Vector2Int secondTile = piecePos + new Vector2Int(2, 0) * direction;

            bool isPathThreatened = BoardState.Instance.IsSquareAttackedBy(firstTile, enemyColor) || BoardState.Instance.IsSquareAttackedBy(secondTile, enemyColor);
            bool areSquaresEmpty = BoardUtils.SquareIsEmpty(firstTile) && BoardUtils.SquareIsEmpty(secondTile);

            return !isPathThreatened && areSquaresEmpty && availableKingside;
        }
        if (side == PieceData.RookSide.Queen)
        {

            Vector2Int firstTile = piecePos + new Vector2Int(-1, 0) * direction;
            Vector2Int secondTile = piecePos + new Vector2Int(-2, 0) * direction;

            bool isPathThreatened = BoardState.Instance.IsSquareAttackedBy(firstTile, PlayerManager.Instance.EnemyColor) ||
                                    BoardState.Instance.IsSquareAttackedBy(secondTile, PlayerManager.Instance.EnemyColor);
            bool areSquaresEmpty = BoardUtils.SquareIsEmpty(firstTile) && BoardUtils.SquareIsEmpty(secondTile);

            return !isPathThreatened && areSquaresEmpty && availableQueenside;
        }

        return false;
    }

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

    public void BackToMenu()
    {
        PhotonNetwork.LeaveRoom();

        UpdateGameState(GameState.MainMenu);
        SceneManager.LoadScene("MenuScene");
    }

    public void TriggerGameOver(GameResult selfResult, GameOverReason reason)
    {
        UpdateGameState(GameState.GameOver);
        SetGameStateNetwork(GameState.GameOver);
        UIManager.Instance.ShowGameOverPanel(selfResult, reason);
    }

    public bool IsGameOver()
    {
        return State == GameState.GameOver;
    }
}
