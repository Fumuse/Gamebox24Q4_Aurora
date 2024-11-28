using UnityEngine;

public class MenuController : SceneController
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private int gameSceneIndex;
    
    private bool _settingsOpened = false;
    private bool _creditsOpened = false;
    private bool _gameStarts = false;

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
        if (_gameStarts) return;
        _gameStarts = true;
        LoadSceneAsync(gameSceneIndex);
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
}
