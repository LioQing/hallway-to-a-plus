using System;
using System.Collections.Generic;
using GaussianSplatting.Runtime;
using UnityEngine;

public class ShrinkController : MonoBehaviour
{
    public Transform center;
    public float radiusInner;
    public float radiusOuter;
    
    private bool _inArea;
    private Transform _player;
    private IEnumerable<GaussianSplatRenderer> _splatRenderers;
    private Camera _mainCamera;
    private CameraClearFlags _originalClearFlags;

    private void Start()
    {
        _player = FindFirstObjectByType<PlayerController>().transform;
        
        _splatRenderers = GameObject.Find("Gaussian Splats").GetComponentsInChildren<GaussianSplatRenderer>(true);
        
        _mainCamera = Camera.main;
        if (_mainCamera != null)
        {
            _originalClearFlags = _mainCamera.clearFlags;
        }
        else
        {
            Debug.LogError("Main camera not found.");
        }
    }
    
    private void Update()
    {
        if (!_inArea) return;
        
        var distance = Vector3.Distance(_player.position, center.position);

        var t = (distance - radiusInner) / (radiusOuter - radiusInner);
        var interpolatedSize = Mathf.Clamp(Mathf.Lerp(0f, 1f, t), 0.1f, 1f);

        foreach (var splatRenderer in _splatRenderers)
        {
            splatRenderer.m_SplatScale = interpolatedSize;
        }
    }

    public void OnAreaEnter()
    {
        _inArea = true;
        
        _mainCamera.clearFlags = CameraClearFlags.SolidColor;
        _mainCamera.backgroundColor = Color.black;
    }

    public void OnAreaExit()
    {
        _inArea = false;
        
        foreach (var splatRenderer in _splatRenderers)
        {
            splatRenderer.m_SplatScale = 1f;
        }
        
        _mainCamera.clearFlags = _originalClearFlags;
    }
}
