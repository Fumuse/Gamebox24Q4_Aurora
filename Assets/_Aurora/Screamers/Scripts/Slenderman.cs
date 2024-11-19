﻿using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Slenderman : Screamer
{
    [SerializeField] private float speedMove;
    [SerializeField] private float distanceAttack;
    [SerializeField] private UnityEvent eventDeathPlayer;

    private CancellationTokenSource _cts = new();
    private bool _isActivate;
    private bool _readyToMove;
    private Vector2 _defaultPosition;

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
        
        if (_teleportProvider == null)
            _teleportProvider = GameProvidersManager.Instance.TeleportProvider;

        _teleportProvider.OnPlayerTeleported += OnPlayerTeleported;
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        
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
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }

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
        await base.Show().AttachExternalCancellation(_cts.Token).SuppressCancellationThrow();
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
        Debug.Log("Аврора умерла в крепких объятиях призрака");
        eventDeathPlayer?.Invoke();
    }

    private void TransformTranslate() => transform.Translate(Direction * Time.deltaTime * speedMove, Space.World);

    private void OnPlayerTeleported()
    {
        ResetGhost();
    }
}