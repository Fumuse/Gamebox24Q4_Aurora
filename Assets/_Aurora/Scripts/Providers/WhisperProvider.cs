using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
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

    private TeleportProvider _teleportProvider;

    private int _localizedTextCharsCount = 1;

    public Action<ActionSettings> OnWhisperEnds;

    private void OnEnable()
    {
        _cts = new();
        InteractableObject.OnInteracted += OnInteracted;
        InteractableObject.OnCancelInteract += OnCancelInteract;
        whisperLocalization.StringChanged += UpdateWhisperText;

        if (_teleportProvider != null)
            _teleportProvider.OnPlayerTeleported += OnPlayerTeleported;
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        InteractableObject.OnInteracted -= OnInteracted;
        InteractableObject.OnCancelInteract -= OnCancelInteract;
        whisperLocalization.StringChanged -= UpdateWhisperText;
        
        if (_teleportProvider != null) 
            _teleportProvider.OnPlayerTeleported -= OnPlayerTeleported;
    }

    private void Start()
    {
        _teleportProvider ??= GameProvidersManager.Instance.TeleportProvider;
    }

    public void Execute(ActionSettings settings)
    {
        _cts?.Cancel();
        _cts = new();
        
        _actionSettings = settings;
        ToWhisper(_lastInteractable);
    }

    public void EmptyExecute(ActionSettings settings)
    {        
        _cts?.Cancel();
        _cts = new();
        
        _actionSettings = settings;
        ToWhisper(null);
    }

    private async void ToWhisper([CanBeNull] IInteractable currentInteractable)
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
        
        EndWhisperInteract(currentInteractable);
        OnWhisperEnds?.Invoke(_actionSettings);
    }

    private void EndWhisperInteract([CanBeNull] IInteractable currentInteractable)
    {
        if (currentInteractable != null)
        {
            if (_actionSettings != null)
            {
                this.AfterInteractChanges(currentInteractable, _actionSettings);
            }
            currentInteractable.FinishInteract();
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

    private void OnCancelInteract(IInteractable interactable)
    {
        _lastInteractable = null;
        _actionSettings = null;
    }

    private void UpdateWhisperText(string text)
    {
        whisperText.text = text;
    }

    public async void Cancel()
    {
        _cts?.Cancel();
        _cts = new();

       await whisperWrapper.FadeIn(this, _cts.Token, fadeSpeed);
    }

    private void OnPlayerTeleported()
    {
        Cancel();
    }
}