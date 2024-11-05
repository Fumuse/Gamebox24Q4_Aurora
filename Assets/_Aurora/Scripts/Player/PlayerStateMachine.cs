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
    
    private void Start()
    {
        InputReader = GetComponent<InputReader>();
        Сontroller = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        MainCamera = Camera.main;
        SpriteRenderer = spriteRenderer;
        
        SwitchState(new PlayerMoveState(this));
    }

    protected override void OnEnable()
    {
        base.OnEnable();

    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }
}