using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public List<Sound> Sounds;

    public bool SfxMuted = false;

    public bool MusicMuted = false;

    private static SoundManager instance;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Object.Destroy(gameObject);
        }

        foreach (var sound in Sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            if (sound.playOnAwake)
            {
                sound.source.Play();
            }
        }
    }

    public void Play(string name)
    {
        var sound = Sounds.First(s => s.name.Equals(name));
        sound.source.Play();
    }

    public void Mute(MuteProfile profile)
    {
        if (profile == MuteProfile.Music)
        {
            MusicMuted = true;
        }

        if (profile == MuteProfile.SFX)
        {
            SfxMuted = true;
        }

        foreach (var sound in Sounds.Where(s => s.muteProfile == profile))
        {
            sound.source.mute = true;
        }
    }

    public void Unmute(MuteProfile profile)
    {
        if (profile == MuteProfile.Music)
        {
            MusicMuted = false;
        }

        if (profile == MuteProfile.SFX)
        {
            SfxMuted = false;
        }

        foreach (var sound in Sounds.Where(s => s.muteProfile == profile))
        {
            sound.source.mute = false;
        }
    }
}

public enum MuteProfile
{
    SFX,
    Music,
}

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;

    public string name;

    public bool playOnAwake;

    public bool loop;

    public MuteProfile muteProfile;

    [HideInInspector]
    public AudioSource source;
}