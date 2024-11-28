using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(ScreamerView))]
public abstract class Screamer : MonoBehaviour
{
    [SerializeField] protected ScreamerEnum screamerType;
    [SerializeField] protected ScreamerView _screamerView;
    [SerializeField, HideInInspector] private Transform _playerTransform;

    [SerializeField] private AudioSource screamerAudioSource;

    private PlayerStateMachine _player;
    private CancellationTokenSource _cts = new();
    
    public ScreamerEnum ScreamerType => screamerType;

    protected bool _isShow;

    protected Vector2 PlayerPosition => _playerTransform.position;
    protected PlayerStateMachine Player => _player;

    protected Vector2 Direction
    {
        get
        {
            Vector2 direction = (_playerTransform.position - transform.position).normalized;
            direction.y = 0;
            
            return direction;
        }
    }
    protected float Distance => Vector2.Distance(transform.position, _playerTransform.position);

    private void OnValidate()
    {
        if (_playerTransform == null)
        {
            PlayerStateMachine player = FindFirstObjectByType<PlayerStateMachine>();
            if (player != null)
            {
                _playerTransform = player.transform;
            }
        }

        _screamerView ??= GetComponent<ScreamerView>();
    }

    private void OnEnable()
    {
        _cts = new();
    }

    private void OnDisable()
    {
        _cts?.Cancel();
    }

    protected virtual void Awake()
    {
        _player = _playerTransform.GetComponent<PlayerStateMachine>();
    }

    public abstract void Activate(bool activate);

    protected virtual async UniTask Show()
    {
        FlipX();
        
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        _isShow = true;
        await _screamerView.SetFade(_isShow).AttachExternalCancellation(_cts.Token).SuppressCancellationThrow();
    }

    protected async UniTask Hide()
    {
        _isShow = false;
        bool isCanceled = await _screamerView.SetFade(_isShow).AttachExternalCancellation(_cts.Token).SuppressCancellationThrow();
        if (isCanceled) return;

        gameObject.SetActive(false);
    }

    protected void FlipX()
    {
        _screamerView.FlipX(_playerTransform.position);
    }

    protected void StartAmbience()
    {
        screamerAudioSource.Play();
    }

    protected void StopAmbience()
    {
        screamerAudioSource.Stop();
    }
}