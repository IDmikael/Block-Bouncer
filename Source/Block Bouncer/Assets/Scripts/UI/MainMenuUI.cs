using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    // Animation objects
    private Animator anim;
    private string[] animationsNames = new string[] { "MainMenuFadeIn", "MainMenuFadeOut" };

    // UI objects
    [SerializeField]
    private Text title = null;
    [SerializeField]
    private GameObject modeObjects = null;
    [SerializeField]
    private GameObject[] modeIcons = null;

    [SerializeField]
    private PlayerController playerController = null;

    // Game pause trigger
    public bool isPaused = true;

    public static event Action<bool> OnGamePause = delegate { };

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        SwitchPauseMode(true); // Set game on pause while menu is shown
        ChangeControlMode(); // Set current control mode according to player prefs
    }

    public void SwitchPauseMode(bool isPaused)
    {
        this.isPaused = isPaused;
        OnGamePause(isPaused);
        Time.timeScale = isPaused ? 0 : 1;

        if (isPaused)
        {
            gameObject.SetActive(true);
            anim.Play(animationsNames[0]);
        }        
    }

    // Animation end event
    public void OnFadeOutAnimEnd()
    {
        gameObject.SetActive(false);

        title.text = isPaused ?  "BLOCK\nBOUNCER" : "PAUSED";
        modeObjects.SetActive(isPaused);
    }

    private void ChangeControlMode()
    {
        string type = PlayerPrefs.GetString(PrefsNames.PREF_CONTROL_TYPE, PrefsNames.modeNames[0]);

        playerController.ChangeControlType(type);

        // Switch mode button icons according to current control type
        if (type == PrefsNames.modeNames[0])
        {
            modeIcons[0].SetActive(true);
            modeIcons[1].SetActive(false);
        }
        else if (type == PrefsNames.modeNames[1])
        {
            modeIcons[0].SetActive(false);
            modeIcons[1].SetActive(true);
        }
    }


    // BUTTONS CLICK HANDLERS
    public void OnBtnPlayPressed()
    {
        SoundHelper.PlayButtonSound();

        if (isPaused)
            SwitchPauseMode(false);

        anim.Play(animationsNames[1]);
    }

    public void OnBtnModePressed()
    {
        SoundHelper.PlayButtonSound();

        string type = PlayerPrefs.GetString(PrefsNames.PREF_CONTROL_TYPE, PrefsNames.modeNames[0]);
        type = type == PrefsNames.modeNames[0] ? PrefsNames.modeNames[1] : PrefsNames.modeNames[0];

        PlayerPrefs.SetString(PrefsNames.PREF_CONTROL_TYPE, type);
        ChangeControlMode();
    }

    public void OnBtnExitPressed()
    {
        SoundHelper.PlayButtonSound();

        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                   .GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call("finish");
        }
        else
            Application.Quit();
    }
}
