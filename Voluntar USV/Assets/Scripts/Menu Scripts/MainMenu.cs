using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsPannel;
    public GameObject menuPannel;

    public void LoadGameScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void OpenSettings()
    {
        menuPannel.SetActive(false);
        settingsPannel.SetActive(true);
    }
    public void CloseSettings()
    {
        settingsPannel.SetActive(false);
        menuPannel.SetActive(true);
    }
    public void ExitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
        Application.OpenURL(webplayerQuitURL);
#else
        Application.Quit();
#endif
    }
}
