using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MatchmakingManager : MonoBehaviourPunCallbacks
{
    private readonly Dictionary<string, RoomInfo> cachedRooms = new();
    public static MatchmakingManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

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

        if (!PhotonNetwork.InLobby && PhotonNetwork.NetworkClientState != ClientState.JoiningLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.ChangeNetworkText("In Lobby");

        Debug.Log("OnJoinedLobby");
        base.OnJoinedLobby();
    }

    public void OnPressPlay()
    {
        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("Not in lobby");
            return;
        }

        if (cachedRooms.Count > 0)
        {
            foreach (var room in cachedRooms.Values)
            {
                PhotonNetwork.JoinRoom(room.Name);
                break;
            }
        }
        else PhotonNetwork.CreateRoom("null", new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnJoinedRoom()
    {
        UIManager.Instance.ChangeNetworkText("In Room. Waiting for player...");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            UIManager.Instance.ChangeNetworkText("In Room with another player");
        }
    }

    public override void OnCreatedRoom()
    {
        UIManager.Instance.ChangeNetworkText("In Room. Waiting for player...");
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

        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");

        SceneManager.LoadScene("MenuScene");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            if (room.RemovedFromList)
                cachedRooms.Remove(room.Name);
            else
                cachedRooms[room.Name] = room;
        }
    }
}
