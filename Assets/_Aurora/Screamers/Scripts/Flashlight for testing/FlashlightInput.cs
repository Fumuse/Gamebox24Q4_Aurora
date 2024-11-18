using UnityEngine;

[RequireComponent(typeof(FlashlightTEST))]
public class FlashlightInput : MonoBehaviour
{
    [SerializeField] private FlashlightTEST _flashlight;
    private bool _enabled;

    private void OnValidate()
    {
        _flashlight??=GetComponent<FlashlightTEST>();
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
