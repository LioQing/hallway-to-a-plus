using GaussianSplatting.Runtime;
using UnityEngine;

public class TorchCutout : MonoBehaviour
{
    public float radius = 1f;
    public float distance = 100f;
    
    private GaussianCutout _cutout;
    
    private void Start()
    {
        _cutout = GetComponentInChildren<GaussianCutout>();
        if (_cutout == null)
        {
            Debug.LogError("GaussianCutout not found in children");
            return;
        }

        _cutout.enabled = true;

        _cutout.transform.localScale = new Vector3(radius, radius, distance);
        _cutout.transform.localPosition = new Vector3(0f, 0f, distance);
    }
}