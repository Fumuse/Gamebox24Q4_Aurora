using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EndGameHandler : MonoBehaviour
{
    [SerializeField] private ActionSettings deathMovieSettings;
    [SerializeField] private CanvasGroup overlayWrapper;
    [SerializeField] private SceneController controller;

    private CancellationTokenSource _cts = new();

    private void OnEnable()
    {
        WindowLoc6_DialogueProvider.OnDeath += OnDeath;
        Slenderman.PlayerDeadFromScreamer += OnPlayerDeadFromScreamer;
    }

    private void OnDisable()
    {
        WindowLoc6_DialogueProvider.OnDeath -= OnDeath;
        Slenderman.PlayerDeadFromScreamer -= OnPlayerDeadFromScreamer;
        ShowVideoSceneProvider.OnVideoHiddenAfterEnd -= OnVideoHiddenAfterEnd;
    }

    private void OnDeath()
    {
        ShowVideoSceneProvider videoSceneProvider = GameProvidersManager.Instance.VideoSceneProvider;
        videoSceneProvider.Execute(deathMovieSettings);

        ShowVideoSceneProvider.OnVideoHiddenAfterEnd += OnVideoHiddenAfterEnd;
    }

    private async void OnPlayerDeadFromScreamer(PlayerStateMachine player)
    {
        player.Die();
        
        bool isCanceled = await overlayWrapper.FadeIn(this, _cts.Token, 1f);
        if (isCanceled) return;

        OnDeath();
    }

    private void OnVideoHiddenAfterEnd()
    {
        // (bool isCanceled, Vector2 mousePosition) = await WaitForMouseClick()
        //     .AttachExternalCancellation(_cts.Token).SuppressCancellationThrow();
        // if (isCanceled) return;
        
        controller.GameEnd();
    }

    private async UniTask<Vector2> WaitForMouseClick()
    {
        UniTaskCompletionSource<Vector2> tcs = new UniTaskCompletionSource<Vector2>();
        InputReader.MouseClicked handler = (mousePosition) => tcs.TrySetResult(mousePosition);
        InputReader.OnMouseClicked += handler;
        
        return await tcs.Task;
    }
}