using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ScreamerView : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    [SerializeField] protected SpriteRenderer[] _spriteRenderers;
    [SerializeField] protected SpriteRenderer renderForFullBody;
    [SerializeField] private float fadeDuration = 3.2f;
    [SerializeField] private bool _FlipXInversion;

    private int _moveAnimationHash = Animator.StringToHash("isMove");
    private Color _defaultColor;
    private Color _transparentColor;
    private float _defaultAlpha = 0.85f;

    private CancellationTokenSource _cts = new();

    public Action OnIdleAnimationEnded;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        if (_spriteRenderers.Length > 0)
        {
            _defaultColor = _spriteRenderers[0].color;
        }
    }

    private void OnEnable()
    {
        _cts = new();

        ChangeSpritesVisible(false);
    }

    private void OnDisable()
    {
        _cts?.Cancel();
    }

    private void OnValidate()
    {
        _animator ??= GetComponent<Animator>();
        _spriteRenderers ??= GetComponentsInChildren<SpriteRenderer>();
    }

    public void FlipX(Vector2 playerPosition)
    {
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _spriteRenderers[i].flipX = _FlipXInversion
                ? (playerPosition.x - transform.position.x) > 0
                : (transform.position.x - playerPosition.x) > 0;
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

        if (renderForFullBody != null)
        {
            renderForFullBody.color = newColor;
        }
    }

    public async UniTask SetFade(bool visible)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            bool isCanceled = await UniTask.WaitForEndOfFrame(this, _cts.Token).SuppressCancellationThrow();
            if (isCanceled) return;
            
            elapsedTime += Time.deltaTime;
            float newAlpha = visible ? Mathf.Clamp01(elapsedTime / fadeDuration) * _defaultAlpha
                : _defaultAlpha - Mathf.Clamp01(elapsedTime / fadeDuration);
            float alpha = newAlpha;
            SetNewColorAlpha(alpha);
        }

        SetNewColorAlpha(visible ? _defaultAlpha : 0);
    }

    public void AnimationMove(bool isMove)
    {
        _animator.SetBool(_moveAnimationHash, isMove);
        ChangeSpritesVisible(isMove);
    }

    private void ChangeSpritesVisible(bool isMove)
    {
        if (renderForFullBody != null)
        {
            renderForFullBody.gameObject.SetActive(!isMove);
            foreach (SpriteRenderer render in _spriteRenderers)
            {
                render.gameObject.SetActive(isMove);
            }
        }
    }

    public void IdleAnimationEnded()
    {
        OnIdleAnimationEnded?.Invoke();
    }

    public void StepSound(AudioClip sound)
    {
        AmbienceAudioController.Instance.PuffAudio(sound);
    }
}