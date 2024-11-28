using System;
using UnityEngine;

public class AmbienceAudioController : Singleton<AmbienceAudioController>
{
    [SerializeField] private AudioLibraryAsset audioAssets;
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource someSoundsAudioSource;
    [SerializeField] private AudioSource uiAudioSource;

    public void PuffAudio(AudioSource source, AudioClip audioClip)
    {
        if (audioClip == null) return;
        source.clip = audioClip;
        source.Play();
    }

    public void PuffAudio(AudioClip audioClip)
    {
        PuffAudio(someSoundsAudioSource, audioClip);
    }
    
    public void PuffAudio(string audioCategory, string audioName)
    {
        AudioClip clip = audioAssets[audioCategory, audioName];
        if (clip != null)
        {
            PuffAudio(clip);
        }
    }

    public void PuffUIAudio(string audioCategory, string audioName)
    {
        PuffAudio(uiAudioSource, audioCategory, audioName);
    }

    public void PuffAudio(AudioSource source, string audioCategory, string audioName)
    {
        AudioClip clip = audioAssets[audioCategory, audioName];
        if (clip != null)
        {
            PuffAudio(source, clip);
        }
    }

    public void StartPlayBackgroundMusic()
    {
        backgroundAudioSource.Play();
    }

    public void PausePlayBackgroundMusic()
    {
        backgroundAudioSource.Pause();
    }

    public void ChangeBackgroundMusicByStage(HouseStageEnum houseStage)
    {
        bool isPlaying = backgroundAudioSource.isPlaying;
        
        AudioClip clip = audioAssets["BackgroundMusic", houseStage.ToString()];
        if (clip != null)
        {
            backgroundAudioSource.clip = clip;
            if (isPlaying)
            {
                StartPlayBackgroundMusic();
            }
        }
    }
}