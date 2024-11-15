using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(ScreamerView))]
public abstract class Screamer : MonoBehaviour
{
    [SerializeField] protected ScreamerView _screamerView;
    private Transform _playerTransform;
   
    protected bool _isShow;

    protected Vector2 PlayerPosition => _playerTransform.position;
    protected Vector2 Direction => (_playerTransform.position - transform.position).normalized;
    protected float Distance => Vector2.Distance(transform.position, _playerTransform.position);

    private void Start()
    {
        _playerTransform = FindFirstObjectByType<PlayerStateMachine>().transform;
    }

    private void OnValidate()
    {
        _playerTransform ??= FindFirstObjectByType<PlayerStateMachine>().transform;
        _screamerView ??= GetComponent<ScreamerView>();
    }

    public abstract void Activate(bool activate);

    protected async UniTask Show()
    {
        _isShow = true;
        await _screamerView.SetFide(_isShow);
    }

    protected async UniTask Hide()
    {
        _isShow = false;
        await  _screamerView.SetFide(_isShow);
    }
   
    protected void FlipX()
    {
        _screamerView.FlipX(_playerTransform.position);
    }
}
