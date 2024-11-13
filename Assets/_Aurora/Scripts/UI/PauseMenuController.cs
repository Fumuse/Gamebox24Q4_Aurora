using System;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private bool _pauseMenuOpened = false;

    public static Action OnPauseChanged;
    public static bool InPause { get; private set; }
    
    private void OnEnable()
    {
        InputReader.OnEscClicked += OnEscClicked;
    }

    private void OnDisable()
    {
        InputReader.OnEscClicked -= OnEscClicked;
    }

    private void OnEscClicked()
    {
        TogglePauseMenu();
    }
    
    public void TogglePauseMenu()
    {
        _pauseMenuOpened = pauseMenu.activeInHierarchy;
        
        pauseMenu.SetActive(!_pauseMenuOpened);
        _pauseMenuOpened = !_pauseMenuOpened;

        InPause = _pauseMenuOpened;

        if (_pauseMenuOpened)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        
        OnPauseChanged?.Invoke();
    }

    public void Exit()
    {
        Application.Quit();
    }
}