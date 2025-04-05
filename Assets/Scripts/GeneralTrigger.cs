using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class GeneralTrigger : MonoBehaviour
{
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    
    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke();
    }
    
    private void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke();
    }
}
