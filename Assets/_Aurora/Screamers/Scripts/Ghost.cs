using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Ghost : Screamer
{
    [SerializeField] private float _speedMove;
    [SerializeField] private float _distanceAttack;
    [SerializeField] private UnityEvent EventDeathPlayer;

    private CancellationTokenSource _cts = new();
    private bool _isActivate;
    private bool _readyToMove;
    private Vector2 _defaultPosition;

    private bool _isIdleAnimationEnded = false;

    private TeleportProvider _teleportProvider;

    private void Start()
    {
        _teleportProvider ??= GameProvidersManager.Instance.TeleportProvider;
        _defaultPosition = transform.position;
        _screamerView.SetNewColorAlpha(alpha: 0);
    }

    private void OnEnable()
    {
        if (_cts == null) _cts = new();

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

        Move();
    }

    public override async void Activate(bool activate)
    {
        if (_isActivate) return;

        _isActivate = activate;

        if (_isActivate)
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
        if (_isActivate == false) return;

        _readyToMove = false;
        _isActivate = false;
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
        TransformTranslate();

        if (Distance <= _distanceAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        EventDeathPlayer?.Invoke();
    }

    private void TransformTranslate() => transform.Translate(Direction * Time.deltaTime * _speedMove, Space.World);

    private void OnIdleAnimationEnded()
    {
        _isIdleAnimationEnded = true;
    }

    private void OnPlayerTeleported()
    {
        ResetGhost();
    }
}