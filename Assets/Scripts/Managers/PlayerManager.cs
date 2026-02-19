using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    private bool colorsAreAssigned;
    public bool ColorsAreAssigned
    {
        get => colorsAreAssigned;
        private set => colorsAreAssigned = value;
    }

    private bool playerIsBlack;
    public bool PlayerIsBlack
    {
        get => playerIsBlack;
        set => playerIsBlack = value;
    }

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

    // Check if colors are assigned by looking at room properties
    public bool CheckColorsAssigned()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("ColorsAssigned"))
        {
            colorsAreAssigned = (bool)PhotonNetwork.CurrentRoom.CustomProperties["ColorsAssigned"];
            return colorsAreAssigned;
        }
        return false;
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

        Debug.Log("Assigning colors to players...");
        bool firstIsWhite = Random.value < 0.5f;

        Hashtable p1props = new();
        p1props["Color"] = firstIsWhite ? "White" : "Black";
        players[0].SetCustomProperties(p1props);

        Hashtable p2props = new();
        p2props["Color"] = firstIsWhite ? "Black" : "White";
        players[1].SetCustomProperties(p2props);

        // Set room property to indicate colors are assigned
        Hashtable roomProps = new();
        roomProps["ColorsAssigned"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);

        Instance.ColorsAreAssigned = true;
    }

    // Called when room properties change
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("ColorsAssigned"))
        {
            colorsAreAssigned = (bool)propertiesThatChanged["ColorsAssigned"];
            Debug.Log($"Colors assignment status updated: {colorsAreAssigned}");
        }
    }
}