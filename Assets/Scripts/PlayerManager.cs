using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI colorInfoText;
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

    public static void AssignRandomColors()
    {
        Player[] players = PhotonNetwork.PlayerList;

        if (players.Length < 2)
        {
            Debug.LogWarning("Not enough players to assign colors!");
            return;
        }

        bool firstIsWhite = Random.value < 0.5f;

        // First color
        ExitGames.Client.Photon.Hashtable p1props = new();
        p1props["Color"] = firstIsWhite ? "White" : "Black";
        players[0].SetCustomProperties(p1props);

        // Opposite color from the first
        ExitGames.Client.Photon.Hashtable p2props = new();
        p2props["Color"] = firstIsWhite ? "Black" : "White";
        players[1].SetCustomProperties(p2props);

        Debug.Log($"Colors: Player1 is {players[0].CustomProperties["Color"]}, Player2 is {players[1].CustomProperties["Color"]}");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Instance.ColorsAreAssigned = true;
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }
}