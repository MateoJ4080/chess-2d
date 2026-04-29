using Photon.Pun;
using UnityEngine;

public class MenuSceneUI : MonoBehaviour
{
    public void OnPressPlay()
    {
        UIManager.Instance.HideMenuPanel();
        if (PhotonNetwork.LocalPlayer.NickName == "")
        {
            UIManager.Instance.ShowNicknamePanel();
            return;
        }
        UIManager.Instance.HideNicknamePanel();
        UIManager.Instance.ShowLoadingPanel();
        GameManager.Instance.UpdateGameState(GameManager.GameState.Loading);
        MatchmakingManager.Instance.TryJoinOrCreate();
    }
}
