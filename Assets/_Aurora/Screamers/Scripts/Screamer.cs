using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Screamer : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected bool _isActivate;

    protected bool _isShow;
    private float _defaultAlpha = 0.75f;
    private Color _defaultColor;
    private Color _transparentColor;
    private float _fateDuration = 3.2f;

    private void OnValidate()
    {
        _animator ??= GetComponent<Animator>();
        _spriteRenderer??= GetComponent<SpriteRenderer>();

        _defaultColor = _spriteRenderer.color;

        SetNewColor(0);
    }

    protected async UniTask Show()
    {
        _isShow=true;
        await SetFide();
    }

    protected async UniTask Hide()
    {
        _isShow = false;
        await SetFide();
    }

    public abstract void Activate(bool activate);

    private async UniTask SetFide()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _fateDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = _isShow ? Mathf.Clamp01(elapsedTime / _fateDuration) * _defaultAlpha : _defaultAlpha - Mathf.Clamp01(elapsedTime / _fateDuration);
            float alpha = newAlpha;
            SetNewColor(alpha);
            await UniTask.Yield();
        }

        SetNewColor(_isShow ? _defaultAlpha : 0);
    }

    private void SetNewColor(float alpha)
    {
        Color newColor = _defaultColor;
        newColor.a = alpha;
        _spriteRenderer.color = newColor;
    }
}
