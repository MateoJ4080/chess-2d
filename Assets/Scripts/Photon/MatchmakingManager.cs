using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

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

        base.OnJoinedLobby();
    }

    public void TryJoinOrCreate()
    {
        foreach (var room in cachedRooms.Values)
        {
            bool matchStarted = room.CustomProperties.ContainsKey(RoomProps.MatchStarted)
                                && (bool)room.CustomProperties[RoomProps.MatchStarted];

            if (!matchStarted && room.PlayerCount < 2)
            {
                PhotonNetwork.JoinRoom(room.Name);
                return;
            }
        }

        PhotonNetwork.CreateRoom(null, new RoomOptions
        {
            MaxPlayers = 20,
            CustomRoomProperties = new Hashtable
                {
                    {RoomProps.MatchStarted, false},
                    {RoomProps.MatchTime, (double)180}
                },
            CustomRoomPropertiesForLobby = new string[] {
                    RoomProps.P1Name,
                    RoomProps.P2Name,
                    RoomProps.P1Elo,
                    RoomProps.P2Elo,
                    RoomProps.MatchStarted,
                    RoomProps.MatchTime
                }
        });
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
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable
            {
                { RoomProps.MatchStarted, true }
            });

            PhotonNetwork.LoadLevel("Game");
        }
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

    public void CancelMatchmaking() => PhotonNetwork.LeaveRoom();
}
