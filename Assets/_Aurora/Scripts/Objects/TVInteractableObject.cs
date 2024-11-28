using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TVInteractableObject : MonoBehaviour
{
    [SerializeField] private Light2D tvLight;

    private void Start()
    {
        tvLight.enabled = false;
    }

    public void TurnOnLight()
    {
        tvLight.enabled = true;
    }
}