using GaussianSplatting.Runtime;
using UnityEditor;
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

[CustomEditor(typeof(TorchCutout))]
public class TorchCutoutEditor : Editor
{
    private void OnSceneGUI()
    {
	    var cutout = target as TorchCutout;

	    if (cutout == null)
	    {
		    Debug.LogError("Cutout is null");
		    return;
	    }
	    
        var scale = new Vector3(cutout.radius, cutout.radius, cutout.distance);
        
        Handles.color = Color.white;
        const float size = 1f;

        // Y-Z Ring (front view)
        Handles.DrawBezier(
            cutout.transform.TransformPoint(new Vector3(0, 0 + scale.y, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(0, 0, cutout.distance + scale.z)),
            cutout.transform.TransformPoint(new Vector3(0, scale.y, cutout.distance + scale.z/2)),
            cutout.transform.TransformPoint(new Vector3(0, scale.y/2, cutout.distance + scale.z)),
            Color.white, Texture2D.whiteTexture, size);
        Handles.DrawBezier(
            cutout.transform.TransformPoint(new Vector3(0, 0 + scale.y, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(0, 0, cutout.distance - scale.z)),
            cutout.transform.TransformPoint(new Vector3(0, scale.y, cutout.distance - scale.z/2)),
            cutout.transform.TransformPoint(new Vector3(0, scale.y/2, cutout.distance - scale.z)),
            Color.white, Texture2D.whiteTexture, size);
        Handles.DrawBezier(
            cutout.transform.TransformPoint(new Vector3(0, 0 - scale.y, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(0, 0, cutout.distance + scale.z)),
            cutout.transform.TransformPoint(new Vector3(0, -scale.y, cutout.distance + scale.z/2)),
            cutout.transform.TransformPoint(new Vector3(0, -scale.y/2, cutout.distance + scale.z)),
            Color.white, Texture2D.whiteTexture, size);
        Handles.DrawBezier(
            cutout.transform.TransformPoint(new Vector3(0, 0 - scale.y, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(0, 0, cutout.distance - scale.z)),
            cutout.transform.TransformPoint(new Vector3(0, -scale.y, cutout.distance - scale.z/2)),
            cutout.transform.TransformPoint(new Vector3(0, -scale.y/2, cutout.distance - scale.z)),
            Color.white, Texture2D.whiteTexture, size);

        // X-Y Ring (side view)
        Handles.DrawBezier(
            cutout.transform.TransformPoint(new Vector3(0 + scale.x, 0, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(0, 0 + scale.y, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(scale.x, scale.y/2, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(scale.x/2, scale.y, cutout.distance)),
            Color.white, Texture2D.whiteTexture, size);
        Handles.DrawBezier(
            cutout.transform.TransformPoint(new Vector3(0 - scale.x, 0, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(0, 0 + scale.y, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(-scale.x, scale.y/2, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(-scale.x/2, scale.y, cutout.distance)),
            Color.white, Texture2D.whiteTexture, size);
        Handles.DrawBezier(
            cutout.transform.TransformPoint(new Vector3(0 + scale.x, 0, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(0, 0 - scale.y, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(scale.x, -scale.y/2, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(scale.x/2, -scale.y, cutout.distance)),
            Color.white, Texture2D.whiteTexture, size);
        Handles.DrawBezier(
            cutout.transform.TransformPoint(new Vector3(0 - scale.x, 0, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(0, 0 - scale.y, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(-scale.x, -scale.y/2, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(-scale.x/2, -scale.y, cutout.distance)),
            Color.white, Texture2D.whiteTexture, size);

        // X-Z Ring (top view)
        Handles.DrawBezier(
            cutout.transform.TransformPoint(new Vector3(0 + scale.x, 0, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(0, 0, cutout.distance + scale.z)),
            cutout.transform.TransformPoint(new Vector3(scale.x, 0, cutout.distance + scale.z/2)),
            cutout.transform.TransformPoint(new Vector3(scale.x/2, 0, cutout.distance + scale.z)),
            Color.white, Texture2D.whiteTexture, size);
        Handles.DrawBezier(
            cutout.transform.TransformPoint(new Vector3(0 - scale.x, 0, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(0, 0, cutout.distance + scale.z)),
            cutout.transform.TransformPoint(new Vector3(-scale.x, 0, cutout.distance + scale.z/2)),
            cutout.transform.TransformPoint(new Vector3(-scale.x/2, 0, cutout.distance + scale.z)),
            Color.white, Texture2D.whiteTexture, size);
        Handles.DrawBezier(
            cutout.transform.TransformPoint(new Vector3(0 + scale.x, 0, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(0, 0, cutout.distance - scale.z)),
            cutout.transform.TransformPoint(new Vector3(scale.x, 0, cutout.distance - scale.z/2)),
            cutout.transform.TransformPoint(new Vector3(scale.x/2, 0, cutout.distance - scale.z)),
            Color.white, Texture2D.whiteTexture, size);
        Handles.DrawBezier(
            cutout.transform.TransformPoint(new Vector3(0 - scale.x, 0, cutout.distance)),
            cutout.transform.TransformPoint(new Vector3(0, 0, cutout.distance - scale.z)),
            cutout.transform.TransformPoint(new Vector3(-scale.x, 0, cutout.distance - scale.z/2)),
            cutout.transform.TransformPoint(new Vector3(-scale.x/2, 0, cutout.distance - scale.z)),
            Color.white, Texture2D.whiteTexture, size);
    }
}
