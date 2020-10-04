using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtonHandler : MonoBehaviour
{
    public GameObject GamerManager;

    public GameObject SfxMuteButton;

    public GameObject MusicMuteButton;

    private SoundManager SoundManager;

    public void Start()
    {
        SoundManager = FindObjectOfType<SoundManager>();
    }
    // Start is called before the first frame update
    public void Pause()
    {
        GamerManager.GetComponent<GamerManager>().Pause();
    }

    public void Unpause()
    {
        GamerManager.GetComponent<GamerManager>().Unpause();
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    public void ToggleMuteSfx()
    {
        if (SoundManager.SfxMuted)
        {
            SoundManager.Unmute(MuteProfile.SFX);
        } else
        {
            SoundManager.Mute(MuteProfile.SFX);
        }
    }

    public void ToggleMuteMusic()
    {
        if (SoundManager.MusicMuted)
        {
            SoundManager.Unmute(MuteProfile.Music);
        } else
        {
            SoundManager.Mute(MuteProfile.Music);
        }
    }
}
