using System;
using System.Collections.Generic;
using System.Linq;
using GaussianSplatting.Runtime;
using Unity.VisualScripting;
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
    public GameObject lidar;
    public PointCloud lidarPointCloud;

    [Header("Gaussian Anomaly")]
    public GaussianSplatAsset[] leftGaussianAnomalies;
    public GaussianSplatAsset[] rightGaussianAnomalies;

    [Header("Mannequin")]
    public GameObject leftMannequin;
    public GameObject rightMannequin;
    public Material mannequinDistanceFadeMaterial;

    [Header("Wall Escape")]
    public GaussianSplatAsset leftWallEscapeGaussian;
    public GaussianSplatAsset rightWallEscapeGaussian;
    public GameObject leftWallEscapeWalls;
    public GameObject rightWallEscapeWalls;
    public GameObject leftWallOriginalWall;
    public GameObject rightWallOriginalWall;
    
    [Header("Leave Triggers")]
    public LeaveTrigger leftLeaveTrigger;
    public LeaveTrigger rightLeaveTrigger;

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
                lidar.SetActive(true);
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
        lidar.SetActive(false);
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
    
    private void CreateGaussianAnomaly(bool isLeft, int index)
    {
        var anomalies = isLeft ? leftGaussianAnomalies : rightGaussianAnomalies;
        var splatRenderer = isLeft ? _left : _right;

        var randomAnomaly = anomalies[index];

        splatRenderer.m_Asset = randomAnomaly;
    }

    private void CreateMannequin(bool isLeft)
    {
        var mannequin = isLeft ? leftMannequin : rightMannequin;
        var instantiated = Instantiate(mannequin);

        if (environment == Environment.Lidar)
        {
            var renderers = instantiated.transform.Find("Mannequin").GetComponentsInChildren<Renderer>();
            lidar.GetComponentInChildren<Lidar>().extraCaptureRenderers.AddRange(renderers);

            foreach (var mannequinRenderer in renderers)
            {
                mannequinRenderer.enabled = false;
            }
        } else if (environment == Environment.DistanceFade)
        {
            var renderers = instantiated.transform.Find("Mannequin").GetComponentsInChildren<Renderer>();
            
            foreach (var mannequinRenderer in renderers)
            {
                mannequinRenderer.material = mannequinDistanceFadeMaterial;
            }
        }
    }

    private void CreateWallEscape(bool isLeft)
    {
        var gaussian = isLeft ? leftWallEscapeGaussian : rightWallEscapeGaussian;
        var walls = isLeft ? leftWallEscapeWalls : rightWallEscapeWalls;
        var originalWall = isLeft ? leftWallOriginalWall : rightWallOriginalWall;
        var splatRenderer = isLeft ? _left : _right;
        
        originalWall.gameObject.SetActive(false);
        splatRenderer.m_Asset = gaussian;
        Instantiate(walls);
    }
}
