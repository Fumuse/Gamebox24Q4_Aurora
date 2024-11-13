using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    private InputSystem_Actions _isActions;
    public Vector2 MousePosition { get; private set; }

    public Action OnEscClicked;

    public delegate void RightMouseClicked(Vector2 mousePosition);
    public static event RightMouseClicked OnRightMouseClicked;

    public delegate void MouseClicked(Vector2 mousePosition);
    public static event MouseClicked OnMouseClicked;

    public void Init()
    {
        _isActions = new();
        
        _isActions.Enable();
        _isActions.Player.Click.performed += OnMouseClick;
        _isActions.Player.RightClick.performed += OnMouseRightClick;
        _isActions.Player.Point.performed += OnMouseMove;
    }

    private void OnDisable()
    {
        _isActions.Disable();

        _isActions.Player.Click.performed -= OnMouseClick;
        _isActions.Player.RightClick.performed -= OnMouseRightClick;
        _isActions.Player.Point.performed -= OnMouseMove;
    }

    private void OnMouseClick(InputAction.CallbackContext context)
    {
        if (!IsMouseInCameraView(Mouse.current.position.value)) return;
        OnMouseClicked?.Invoke(Mouse.current.position.value);
    }
    
    private bool IsMouseInCameraView(Vector3 mousePosition)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        if (mousePosition.x >= 0 && mousePosition.x <= screenWidth && mousePosition.y >= 0 && mousePosition.y <= screenHeight)
        {
            return true;
        }
        return false;
    }

    private void OnMouseMove(InputAction.CallbackContext context)
    {
        MousePosition = context.ReadValue<Vector2>();
    }

    private void OnMouseRightClick(InputAction.CallbackContext context)
    {
        if (!IsMouseInCameraView(Mouse.current.position.value)) return;
        OnRightMouseClicked?.Invoke(Mouse.current.position.value);
    }
}