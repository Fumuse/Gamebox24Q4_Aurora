using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SceneController : MonoBehaviour
{
    protected CancellationTokenSource _cts = new();
    public const string GameEndedSaveName = "GameEnded";
    
    protected async void LoadSceneAsync(int sceneIndex)
    {
        await SceneManager.LoadSceneAsync(sceneIndex).WithCancellation(_cts.Token)
            .SuppressCancellationThrow();
    }  

    public void GameEnd()
    {
        DestroyWatcher.Disable();
        
        PlayerPrefs.SetInt(GameEndedSaveName, 1);
        
        LoadSceneAsync(0);
    }

    public void Exit()
    {
        Application.Quit();
    }
}