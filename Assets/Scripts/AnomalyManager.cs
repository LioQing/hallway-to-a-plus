using System;
using System.Collections.Generic;
using System.Linq;
using GaussianSplatting.Runtime;
using UnityEngine;

public class AnomalyManager : MonoBehaviour
{
    public enum Environment
    {
        Normal,
        DistanceFade,
        Torch,
        Lidar
    }

    public enum AnomalyType
    {
        // Gaussian Anomalies
        LeftF,
        LeftGreenGraphic,
        LeftRedGraphic,
        LeftRedDoor1,
        LeftRedDoor2,
        LeftShuffleInfographics,
        RightF,
        RightFlipGenderColor,
        RightRedDoor1,
        RightRedDoor2,
        RightRedDoor3,
        RightShuffleHouses,
        
        // Mannequin
        LeftMannequin,
        RightMannequin,
        
        // TWChim
        LeftTwChim,
        RightTwChim,
        
        // Wall Escape
        LeftWallEscape,
        RightWallEscape,
    }
    
    public GameObject gaussianSplatsScene;
    public GameObject hideTriggers;
    public Environment environment = Environment.Normal;
    public AnomalyType? Anomaly;
    public bool initializeFromSettings = true;
    
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

    [Header("TWChim")]
    public GameObject leftTwChim;
    public GameObject rightTwChim;

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
        
        if (initializeFromSettings)
        {
            environment = AnomalySettings.Environment;
            Anomaly = AnomalySettings.Anomaly;
        }
        
        SetEnvironment(environment);
        SetAnomaly();
        SetLeaveTriggers();
    }

    private void SetEnvironment(Environment newEnv)
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

    private void SetAnomaly()
    {
        switch (Anomaly)
        {
            case AnomalyType.LeftF:
            case AnomalyType.LeftGreenGraphic:
            case AnomalyType.LeftRedGraphic:
            case AnomalyType.LeftRedDoor1:
            case AnomalyType.LeftRedDoor2:
            case AnomalyType.LeftShuffleInfographics:
                CreateGaussianAnomaly((int) Anomaly);
                break;
            case AnomalyType.RightF:
            case AnomalyType.RightFlipGenderColor:
            case AnomalyType.RightRedDoor1:
            case AnomalyType.RightRedDoor2:
            case AnomalyType.RightRedDoor3:
            case AnomalyType.RightShuffleHouses:
                CreateGaussianAnomaly((int) Anomaly - (int) AnomalyType.RightF);
                break;
            case AnomalyType.LeftMannequin:
            case AnomalyType.RightMannequin:
                CreateMannequin();
                break;
            case AnomalyType.LeftTwChim:
            case AnomalyType.RightTwChim:
                CreateTwChim();
                break;
            case AnomalyType.LeftWallEscape:
            case AnomalyType.RightWallEscape:
                CreateWallEscape();
                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void CreateGaussianAnomaly(int index)
    {
        var isLeft = IsLeftAnomaly();
        var anomalies = isLeft ? leftGaussianAnomalies : rightGaussianAnomalies;
        var splatRenderer = isLeft ? _left : _right;

        var randomAnomaly = anomalies[index];

        splatRenderer.m_Asset = randomAnomaly;
    }

    private void CreateMannequin()
    {
        var isLeft = IsLeftAnomaly();
        var mannequin = isLeft ? leftMannequin : rightMannequin;
        var instantiated = Instantiate(mannequin);

        switch (environment)
        {
            case Environment.Lidar:
            {
                var renderers = instantiated.transform.Find("Mannequin").GetComponentsInChildren<Renderer>();
                lidar.GetComponentInChildren<Lidar>().extraCaptureRenderers.AddRange(renderers);

                foreach (var mannequinRenderer in renderers)
                {
                    mannequinRenderer.enabled = false;
                }

                break;
            }
            case Environment.DistanceFade:
            {
                var renderers = instantiated.transform.Find("Mannequin").GetComponentsInChildren<Renderer>();
            
                foreach (var mannequinRenderer in renderers)
                {
                    mannequinRenderer.material = mannequinDistanceFadeMaterial;
                }

                break;
            }
            case Environment.Normal:
                break;
            case Environment.Torch:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CreateTwChim()
    {
        var isLeft = IsLeftAnomaly();
        var twChim = isLeft ? leftTwChim : rightTwChim;
        
        var instantiated = Instantiate(twChim);

        var twChimRenderer = instantiated.GetComponentInChildren<GaussianSplatRenderer>();
        if (environment == Environment.DistanceFade)
        {
            twChimRenderer.m_CSSplatUtilities = distanceFadeSplatUtilities;
            twChimRenderer.m_Cutouts = new[] {distanceCutout};
        }
        else if (environment == Environment.Torch)
        {
            var cutout = torch.GetComponentInChildren<GaussianCutout>();
            twChimRenderer.m_Cutouts = new[] {cutout};
        }
    }

    private void CreateWallEscape()
    {
        var isLeft = IsLeftAnomaly();
        var gaussian = isLeft ? leftWallEscapeGaussian : rightWallEscapeGaussian;
        var walls = isLeft ? leftWallEscapeWalls : rightWallEscapeWalls;
        var originalWall = isLeft ? leftWallOriginalWall : rightWallOriginalWall;
        var splatRenderer = isLeft ? _left : _right;
        
        originalWall.gameObject.SetActive(false);
        splatRenderer.m_Asset = gaussian;
        Instantiate(walls);
    }

    private void SetLeaveTriggers()
    {
        var isLeft = IsLeftAnomaly();
        leftLeaveTrigger.isCorrect = !isLeft;
        rightLeaveTrigger.isCorrect = isLeft;
    }

    private bool IsLeftAnomaly()
    {
        return Anomaly switch
        {
            AnomalyType.LeftF => true,
            AnomalyType.LeftGreenGraphic => true,
            AnomalyType.LeftRedGraphic => true,
            AnomalyType.LeftRedDoor1 => true,
            AnomalyType.LeftRedDoor2 => true,
            AnomalyType.LeftShuffleInfographics => true,
            AnomalyType.LeftMannequin => true,
            AnomalyType.LeftTwChim => true,
            AnomalyType.LeftWallEscape => true,
            _ => false
        };
    }
}
