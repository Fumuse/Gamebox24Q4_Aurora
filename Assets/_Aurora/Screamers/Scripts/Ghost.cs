using UnityEngine;
using UnityEngine.Events;

public class Ghost : Screamer
{
    [SerializeField] private float _speedMove;
    [SerializeField] private float _distanceAttack;
    [SerializeField] private UnityEvent EventDeathPlayer;

    private bool _isActivate;
    private bool _readyToMove;
    private Vector2 _defaultPosition;

    private void Start()
    {
        _defaultPosition = transform.position;
        _screamerView.SetNewColorAlpha(alpha: 0);
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
        _screamerView.AnimationMove(_readyToMove); ;
        _screamerView.SetNewColorAlpha(alpha: 0);
    }

    private void Move()
    {
        TransformTranslate();

        if(Distance <= _distanceAttack)
        {
            Attack();
        }    
    }
    
    private void Attack()
    {
        Debug.Log("Аврора умерла в крепких объятиях призрака");
        EventDeathPlayer?.Invoke();
    }

    private void TransformTranslate()=> transform.Translate(Direction * Time.deltaTime * _speedMove);
}
