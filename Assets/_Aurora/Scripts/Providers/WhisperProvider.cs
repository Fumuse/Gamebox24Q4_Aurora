using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class WhisperProvider : MonoBehaviour, IAction
{
    [SerializeField] private CanvasGroup whisperWrapper;
    [SerializeField] private TextMeshProUGUI whisperText;
    [SerializeField] private LocalizedString whisperLocalization;
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private float timeToWaitToReadGlobal = 1f;
    [SerializeField] private float timeToWaitToReadOneChar = 0.05f;

    private IInteractable _lastInteractable;
    private ActionSettings _actionSettings;
    private CancellationTokenSource _cts;

    private int _localizedTextCharsCount = 1;

    public Action OnWhisperEnds;

    private void OnEnable()
    {
        _cts = new();
        InteractableObject.OnInteracted += OnInteracted;
        whisperLocalization.StringChanged += UpdateWhisperText;
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        InteractableObject.OnInteracted -= OnInteracted;
        whisperLocalization.StringChanged -= UpdateWhisperText;
    }

    public void Execute(ActionSettings settings)
    {
        _cts?.Cancel();
        _cts = new();
        
        _actionSettings = settings;
        ToWhisper();
    }

    private async void ToWhisper()
    {
        SetWhisperLocalizationString();
        
        bool isCanceled = await whisperWrapper.FadeIn(this, _cts.Token, fadeSpeed);
        if (isCanceled) return;

        float timeToWaitToRead = (_localizedTextCharsCount * timeToWaitToReadOneChar) + timeToWaitToReadGlobal;
        isCanceled = await UniTask.WaitForSeconds(timeToWaitToRead, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
        if (isCanceled) return;

        isCanceled = await whisperWrapper.FadeOut(this, _cts.Token, fadeSpeed);
        if (isCanceled) return;

        OnWhisperEnds?.Invoke();
        if (_lastInteractable != null)
        {
            if (_actionSettings != null)
            {
                this.AfterInteractChanges(_lastInteractable, _actionSettings);
            }
            _lastInteractable.FinishInteract();
        }
    }

    private void SetWhisperLocalizationString()
    {
        if (_actionSettings.WhisperText == null) return;

        string whisperText = _actionSettings.WhisperText.GetLocalizedString();
        UpdateWhisperText(whisperText);

        _localizedTextCharsCount = whisperText.Length;
    }

    private void OnInteracted(IInteractable interactable)
    {
        _lastInteractable = interactable;
    }

    private void UpdateWhisperText(string text)
    {
        whisperText.text = text;
    }
}