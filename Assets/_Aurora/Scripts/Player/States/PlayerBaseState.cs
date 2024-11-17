using System;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PlayerBaseState : State
{
    protected readonly PlayerStateMachine stateMachine;
    protected Vector3 targetPosition;
    protected Vector3 moveDirection;

    protected bool isMoving = false;
    protected bool isClickedToUI = false;

    public Action OnEndMove;

    #region Animations
    
    protected readonly int moveAnimParamHash = Animator.StringToHash("MoveSpeed");
    protected readonly int moveAnimBlendTreeHash = Animator.StringToHash("MoveBlendTree");
    
    protected const float AnimationDampTime = 0f;
    protected const float CrossFadeDuration = 0f;

    #endregion
    
    protected PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected void Move()
    {
        if (targetPosition == Vector3.zero || !isMoving) return;

        FixMoveDirection();

        stateMachine.Сontroller.Move(moveDirection * stateMachine.MovementSpeed * Time.deltaTime);

        if (
            Mathf.Abs(targetPosition.x - stateMachine.transform.position.x) <= .1f ||
            stateMachine.Сontroller.velocity.magnitude < .1f
        )
        {
            isMoving = false;
            targetPosition = Vector3.zero;
            OnEndMove?.Invoke();
        }
    }

    protected void MoveAnimation()
    {
        stateMachine.Animator.SetFloat(moveAnimParamHash, 
            isMoving ? 1f : 0f, 
            AnimationDampTime, 
            Time.deltaTime
        );
    }

    private void FixMoveDirection()
    {        
        moveDirection = (targetPosition - stateMachine.transform.position).normalized;
        moveDirection.x = Mathf.Sign(moveDirection.x);
        moveDirection.y = 0;
        moveDirection.z = 0;
    }

    protected void Rotate()
    {
        if (targetPosition == Vector3.zero || !isMoving) return;
        
        stateMachine.SpriteRenderer.flipX = (targetPosition.x - stateMachine.transform.position.x > 0);
    }

    protected void CheckClickToUI()
    {
        if (EventSystem.current != null)
        {
            isClickedToUI = EventSystem.current.IsPointerOverGameObject();
        }
    }
}