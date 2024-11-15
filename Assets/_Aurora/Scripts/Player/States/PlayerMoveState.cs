using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine)
    {}

    public override void Enter()
    {
        InputReader.OnMouseClicked += OnMouseClicked;
        
        stateMachine.Animator.CrossFadeInFixedTime(moveAnimBlendTreeHash, CrossFadeDuration);
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
        InputReader.OnMouseClicked -= OnMouseClicked;
    }

    private void OnMouseClicked(Vector2 mousePosition)
    {
        if (isClickedToUI) return;
        
        targetPosition = stateMachine.MainCamera.ScreenToWorldPoint(mousePosition);
        Collider2D clickedCollider = Physics2D.OverlapPoint(targetPosition);

        if (clickedCollider != null)
        {
            if (clickedCollider.TryGetComponent(out IInteractable interactable))
            {
                if (interactable.IsViewed)
                {
                    stateMachine.SwitchState(new PlayerInteractState(stateMachine, interactable));
                    return;
                }
            }
        }

        isMoving = true; 
    }
}