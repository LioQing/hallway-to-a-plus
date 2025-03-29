using System;
using System.Linq;
using GaussianSplatting.Runtime;
using UnityEngine;
using UnityEngine.Events;

public class HideTrigger : MonoBehaviour
{
    public bool isHidden;
    public bool handleRenderMode = true;
    public GaussianSplatRenderer[] splatRenderers;
    public HideTrigger[] compositeTriggers;
    public UnityEvent onUpdate;
    public UnityEvent onEnter;
    public UnityEvent onExit;

    private void Start()
    {
        foreach (var trigger in compositeTriggers)
        {
            trigger.handleRenderMode = false;
        }
    }

    public void OnCompositeTriggerUpdate()
    {
        if (compositeTriggers.Length == 0)
        {
            return;
        }
        
        var anyHidden = compositeTriggers.Aggregate(false, (hidden, trigger) => hidden || trigger.isHidden);
        switch (anyHidden)
        {
            case true when !isHidden:
            {
                if (handleRenderMode)
                {
                    foreach (var splatRenderer in splatRenderers)
                    {
                        splatRenderer.m_RenderMode = GaussianSplatRenderer.RenderMode.Hidden;
                    }
                }
            
                isHidden = true;
                onUpdate.Invoke();
                onEnter.Invoke();
                break;
            }
            case false when isHidden:
            {
                if (handleRenderMode)
                {
                    foreach (var splatRenderer in splatRenderers)
                    {
                        splatRenderer.m_RenderMode = GaussianSplatRenderer.RenderMode.Splats;
                    }
                }

                isHidden = false;
                onUpdate.Invoke();
                onExit.Invoke();
                break;
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (handleRenderMode)
        {
            foreach (var splatRenderer in splatRenderers)
            {
                splatRenderer.m_RenderMode = GaussianSplatRenderer.RenderMode.Hidden;
            }
        }

        isHidden = true;
        onUpdate.Invoke();
        onEnter.Invoke();
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (handleRenderMode)
        {
            foreach (var splatRenderer in splatRenderers)
            {
                splatRenderer.m_RenderMode = GaussianSplatRenderer.RenderMode.Splats;
            }
        }

        isHidden = false;
        onUpdate.Invoke();
        onExit.Invoke();
    }
}
