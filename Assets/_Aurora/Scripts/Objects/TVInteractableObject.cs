using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(AudioSource))]
public class TVInteractableObject : MonoBehaviour
{
    [SerializeField] private Light2D tvLight;
    [SerializeField] private AudioClip tvOnSound;

    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        tvLight.enabled = false;
    }

    public void TurnOnLight()
    {
        tvLight.enabled = true;
        AmbienceAudioController.Instance.PuffAudio(_audioSource, tvOnSound);
    }
}