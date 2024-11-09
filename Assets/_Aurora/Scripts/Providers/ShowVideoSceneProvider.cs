using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Video;

public class ShowVideoSceneProvider : MonoBehaviour, IAction
{
    [SerializeField] protected CanvasGroup videoPlayerWrapper;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private LocalizedAsset<VideoClip> videoLocalization;

    private IInteractable _lastInteractable;
    private ActionSettings _actionSettings;
    private CancellationTokenSource _cts;

    private void OnEnable()
    {
        _cts = new();
        InteractableObject.OnInteracted += OnInteracted;
        videoLocalization.AssetChanged += UpdateVideoClip;
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        InteractableObject.OnInteracted -= OnInteracted;
        videoLocalization.AssetChanged -= UpdateVideoClip;
    }

    private void OnValidate()
    {
        videoPlayer ??= FindFirstObjectByType<VideoPlayer>();
    }

    public void Execute(ActionSettings settings)
    {
        _actionSettings = settings;
        
        PrepareVideo();
    }

    private void SetVideoLocalizationClip()
    {
        if (_actionSettings.CatSceneClipKey == null) return;
        videoLocalization.TableEntryReference = _actionSettings.CatSceneClipKey;
        UpdateVideoClip(videoLocalization.LoadAsset());
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
        
        vp.Play();

        bool isCanceled = await UniTask.WaitUntil(vp, (p) => p.frame == 0, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
        if (isCanceled) return;
        ShowVideoCanvas();
    }

    private async void OnVideoFinished(VideoPlayer vp)
    {
        videoPlayer.loopPointReached -= OnVideoFinished;
        
        bool result = await HideVideoCanvas();
        if (!result)
        {
            ResetVideo(true);
            return;
        }

        ResetVideo(true);
        
        this.ChangeInteractableObjectAction(
                _lastInteractable, 
                _actionSettings.ChangeActionSettingsAfterPlay, 
                _actionSettings.ChangeObjectEventAfterPlay
            );
        _lastInteractable.FinishInteract();
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