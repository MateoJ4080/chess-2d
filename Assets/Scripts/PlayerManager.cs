using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    void Awake()
    {
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

        bool firstIsWhite = Random.value < 0.5f;

        ExitGames.Client.Photon.Hashtable p1props = new();
        p1props["Color"] = firstIsWhite ? "White" : "Black";
        players[0].SetCustomProperties(p1props);

        ExitGames.Client.Photon.Hashtable p2props = new();
        p2props["Color"] = firstIsWhite ? "White" : "Black";
        players[1].SetCustomProperties(p2props);
    }
}
