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
    [SerializeField] private float timeToWaitToReadText = 2f;

    private IInteractable _lastInteractable;
    private ActionSettings _actionSettings;
    private CancellationTokenSource _cts;

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
        _actionSettings = settings;
        ToWhisper();
    }

    private async void ToWhisper()
    {
        SetWhisperLocalizationString();
        
        bool isCanceled = await whisperWrapper.FadeIn(this, _cts.Token, fadeSpeed);
        if (isCanceled) return;

        isCanceled = await UniTask.WaitForSeconds(timeToWaitToReadText, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
        if (isCanceled) return;

        isCanceled = await whisperWrapper.FadeOut(this, _cts.Token, fadeSpeed);
        if (isCanceled) return;

        _lastInteractable.FinishInteract();
    }

    private void SetWhisperLocalizationString()
    {
        if (_actionSettings.WhisperTextKey == null) return;

        whisperLocalization.TableEntryReference = _actionSettings.WhisperTextKey;
        UpdateWhisperText(whisperLocalization.GetLocalizedString());
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