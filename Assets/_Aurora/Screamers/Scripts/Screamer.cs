using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(ScreamerView))]
public abstract class Screamer : MonoBehaviour
{
    [SerializeField] protected ScreamerEnum screamerType;
    [SerializeField] protected ScreamerView _screamerView;
    [SerializeField, HideInInspector] private Transform _playerTransform;
    
    public ScreamerEnum ScreamerType => screamerType;

    protected bool _isShow;

    protected Vector2 PlayerPosition => _playerTransform.position;

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

    public abstract void Activate(bool activate);

    protected virtual async UniTask Show()
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        _isShow = true;
        await _screamerView.SetFade(_isShow);
    }

    protected async UniTask Hide()
    {
        _isShow = false;
        await _screamerView.SetFade(_isShow);
        
        gameObject.SetActive(false);
    }

    protected void FlipX()
    {
        _screamerView.FlipX(_playerTransform.position);
    }
}