using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    private bool colorsAreAssigned;
    public bool ColorsAreAssigned
    {
        get => colorsAreAssigned;
        private set => colorsAreAssigned = value;
    }
    public PlayerColor SelfColor { get; private set; }

    public static PlayerManager Instance { get; private set; }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void OnConnectedToMaster()
    {
        Hashtable props = new()
        {
            { PlayerProps.Elo, 200 },
            { PlayerProps.IsPlayer, true }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    // Check if colors are assigned by looking at room properties
    public bool CheckColorsAssigned()
    {
        return PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("ColorsAssigned", out var value) && (bool)value;
    }

    public static void AssignRandomColors()
    {
        Player[] players = PhotonNetwork.PlayerList;

        if (players.Length < 2)
        {
            Debug.LogWarning("Not enough players to assign colors");
            return;
        }

        if (!PhotonNetwork.IsMasterClient) return;

        bool firstIsWhite = Random.value < 0.5f;

        Hashtable p1props = new();
        p1props["Color"] = (int)(firstIsWhite ? PlayerColor.White : PlayerColor.Black);
        players[0].SetCustomProperties(p1props);

        Hashtable p2props = new();
        p2props["Color"] = (int)(firstIsWhite ? PlayerColor.Black : PlayerColor.White);
        players[1].SetCustomProperties(p2props);

        // Set room property to indicate colors are assigned
        Hashtable roomProps = new();
        roomProps["ColorsAssigned"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);

        Instance.ColorsAreAssigned = true;
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("ColorsAssigned"))
        {
            colorsAreAssigned = (bool)propertiesThatChanged["ColorsAssigned"];
            Hashtable p = PhotonNetwork.CurrentRoom.CustomProperties;
        }
    }

    public void SetSelfNickname(string text)
    {
        PhotonNetwork.NickName = text;
    }

    public void SetSelfColor(PlayerColor color)
    {
        SelfColor = color;
    }

    // Debug colors (give each player a specific color)

    // Debug method:

    // Hashtable p1props = new();
    //     p1props["Color"] = PlayerColor.Black;
    //     players[0].SetCustomProperties(p1props);

    //     Hashtable p2props = new();
    //     p2props["Color"] = "PlayerColor.White";
    //     players[1].SetCustomProperties(p2props);

    // Normal method:

    // Hashtable p1props = new();
    //     p1props["Color"] = firstIsWhite ? "PlayerColor.White" : "PlayerColor.Black";
    //     players[0].SetCustomProperties(p1props);

    //     Hashtable p2props = new();
    //     p2props["Color"] = firstIsWhite ? "PlayerColor.Black" : "PlayerColor.White";
    //     players[1].SetCustomProperties(p2props);
}