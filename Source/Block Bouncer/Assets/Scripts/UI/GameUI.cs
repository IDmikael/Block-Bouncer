using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    private MainMenuUI menuUI = null;

    private void Awake()
    {
        // Handling game pause
        MainMenuUI.OnGamePause += MainMenuUI_OnGamePause;
    }

    public void OnBtnPausePressed()
    {
        SoundHelper.PlayButtonSound();

        menuUI.SwitchPauseMode(true);
    }

    private void MainMenuUI_OnGamePause(bool isPaused)
    {
        if (!isPaused)
        {
            gameObject.SetActive(true);
            GetComponent<Animator>().Play("GameUIFadeIn");
        }
        else
            gameObject.SetActive(false);
    }
}
