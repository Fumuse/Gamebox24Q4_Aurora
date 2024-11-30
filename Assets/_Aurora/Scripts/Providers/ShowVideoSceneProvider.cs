using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using UnityEngine.Video;

public class ShowVideoSceneProvider : MonoBehaviour, IAction
{
    [SerializeField] protected CanvasGroup videoPlayerWrapper;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private LocalizedAsset<VideoClip> videoLocalization;
    [SerializeField] protected GameObject loaderSkipPanel;
    [SerializeField] protected Image loaderSkipImage;

    private IInteractable _lastInteractable;
    private ActionSettings _actionSettings;
    private CancellationTokenSource _cts;
    private CancellationTokenSource _holdCts = new();

    private bool _isHeld = false;

    public Action OnVideoEndShowed;
    public static Action OnVideoHiddenAfterEnd;

    public CanvasGroup VideoPlayerWrapper => videoPlayerWrapper;

    private bool _gameEndedOnce = false;

    private void OnEnable()
    {
        _cts = new();
        _holdCts = new();
        InteractableObject.OnInteracted += OnInteracted;
        videoLocalization.AssetChanged += UpdateVideoClip;
        
        InputReader.OnSkipVideoStarted += OnSkipVideoStarted;
        InputReader.OnSkipVideoCanceled += OnSkipVideoCanceled;

        _gameEndedOnce = PlayerPrefs.GetInt(SceneController.GameEndedSaveName) == 1;
    }

    private void OnDisable()
    {
        _holdCts?.Cancel();
        _cts?.Cancel();
        InteractableObject.OnInteracted -= OnInteracted;
        videoLocalization.AssetChanged -= UpdateVideoClip;
        
        InputReader.OnSkipVideoStarted -= OnSkipVideoStarted;
        InputReader.OnSkipVideoCanceled -= OnSkipVideoCanceled;
    }

    private void OnValidate()
    {
        videoPlayer ??= FindFirstObjectByType<VideoPlayer>();
    }

    public void Execute(ActionSettings settings)
    {
        _actionSettings = settings;

        ShowLoaderTip();
        PrepareVideo();
    }

    private void SetVideoLocalizationClip()
    {
        if (_actionSettings.CatSceneClip == null) return;
        UpdateVideoClip(_actionSettings.CatSceneClip.LoadAsset());
    }

    private void UpdateVideoClip(VideoClip clip)
    {
        videoPlayer.clip = clip;
    }

    private void ResetVideo(bool resetClip = false)
    {
        videoPlayer.Stop();
        videoPlayer.time = 0;
        videoPlayer.frame = 0;
        if (resetClip) videoPlayer.clip = null;
    }

    private void PrepareVideo()
    {
        //fix на случай, если у нас остались какие-то ивенты
        videoPlayer.loopPointReached -= OnVideoFinished;
        videoPlayer.prepareCompleted -= PlayVideo;

        SetVideoLocalizationClip();
        
        videoPlayer.prepareCompleted += PlayVideo;
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Prepare();
    }

    private async void PlayVideo(VideoPlayer vp)
    {
        ResetVideo();
        
        videoPlayer.prepareCompleted -= PlayVideo;
        
        if (_lastInteractable != null) _lastInteractable.PuffAudio();
        AmbienceAudioController.Instance.PausePlayBackgroundMusic();
        vp.Play();

        bool isCanceled = await UniTask.WaitUntil(vp, (p) => p.frame == 0, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
        if (isCanceled) return;
        ShowVideoCanvas();
    }

    private async void OnVideoFinished(VideoPlayer vp)
    {
        OnVideoEndShowed?.Invoke();
        videoPlayer.loopPointReached -= OnVideoFinished;

        bool result = await HideVideoCanvas();
        if (!result)
        {
            ResetVideo(true);
            return;
        }

        ResetVideo(true);

        if (_lastInteractable != null)
        {
            if (_actionSettings != null)
            {
                this.AfterInteractChanges(_lastInteractable, _actionSettings);
            }
            _lastInteractable.FinishInteract();
        }
        OnVideoHiddenAfterEnd?.Invoke();

        AmbienceAudioController.Instance.StartPlayBackgroundMusic();
    }

    private void ShowLoaderTip()
    {
        loaderSkipPanel.SetActive(_gameEndedOnce);
    }

    private void StopPlayedVideo()
    {
        if (!videoPlayer.isPlaying) return;
        if (!_gameEndedOnce) return;
        
        videoPlayer.Stop();
        OnVideoFinished(videoPlayer);
    }

    private void OnSkipVideoStarted()
    {
        if (_isHeld) return;
        if (!videoPlayer.isPlaying) return;
        if (!_gameEndedOnce) return;

        _holdCts?.Cancel();
        _holdCts = new();
        
        _isHeld = true;
        WaitForHold();
    }

    private void OnSkipVideoCanceled()
    {
        _isHeld = false;
    }

    
    private float _hourArrowStep = 360f / 12f / 60f;
    
    private async void WaitForHold()
    {
        float elapsedTime = 0f;
        float holdDuration = 3f;
        float totalRotation = -360f;

        while (_isHeld && elapsedTime < holdDuration)
        {
            bool isCanceled = await UniTask.WaitForEndOfFrame(this, cancellationToken: _holdCts.Token)
                .SuppressCancellationThrow();
            if (isCanceled) return;
            
            elapsedTime += Time.deltaTime;

            float rotationStep = totalRotation / (holdDuration / Time.deltaTime);
            loaderSkipImage.transform.Rotate(0, 0, rotationStep);
        }

        if (_isHeld) 
            StopPlayedVideo();
        
        loaderSkipImage.transform.localRotation = Quaternion.Euler(0, 0, -120f);
    }

    private async void ShowVideoCanvas()
    {
        videoPlayerWrapper.blocksRaycasts = true;
        await videoPlayerWrapper.FadeIn(this, _cts.Token, 2f);
    }

    private async UniTask<bool> HideVideoCanvas()
    {
        bool isCanceled = await videoPlayerWrapper.FadeOut(this, _cts.Token, 2f);
        if (!isCanceled)
        {
            videoPlayerWrapper.blocksRaycasts = false;
        }
        return !isCanceled;
    }
    
    private void OnInteracted(IInteractable interactable)
    {
        _lastInteractable = interactable;
    }
}