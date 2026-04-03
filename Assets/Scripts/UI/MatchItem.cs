using TMPro;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class MatchItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI player1Text;
    [SerializeField] private TextMeshProUGUI player2Text;
    [SerializeField] private TextMeshProUGUI timeText;
    private string _roomName;


    public void SetData(RoomInfo room)
    {
        _roomName = room.Name;
        player1Text.text = $"{GetProp(room, RoomProps.P1Name)} ({GetProp(room, RoomProps.P1Elo)})";
        player2Text.text = $"{GetProp(room, RoomProps.P2Name)} ({GetProp(room, RoomProps.P2Elo)})";
    }

    public string GetProp(RoomInfo room, string key)
    {
        return room.CustomProperties.TryGetValue(key, out object value) ? value.ToString() : "-";
    }

    // Debug
    public void SetData(string p1, string p2, int e1, int e2)
    {
        player1Text.text = $"{p1} ({e1})";
        player2Text.text = $"{p2} ({e2})";
    }

    public void JoinRoomByButton()
    {
        if (!PhotonNetwork.InLobby) return;

        UIManager.Instance.HideMatchList();
        Debug.Log($"JoinRoomByButton: trying joining to {_roomName}");
        PhotonNetwork.JoinRoom(_roomName);
    }
}
