using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using TMPro;

public class MatchmakingManager : MonoBehaviourPunCallbacks
{

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        UIManager.ChangeNetworkText("Connecting to Master...");

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        UIManager.ChangeNetworkText("In Master");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        UIManager.ChangeNetworkText("In Lobby");
        UIManager.ShowMainMenu();

        base.OnJoinedLobby();
    }

    public void OnPressPlay()
    {
        if (PhotonNetwork.InLobby)
        {
            UIManager.ShowLoadingMenu();

            PhotonNetwork.JoinRandomRoom();
        }
        else Debug.Log($"OnPressPlay: Not in a lobby yet. Yo can only join a room if you are in a lobby.");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions options = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(null, options);
    }

    public override void OnJoinedRoom()
    {
        UIManager.ChangeNetworkText("In Room. Waiting for player...");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            UIManager.ChangeNetworkText("In Room with another player");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }
}
