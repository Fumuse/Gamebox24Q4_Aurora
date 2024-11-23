using System;
using UnityEngine;

public class PlayerInteractState : PlayerBaseState
{
    private IInteractable _interactable;

    private bool _playerMovingToItem = true;

    public static Action OnPlayerInteracted;
    public static Action OnPlayerExitInteract;

    public PlayerInteractState(PlayerStateMachine stateMachine, IInteractable interactable) : base(stateMachine)
    {
        _interactable = interactable;
    }

    public override void Enter()
    {
        base.OnEndMove += OnEndMoving;
        InteractableObject.OnInteracted += OnInteracted;
        InteractableObject.OnCancelInteract += OnCancelInteract;
        PauseMenuController.OnPauseChanged += OnPauseChanged;
        
        SetupMove();
    }

    private void SetupMove()
    {
        Vector3 offset = _interactable.Offset * GetMoveDirection();
        targetPosition = _interactable.Position - offset;
        
        isMoving = true; 
        
        stateMachine.Animator.CrossFadeInFixedTime(moveAnimBlendTreeHash, CrossFadeDuration);
    }

    private Vector3 GetMoveDirection()
    {
        //справа
        if ((_interactable.Position - stateMachine.transform.position).x > 0)
        {
            return Vector3.right;
        }
        
        //слева
        return Vector3.left;
    }

    public override void Tick()
    {
        CheckClickToUI();
        Rotate();
        Move();

        MoveAnimation();
    }

    public override void Exit()
    {
        base.OnEndMove -= OnEndMoving;
        InteractableObject.OnCancelInteract -= OnCancelInteract;
        InputReader.OnMouseClicked -= OnMouseClicked;
        PauseMenuController.OnPauseChanged -= OnPauseChanged;
        OnPlayerExitInteract?.Invoke();
    }

    private void OnEndMoving()
    {
        InputReader.OnMouseClicked += OnMouseClicked;
        _playerMovingToItem = false;
        _interactable.PreInteract();
    }

    private void OnMouseClicked(Vector2 mousePosition)
    {
        if (isClickedToUI) return;
        
        Collider2D clickedCollider = Physics2D.OverlapPoint(
            stateMachine.MainCamera.ScreenToWorldPoint(mousePosition)
            );
        
        if (clickedCollider != null)
        {
            if (clickedCollider.TryGetComponent(out IInteractable interactable))
            {
                if (interactable.Equals(_interactable))
                {
                    return;
                }

                _interactable.DeclineInteract();
                stateMachine.SwitchState(new PlayerInteractState(stateMachine, interactable));
                return;
            }
        }

        _interactable.DeclineInteract();
        stateMachine.SwitchState(new PlayerMoveState(stateMachine));
    }

    private void OnInteracted(IInteractable interactable)
    {
        if (!interactable.Equals(_interactable)) return;
        
        InputReader.OnMouseClicked -= OnMouseClicked;
        InteractableObject.OnInteracted -= OnInteracted;
        OnPlayerInteracted?.Invoke();
    }

    private void OnCancelInteract(IInteractable interactable)
    {
        InteractableObject.OnCancelInteract -= OnCancelInteract;
        stateMachine.SwitchState(new PlayerMoveState(stateMachine));
    }

    private void OnPauseChanged()
    {
        if (!_playerMovingToItem) return;
        
        if (PauseMenuController.InPause)
        {
            stateMachine.SwitchState(new PlayerMoveState(stateMachine));
        }
    }
}