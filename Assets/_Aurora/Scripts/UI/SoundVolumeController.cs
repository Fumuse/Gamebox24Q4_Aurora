using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundVolumeController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private AudioMixerGroup audioMixerGroup;

    private void OnValidate()
    {
        slider ??= GetComponentInChildren<Slider>();
    }

    private void Start()
    {
        Load();
    }

    public void ChangeVolume(float volumeSliderValue)
    {
        audioMixerGroup.audioMixer.SetFloat($"{audioMixerGroup.name}Volume", Mathf.Log10(volumeSliderValue) * 20);
        Save(volumeSliderValue);
    }

    private void Load()
    {
        float playerVolume = PlayerPrefs.GetFloat($"{audioMixerGroup.name}_volume", 1f);
        ChangeVolume(playerVolume);
        slider.value = playerVolume;
    }

    private void Save(float volumeSliderValue)
    {
        PlayerPrefs.SetFloat($"{audioMixerGroup.name}_volume", volumeSliderValue);
    }
}
