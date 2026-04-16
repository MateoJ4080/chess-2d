using TMPro;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerPanelsManager : MonoBehaviour
{
    string _selfText;
    string _opponentText;

    void Start()
    {
        UpdatePanels();
    }

    void UpdatePanels()
    {
        _selfText = $"{PhotonNetwork.LocalPlayer.NickName} ({PhotonNetwork.LocalPlayer.CustomProperties[PlayerProps.Elo]})";
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (!p.IsLocal)
            {
                _opponentText = $"{p.NickName} ({p.CustomProperties[PlayerProps.Elo]})";
            }
        }

        UIManager.Instance.UpdatePlayerPanels(_selfText, _opponentText);
    }
}