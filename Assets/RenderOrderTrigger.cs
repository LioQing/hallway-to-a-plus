using GaussianSplatting.Runtime;
using UnityEngine;

public class RenderOrderTrigger : MonoBehaviour
{
    public GaussianSplatRenderer currentSplatRenderer;
    public GaussianSplatRenderer[] adjacentSplatRenderers;
    public GaussianSplatRenderer[] otherSplatRenderers;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        currentSplatRenderer.m_RenderOrder = 2;
        
        foreach (var splatRenderer in adjacentSplatRenderers)
        {
            splatRenderer.m_RenderOrder = 1;
        }
        
        foreach (var splatRenderer in otherSplatRenderers)
        {
            splatRenderer.m_RenderOrder = 0;
        }
    }
}
