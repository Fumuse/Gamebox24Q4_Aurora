using Cysharp.Threading.Tasks;
using UnityEngine;

public class Ghost : Screamer
{
    [SerializeField] private float _speedMove;
    [SerializeField] private float _distanceAttack;

    private bool _readyToMove;
    private Transform _playerTransform;
    private Vector2 _defaultPosition;

    private void Start()
    {
        _playerTransform = FindFirstObjectByType<PlayerStateMachine>().transform;
        _defaultPosition = transform.position;
    }

    private void OnValidate()
    {
        Activate(_isActivate);
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
            await Show();
            _readyToMove = true;
        }
        else
        {
            await Hide();
        }
    }

    private void ResetGhost()
    {
        _readyToMove = false;
        _isActivate = false;
        transform.position = _defaultPosition;
        UniTask task = Hide();
    }

    private void Move()
    {
        Vector3 positionPlayer = _playerTransform.position;
        positionPlayer.y = 0;

        Vector2 direction = (positionPlayer - transform.position).normalized;
        transform.Translate(direction * Time.deltaTime * _speedMove);

        float distance = Vector2.Distance(transform.position, positionPlayer);

        if (distance > 10)
        {
            ResetGhost();
        }
        

        if(Vector2.Distance(transform.position, positionPlayer) <= _distanceAttack)
        {
            Attack();
        }    
    }
    
    private void Attack()
    {
        Debug.Log("death");
    }
}
