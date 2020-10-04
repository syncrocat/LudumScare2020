using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseButtonHandler : MonoBehaviour
{
    public GameObject GamerManager;

    public GameObject SfxMuteButton;

    public GameObject MusicMuteButton;

    private SoundManager SoundManager;

    public Image MusicImage;

    public Image SfxImage;
    public Sprite NotMutedMusicIcon;
    public Sprite NotMutedSfxIcon;
    public Sprite MutedMusicIcon;
    public Sprite MutedSfxIcon;

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
            SfxImage.sprite = NotMutedSfxIcon;
            SoundManager.Unmute(MuteProfile.SFX);
        } else
        {
            SfxImage.sprite = MutedSfxIcon;
            SoundManager.Mute(MuteProfile.SFX);
        }
    }

    public void ToggleMuteMusic()
    {
        if (SoundManager.MusicMuted)
        {
            MusicImage.sprite = NotMutedMusicIcon;
            SoundManager.Unmute(MuteProfile.Music);
        } else
        {
            MusicImage.sprite = MutedMusicIcon;
            SoundManager.Mute(MuteProfile.Music);
        }
    }
}
