using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LeaveTrigger : MonoBehaviour
{
    public bool isCorrect;
    public TextMeshProUGUI triggerDisplay;
    
    private bool _isTriggerActive;
    
    private void Start()
    {
        triggerDisplay.enabled = false;
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        var isPressed = ctx.ReadValueAsButton();
        
        if (!isPressed || !_isTriggerActive)
        {
            return;
        }

        if (isCorrect)
        {
            AnomalySettings.NextEnvironment(); // Environment has to be set before anomaly
            AnomalySettings.NextAnomaly();
            
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            AnomalySettings.PastAnomalies.Clear();
            AnomalySettings.Anomaly = null;
            AnomalySettings.Environment = AnomalyManager.Environment.Normal;
            
            AnomalySettings.NextAnomaly();

            SceneManager.LoadScene("MainScene");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        triggerDisplay.enabled = true;
        _isTriggerActive = true;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        triggerDisplay.enabled = false;
        _isTriggerActive = false;
    }
}
