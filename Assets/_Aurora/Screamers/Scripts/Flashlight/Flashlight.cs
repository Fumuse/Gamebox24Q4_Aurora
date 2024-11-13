using System;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public static event Action<bool> LightState;

    [SerializeField] private SpriteMask _mask;
    [SerializeField] private bool _enabled = false;

    private void Start()
    {
        Initialize();
    }

    private void OnValidate()
    {
        Initialize();
        SetEnableComponents(_enabled);
    }

    private void Update()
    {
        if(_enabled == false) return;

        Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = position;  
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    private void Initialize()
    {
        _mask ??= GetComponentInChildren<SpriteMask>();
    }

    public void SetEnable(bool state)
    {
        SetEnableComponents(state);
    }

    public void On()
    {
        SetEnableComponents(true);
    }

    public void Off()
    {
        SetEnableComponents(false);
    }

    private void SetEnableComponents(bool enable)
    {
        _enabled = enable;
        _mask.enabled = enable;

        LightState?.Invoke(enable);
    }
}
