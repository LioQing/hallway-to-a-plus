using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LeaveTrigger : MonoBehaviour
{
    public bool isLeft;
    public bool isCorrect;
    public TextMeshProUGUI triggerDisplay;
    public CutsceneManager cutsceneManager;
    
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
        
        cutsceneManager.OnLeave(isLeft, isCorrect);
        
        triggerDisplay.enabled = false;
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
