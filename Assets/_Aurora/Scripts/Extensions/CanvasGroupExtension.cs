using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class CanvasGroupExtension
{
    public static async UniTask<bool> FadeIn(this CanvasGroup group, MonoBehaviour coroutineRunner, CancellationToken token, float fadeSpeed)
    {
        group.alpha = 0;

        while (group.alpha < 1)
        {
            bool isCanceled = await UniTask.WaitForEndOfFrame(coroutineRunner, cancellationToken: token)
                .SuppressCancellationThrow();
            if (isCanceled) return true;

            group.alpha += Time.deltaTime * fadeSpeed;
        }

        group.alpha = 1;
        return false;
    }
    
    public static async UniTask<bool> FadeOut(this CanvasGroup group, MonoBehaviour coroutineRunner, CancellationToken token, float fadeSpeed)
    {
        group.alpha = 1;

        while (group.alpha > 0)
        {
            bool isCanceled = await UniTask.WaitForEndOfFrame(coroutineRunner, cancellationToken: token)
                .SuppressCancellationThrow();
            if (isCanceled) return true;

            group.alpha -= Time.deltaTime * fadeSpeed;
        }

        group.alpha = 0;
        return false;
    }
}