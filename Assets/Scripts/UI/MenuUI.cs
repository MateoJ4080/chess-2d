using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
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

    public void BackToMenu()
    {
        PhotonNetwork.LeaveRoom();

        GameManager.Instance.UpdateGameState(GameManager.GameState.MainMenu);
        SceneManager.LoadScene("Menu");
    }
}
