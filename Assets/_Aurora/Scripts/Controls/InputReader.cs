using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    private InputSystem_Actions _isActions;
    public Vector2 MousePosition { get; private set; }

    public Action OnEscClicked;

    public delegate void MouseClicked(Vector2 mousePosition);
    public static event MouseClicked OnMouseClicked;

    private void Awake()
    {
        _isActions = new();
    }

    private void OnEnable()
    {
        _isActions.Enable();

        _isActions.Player.Click.performed += OnMouseClick;
    }

    private void OnDisable()
    {
        _isActions.Disable();

        _isActions.Player.Click.performed -= OnMouseClick;
    }

    private void OnMouseClick(InputAction.CallbackContext context)
    {
        OnMouseClicked?.Invoke(Mouse.current.position.value);
    }
}