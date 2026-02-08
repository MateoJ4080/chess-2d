using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MatchmakingManager : MonoBehaviourPunCallbacks
{

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.ChangeNetworkText("In Master");

        if (!PhotonNetwork.InLobby && PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.ChangeNetworkText("In Lobby");

        base.OnJoinedLobby();
    }

    public void OnPressPlay()
    {
        if (PhotonNetwork.InLobby)
        {
            UIManager.Instance.ShowLoadingMenu();
            PhotonNetwork.JoinRandomRoom();
        }
        else Debug.Log($"OnPressPlay: Not in a lobby yet. You can only join a room if you are in one.");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions options = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(null, options);
    }

    public override void OnJoinedRoom()
    {
        UIManager.Instance.ChangeNetworkText("In Room. Waiting for player...");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            UIManager.Instance.ChangeNetworkText("In Room with another player");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("OnPlayerLeftRoom");

        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected");

        SceneManager.LoadScene("MenuScene");
    }
}
