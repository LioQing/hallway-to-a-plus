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
        AnomalySettings.Environment = AnomalyManager.Environment.Normal;
    }

    public void OnStart()
    {
        if (AnomalySettings.Anomaly == null)
        {
            AnomalySettings.NextAnomaly();
        }

        SceneManager.LoadScene("MainScene");
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
