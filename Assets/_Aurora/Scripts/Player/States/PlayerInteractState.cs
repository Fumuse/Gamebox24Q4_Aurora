using UnityEngine;

public class PlayerInteractState : PlayerBaseState
{
    private IInteractable _interactable;

    public PlayerInteractState(PlayerStateMachine stateMachine, IInteractable interactable) : base(stateMachine)
    {
        _interactable = interactable;
    }

    public override void Enter()
    {
        base.OnEndMove += OnEndMoving;
        InteractableObject.OnInteracted += OnInteracted;
        InteractableObject.OnCancelInteract += OnCancelInteract;
        
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
        InputReader.OnMouseClicked -= OnMouseClicked;
    }

    private void OnEndMoving()
    {
        InputReader.OnMouseClicked += OnMouseClicked;
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
    }

    private void OnCancelInteract(IInteractable interactable)
    {
        InteractableObject.OnCancelInteract -= OnCancelInteract;
        stateMachine.SwitchState(new PlayerMoveState(stateMachine));
    }
}