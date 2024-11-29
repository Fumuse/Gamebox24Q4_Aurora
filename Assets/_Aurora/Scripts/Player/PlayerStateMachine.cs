using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(InputReader))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerStateMachine : StateMachine
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerState playerState;
    [SerializeField] private AudioClip stopStepSound;

    protected override PlayerLoopTiming UpdateYield => PlayerLoopTiming.Update;

    public float MovementSpeed => speed;
    public InputReader InputReader { get; private set; }
    public CharacterController Сontroller { get; private set; }
    public Animator Animator { get; private set; }
    public Camera MainCamera { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }

    public bool InInteract => currentState is PlayerInteractState;

    public Vector2 LookDirection
    {
        get
        {
            if (SpriteRenderer.flipX)
                return Vector2.right;

            return Vector2.left;
        }
    }
    
    protected readonly int fearTriggerAnimParamHash = Animator.StringToHash("PlayerFear");

    private AudioSource _audioSource;

    public void Init()
    {
        InputReader = GetComponent<InputReader>();
        Сontroller = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        MainCamera = Camera.main;
        SpriteRenderer = spriteRenderer;
        _audioSource = GetComponent<AudioSource>();
        
        SwitchState(new PlayerMoveState(this));
    }

    protected override void UpdateLoop()
    {
        base.UpdateLoop();
    }

    public void ChangePlayerSpriteByStage(HouseStageEnum stage)
    {
        spriteRenderer.color = stage == HouseStageEnum.Light ? Color.white : Color.gray;
        
        if (stage == HouseStageEnum.Light && playerState.Light != null)
            spriteRenderer.sprite = playerState.Light;
        if (stage == HouseStageEnum.Dark && playerState.Dark != null)
            spriteRenderer.sprite = playerState.Dark;
        if (stage == HouseStageEnum.Broken && playerState.Broken != null)
        {
            spriteRenderer.sprite = playerState.Broken;
            Animator.SetTrigger(fearTriggerAnimParamHash);
        }
    }

    public void BlockMove()
    {
        SwitchState(null);
    }

    public void UnblockMove()
    {
        SwitchState(new PlayerMoveState(this));
    }

    public void Die()
    {
        SwitchState(null);
    }

    public void StepSound(AudioClip sound)
    {
        if (_audioSource == null) return;
        AmbienceAudioController.Instance.PuffAudio(_audioSource, sound);
    }

    public void StepStopSound()
    {
        // if (_audioSource == null) return;
        // AmbienceAudioController.Instance.PuffAudio(_audioSource, stopStepSound);
    }
}