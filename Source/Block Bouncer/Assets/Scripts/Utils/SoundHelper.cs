using UnityEngine;

// Android sounds handling
public class SoundHelper : MonoBehaviour
{
    private static int buttonSoundID;
    private static int musicID;

    private void Start()
    {
        musicID = ANAMusic.load("Android Native Audio/Music Native.ogg");
        PlayBackgroundMusic();

        InitNativeAudio();
    }

    // To prevent music from playing after app's pause
    private void OnApplicationPause(bool pause)
    {
        if (pause)
            StopBackgroundMusic();
        else
            PlayBackgroundMusic();
    }

    private void InitNativeAudio()
    {
        AndroidNativeAudio.makePool(16);

        buttonSoundID = AndroidNativeAudio.load("Android Native Audio/button.ogg");
    }

    //Play sounds
    public static void PlayButtonSound()
    {
        AndroidNativeAudio.play(buttonSoundID);
    }

    public static void PlayBackgroundMusic()
    {

        ANAMusic.setLooping(musicID, true);
        ANAMusic.setVolume(musicID, 0.5f);
        ANAMusic.play(musicID);
    }

    //Stop sounds
    public static void StopBackgroundMusic()
    {
        if (ANAMusic.isPlaying(musicID))
            ANAMusic.pause(musicID);
    }
}
