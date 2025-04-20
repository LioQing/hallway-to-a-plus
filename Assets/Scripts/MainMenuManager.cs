using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        AnomalySettings.PastAnomalies.Clear();
        AnomalySettings.Anomaly = null;
        AnomalySettings.StoryLevel = null;
        AnomalySettings.PrevTrapped = false;
        AnomalySettings.Environment = AnomalyManager.Environment.Normal;
    }

    public void OnStart()
    {
        AnomalySettings.StoryLevel = 0;
        AnomalySettings.PrevTrapped = false;
        AnomalySettings.Environment = AnomalyManager.Environment.Normal;
        
        AnomalySettings.PastAnomalies.Clear();
        AnomalySettings.NextAnomaly();
        SceneManager.LoadScene("IntroScene");
    }

    public void OnCredits()
    {
        SceneManager.LoadScene("EndScene");
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
