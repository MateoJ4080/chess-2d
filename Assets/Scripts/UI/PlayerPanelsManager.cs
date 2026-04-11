using TMPro;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerPanelsManager : MonoBehaviour
{
    string _selfName;
    string _selfElo;
    string _opponentName;
    string _opponentElo;

    void Start()
    {
        UpdatePanels();
    }

    void UpdatePanels()
    {
        _selfName = PhotonNetwork.LocalPlayer.NickName;
        _selfElo = $"({PhotonNetwork.LocalPlayer.CustomProperties[PlayerProps.Elo]})";
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (!p.IsLocal)
            {
                _opponentName = p.NickName;
                _opponentElo = $"({p.CustomProperties[PlayerProps.Elo]})";
            }
        }

        UIManager.Instance.UpdatePlayerPanels(_selfName, _selfElo, _opponentName, _opponentElo);
    }
}