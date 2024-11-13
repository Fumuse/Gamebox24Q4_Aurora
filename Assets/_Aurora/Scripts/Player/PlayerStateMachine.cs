using UnityEngine;

[RequireComponent(typeof(InputReader))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerStateMachine : StateMachine
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public float MovementSpeed => speed;
    public InputReader InputReader { get; private set; }
    public CharacterController Сontroller { get; private set; }
    public Animator Animator { get; private set; }
    public Camera MainCamera { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }

    public void Init()
    {
        InputReader = GetComponent<InputReader>();
        Сontroller = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        MainCamera = Camera.main;
        SpriteRenderer = spriteRenderer;
        
        SwitchState(new PlayerMoveState(this));
    }

    public void ChangePlayerSpriteByStage(HouseStageEnum stage)
    {
        if (stage == HouseStageEnum.Light)
        {
            spriteRenderer.color = Color.white;
        }
        else
        {
            spriteRenderer.color = Color.gray;
        }
    }
}