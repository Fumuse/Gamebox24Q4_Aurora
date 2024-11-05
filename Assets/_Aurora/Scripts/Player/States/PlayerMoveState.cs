using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    private readonly int _moveAnimParamHash = Animator.StringToHash("MoveSpeed");
    private readonly int _moveAnimBlendTreeHash = Animator.StringToHash("MoveBlendTree");
    
    private const float AnimationDampTime = 0.1f;
    private const float CrossFadeDuration = 0.1f;
    
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine)
    {}

    public override void Enter()
    {
        InputReader.OnMouseClicked += OnMouseClicked;
        
        stateMachine.Animator.CrossFadeInFixedTime(_moveAnimBlendTreeHash, CrossFadeDuration);
    }

    public override void Tick()
    {
        Rotate();
        Move();

        stateMachine.Animator.SetFloat(_moveAnimParamHash, 
            isMoving ? 1f : 0f, 
            AnimationDampTime, 
            Time.deltaTime
        );
    }

    public override void Exit()
    {
        InputReader.OnMouseClicked -= OnMouseClicked;
    }

    private void OnMouseClicked(Vector2 mousePosition)
    {
        targetPosition = stateMachine.MainCamera.ScreenToWorldPoint(mousePosition);

        isMoving = true;
    }
}