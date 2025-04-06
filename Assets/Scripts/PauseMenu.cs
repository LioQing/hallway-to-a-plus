using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool Paused;
    
    private void Start()
    {
        OnResume();
    }
    
    public void OnPause()
    {
        if (Paused)
        {
            Time.timeScale = 1.0f;
            pauseMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Time.timeScale = 0.0f;
            pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        Paused = !Paused;
    }
    
    public void OnResume()
    {
        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Paused = false;
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
