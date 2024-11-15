using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ScreamerView : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    [SerializeField] protected SpriteRenderer[] _spriteRenderers;
    [SerializeField] private float _fateDuration = 3.2f;
    [SerializeField] private bool _FlipXInversion;

    private int _moveAnimationHash = Animator.StringToHash("isMove");
    private Color _defaultColor;
    private Color _transparentColor;
    private float _defaultAlpha = 0.85f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        
        if (_spriteRenderers.Length > 0)
        {
            _defaultColor = _spriteRenderers[0].color;
        }
    }

    private void OnValidate()
    {
        _animator??=GetComponent<Animator>();
        _spriteRenderers ??= GetComponentsInChildren<SpriteRenderer>();
    }

    public void FlipX(Vector2 playerPosition)
    {
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _spriteRenderers[i].flipX = _FlipXInversion ? (playerPosition.x - transform.position.x) > 0 : (transform.position.x - playerPosition.x) > 0;
        }
    }

    public void SetNewColorAlpha(float alpha)
    {
        Color newColor = _defaultColor;
        newColor.a = alpha;
        foreach (SpriteRenderer sprite in _spriteRenderers)
        {
            sprite.color = newColor;
        }
    }

    public async UniTask SetFide(bool visible)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _fateDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = visible ? Mathf.Clamp01(elapsedTime / _fateDuration) * _defaultAlpha : _defaultAlpha - Mathf.Clamp01(elapsedTime / _fateDuration);
            float alpha = newAlpha;
            SetNewColorAlpha(alpha);
            await UniTask.WaitForEndOfFrame();
        }

        SetNewColorAlpha(visible ? _defaultAlpha : 0);
    }

    public void AnimationMove(bool isMove)=>
        _animator.SetBool(_moveAnimationHash, isMove);
}
