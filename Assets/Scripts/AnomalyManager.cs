using System;
using System.Collections.Generic;
using System.Linq;
using GaussianSplatting.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnomalyManager : MonoBehaviour
{
    public enum Environment
    {
        Normal,
        DistanceFade,
        Torch,
        Lidar
    }
    
    public GameObject gaussianSplatsScene;
    public GameObject hideTriggers;
    public Environment environment = Environment.Normal;
    
    [Header("Normal")]
    public ComputeShader normalSplatUtilities;
    
    [Header("Distance Fade")]
    public ComputeShader distanceFadeSplatUtilities;
    public GaussianCutout distanceCutout;

    [Header("Torch")]
    public GameObject torch;

    [Header("Lidar")]
    public Lidar lidar;
    public PointCloud lidarPointCloud;

    [Header("Gaussian Anomaly")]
    public GaussianSplatAsset[] leftGaussianAnomalies;
    public GaussianSplatAsset[] rightGaussianAnomalies;

    private (GaussianSplatRenderer, HideTrigger)[] _splats;
    private IEnumerable<GaussianSplatRenderer> SplatRenderers => _splats?.Select(s => s.Item1);
    private GaussianSplatRenderer _left;
    private GaussianSplatRenderer _right;

    private void Start()
    {
        var splatRenderers = gaussianSplatsScene.GetComponentsInChildren<GaussianSplatRenderer>(true);
        
        _splats = splatRenderers.Select(splatRenderer =>
        {
            var hideTrigger = hideTriggers.transform.Find(splatRenderer.name).GetComponent<HideTrigger>();
            
            return (splatRenderer, hideTrigger);
        }).ToArray();

        _left = _splats.Where(s => s.Item1.gameObject.name == "Left").Select(s => s.Item1).First();
        _right = _splats.Where(s => s.Item1.gameObject.name == "Right").Select(s => s.Item1).First();
        
        if (_left == null || _right == null)
        {
            Debug.LogError("Left or Right Gaussian Splat Renderer not found.");
            return;
        }
        
        SetEnvironment(environment);
        
        GenerateGaussianAnomaly(true);
    }

    public void SetEnvironment(Environment newEnv)
    {
        environment = newEnv;
        
        ResetEnvironment();
        
        switch (environment)
        {
            case Environment.DistanceFade:
            {
                distanceCutout.gameObject.SetActive(true);

                var cam = Camera.main;
                if (cam != null)
                {
                    cam.clearFlags = CameraClearFlags.SolidColor;
                    cam.backgroundColor = Color.black;
                }
                
                foreach (var splatRenderer in SplatRenderers)
                {
                    splatRenderer.m_CSSplatUtilities = distanceFadeSplatUtilities;
                    splatRenderer.m_Cutouts = new[] {distanceCutout};
                }
                
                break;
            }
            case Environment.Torch:
            {
                torch.SetActive(true);

                var cam = Camera.main;
                if (cam != null)
                {
                    cam.clearFlags = CameraClearFlags.SolidColor;
                    cam.backgroundColor = Color.black;
                }

                var cutout = torch.GetComponentInChildren<GaussianCutout>();
                
                foreach (var splatRenderer in SplatRenderers)
                {
                    splatRenderer.m_Cutouts = new[] { cutout };
                }
                
                break;
            }
            case Environment.Lidar:
            {
                lidar.gameObject.SetActive(true);
                lidarPointCloud.gameObject.SetActive(true);

                var cam = Camera.main;
                if (cam != null)
                {
                    cam.clearFlags = CameraClearFlags.SolidColor;
                    cam.backgroundColor = Color.black;
                }
                
                foreach (var (splatRenderer, hideTrigger) in _splats)
                {
                    splatRenderer.m_RenderMode = GaussianSplatRenderer.RenderMode.Hidden;
                    hideTrigger.handleRenderMode = false;
                }
                
                break;
            }
            case Environment.Normal:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ResetEnvironment()
    {
        distanceCutout.gameObject.SetActive(false);
        torch.SetActive(false);
        lidar.gameObject.SetActive(false);
        lidarPointCloud.gameObject.SetActive(false);

        var cam = Camera.main;
        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.Skybox;
        }
        
        foreach (var (splatRenderer, hideTrigger) in _splats)
        {
            splatRenderer.m_RenderMode = GaussianSplatRenderer.RenderMode.Splats;
            splatRenderer.m_Cutouts = new GaussianCutout[] {};
            splatRenderer.m_CSSplatUtilities = normalSplatUtilities;
            hideTrigger.handleRenderMode = true;
        }
    }

    private void GenerateGaussianAnomaly(bool isLeft)
    {
        var anomalies = isLeft ? leftGaussianAnomalies : rightGaussianAnomalies;
        var splatRenderer = isLeft ? _left : _right;

        var randomAnomaly = anomalies[Random.Range(0, anomalies.Length)];

        splatRenderer.m_Asset = randomAnomaly;
    }
}
