using System;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PlayerBaseState : State
{
    protected readonly PlayerStateMachine stateMachine;
    protected Vector3 targetPosition;
    protected Vector3 moveDirection;

    private bool _isMoving = false;

    protected bool IsMoving
    {
        get => _isMoving;
        set
        {
            bool prevMoving = _isMoving;
            _isMoving = value;
        }
    }

    protected bool isClickedToUI = false;

    public Action OnEndMove;

    #region Animations
    
    protected readonly int moveAnimParamHash = Animator.StringToHash("MoveSpeed");
    protected readonly int moveAnimBlendTreeHash = Animator.StringToHash("MoveBlendTree");
    protected readonly int moveAnimFearBlendTreeHash = Animator.StringToHash("FearMoveBlendTree");
    
    protected const float AnimationDampTime = 0f;
    protected const float CrossFadeDuration = 0f;

    #endregion
    
    protected PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected void Move()
    {
        if (targetPosition == Vector3.zero || !IsMoving) return;

        FixMoveDirection();

        stateMachine.Сontroller.Move(moveDirection * stateMachine.MovementSpeed * Time.deltaTime);

        if (
            Mathf.Abs(targetPosition.x - stateMachine.transform.position.x) <= .1f ||
            stateMachine.Сontroller.velocity.magnitude < .1f
        )
        {
            IsMoving = false;
            targetPosition = Vector3.zero;
            OnEndMove?.Invoke();
        }
    }

    protected void MoveAnimation()
    {
        stateMachine.Animator.SetFloat(moveAnimParamHash, 
            IsMoving ? 1f : 0f
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
        if (targetPosition == Vector3.zero || !IsMoving) return;
        
        stateMachine.SpriteRenderer.flipX = (targetPosition.x - stateMachine.transform.position.x > 0);
    }

    protected void CheckClickToUI()
    {
        if (EventSystem.current != null)
        {
            isClickedToUI = EventSystem.current.IsPointerOverGameObject();
        }
    }

    protected void SetMovingAnimate()
    {
        if (GameManager.Instance.CurrentStage == HouseStageEnum.Broken)
        {
            stateMachine.Animator.CrossFadeInFixedTime(moveAnimFearBlendTreeHash, CrossFadeDuration);
        }
        else
        {
            stateMachine.Animator.CrossFadeInFixedTime(moveAnimBlendTreeHash, CrossFadeDuration);
        }
    }
}