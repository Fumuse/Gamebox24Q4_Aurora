using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Slenderman : Screamer
{
    [SerializeField] private float speedMove;
    [SerializeField] private float distanceAttack;

    public static bool IsActive { get; private set; }

    private CancellationTokenSource _cts = new();
    private bool _isActivate;

    public bool IsActivate
    {
        get => _isActivate;
        private set
        {
            _isActivate = value;
            IsActive = _isActivate;
        }
    }
    
    private bool _readyToMove;
    private Vector2 _defaultPosition;

    private bool _playerAttacked = false;
    private bool _isIdleAnimationEnded = false;

    private TeleportProvider _teleportProvider;

    public static Action<PlayerStateMachine> PlayerDeadFromScreamer;

    private void Start()
    {
        _teleportProvider ??= GameProvidersManager.Instance.TeleportProvider;
        _defaultPosition = transform.position;
        _screamerView.SetNewColorAlpha(alpha: 0);
    }

    private void OnEnable()
    {
        _cts = new();
        
        _screamerView.OnIdleAnimationEnded += OnIdleAnimationEnded;
        
        if (_teleportProvider == null)
            _teleportProvider = GameProvidersManager.Instance.TeleportProvider;

        _teleportProvider.OnPlayerTeleported += OnPlayerTeleported;
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        
        _screamerView.OnIdleAnimationEnded -= OnIdleAnimationEnded;
        _teleportProvider.OnPlayerTeleported -= OnPlayerTeleported;
    }

    private void Update()
    {
        if (_readyToMove == false) return;
        if (_playerAttacked) return;

        Move();
    }

    public override async void Activate(bool activate)
    {
        if (IsActivate) return;
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }

        IsActivate = activate;

        if (IsActivate)
        {
            bool isCanceled = await Show().AttachExternalCancellation(_cts.Token).SuppressCancellationThrow();
            if (isCanceled) return;
            
            _readyToMove = true;
            _screamerView.AnimationMove(_readyToMove);
        }
        else
        {
            await Hide();
        }
    }

    public void ResetGhost()
    {
        if (IsActivate == false) return;

        _isIdleAnimationEnded = false;
        _readyToMove = false;
        IsActivate = false;
        transform.position = _defaultPosition;
        _screamerView.AnimationMove(_readyToMove);
        _screamerView.SetNewColorAlpha(alpha: 0);
        gameObject.SetActive(false);
    }

    protected override async UniTask Show()
    {
        bool isCanceled = await base.Show().AttachExternalCancellation(_cts.Token).SuppressCancellationThrow();
        if (isCanceled) return;
        
        await UniTask.WaitUntil(() => _isIdleAnimationEnded, 
            cancellationToken: _cts.Token).SuppressCancellationThrow();
    }

    private void Move()
    {
        FlipX();
        TransformTranslate();

        if (Distance <= distanceAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        _playerAttacked = true;
        PlayerDeadFromScreamer?.Invoke(Player);
    }

    private void TransformTranslate() => transform.Translate(Direction * Time.deltaTime * speedMove, Space.World);

    private void OnPlayerTeleported()
    {
        ResetGhost();
    }
    
    private void OnIdleAnimationEnded()
    {
        _isIdleAnimationEnded = true;
    }
}