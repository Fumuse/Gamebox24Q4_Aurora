using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private bool _pauseMenuOpened = false;
    
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

        if (_pauseMenuOpened)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}