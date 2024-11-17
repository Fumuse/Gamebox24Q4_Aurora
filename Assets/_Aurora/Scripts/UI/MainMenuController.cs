using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private int gameSceneIndex;
    
    private bool _settingsOpened = false;
    private bool _creditsOpened = false;

    private CancellationTokenSource _cts;

    private void Start()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    private void OnEnable()
    {
        _cts = new();
    }

    private void OnDisable()
    {
        _cts?.Cancel();
    }

    public void GameStart()
    {
        LoadSceneAsync(gameSceneIndex);
    }

    private async void LoadSceneAsync(int sceneIndex)
    {
        await SceneManager.LoadSceneAsync(sceneIndex).WithCancellation(_cts.Token)
            .SuppressCancellationThrow();
    }
    
    public void ToggleSettings()
    {
        _settingsOpened = settingsMenu.activeInHierarchy;
        
        settingsMenu.SetActive(!_settingsOpened);
        _settingsOpened = !_settingsOpened;
        mainMenu.SetActive(!_settingsOpened);
    }
    
    public void ToggleCredits()
    {
        _creditsOpened = creditsMenu.activeInHierarchy;
        
        creditsMenu.SetActive(!_creditsOpened);
        _creditsOpened = !_creditsOpened;
        mainMenu.SetActive(!_creditsOpened);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
