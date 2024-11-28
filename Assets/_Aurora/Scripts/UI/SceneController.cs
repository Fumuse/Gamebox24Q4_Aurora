using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SceneController : MonoBehaviour
{
    protected CancellationTokenSource _cts = new(); 
    
    protected async void LoadSceneAsync(int sceneIndex)
    {
        await SceneManager.LoadSceneAsync(sceneIndex).WithCancellation(_cts.Token)
            .SuppressCancellationThrow();
    }  

    public void GameEnd()
    {
        LoadSceneAsync(0);
    }

    public void Exit()
    {
        Application.Quit();
    }
}