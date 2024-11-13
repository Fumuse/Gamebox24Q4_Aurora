using UnityEngine;

[RequireComponent(typeof(Flashlight))]
public class FlashlightInput : MonoBehaviour
{
    [SerializeField] private Flashlight _flashlight;
    private bool _enabled;

    private void OnValidate()
    {
        _flashlight??=GetComponent<Flashlight>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            _enabled=!_enabled;
            _flashlight.SetEnable(_enabled);
        }
    }
}
