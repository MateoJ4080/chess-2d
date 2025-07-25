using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    public enum PlayerColor
    {
        White,
        Black
    }

    public enum GameState
    {
        MainMenu,
        Loading,
        InGame,
        GameOver
    }

    public PlayerColor currentTurn { get; private set; }
    public GameState state;

    private bool piecesAreSpawned = false;
    public bool PiecesAreSpawned
    {
        get => piecesAreSpawned;
        set => piecesAreSpawned = value;
    }

    private void Awake()
    {
        Debug.Log("GameManager Awake");
        DontDestroyOnLoad(gameObject);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (state)
        {
            case GameState.MainMenu:
                break;
            case GameState.Loading:
                break;
            case GameState.InGame:
                break;
            case GameState.GameOver:
                break;
        }
    }

    public void OnPressPlay()
    {
        Instance.state = GameState.Loading;
    }

    // Handle turns
    public bool ItsMyTurn()
    {
        return PhotonNetwork.CurrentRoom.CustomProperties["Turn"] as string == PhotonNetwork.LocalPlayer.CustomProperties["Color"] as string;
    }

    public void SwitchTurn()
    {
        currentTurn = PhotonNetwork.CurrentRoom.CustomProperties["Turn"] as string == PlayerColor.White.ToString() ? PlayerColor.Black : PlayerColor.White;
        Debug.Log($"<color=yellow>Switching turn to {currentTurn}");

        // Assign new turn to room properties
        ExitGames.Client.Photon.Hashtable turnProps = new() { { "Turn", currentTurn.ToString() } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);
    }

    public static void AssignFirstTurnWhite()
    {
        ExitGames.Client.Photon.Hashtable turnProps = new() { { "Turn", "White" } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);
    }
}
