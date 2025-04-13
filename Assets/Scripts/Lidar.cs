using System;
using System.Collections.Generic;
using System.Linq;
using GaussianSplatting.Runtime;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(LineRenderer))]
public class Lidar : MonoBehaviour
{
    public Camera lidarCamera;
    public PointCloud pointCloud;
    public GameObject gaussianSplatsScene;
    public GameObject hideTriggers;
    [CanBeNull] public ItemFollow itemFollow;
    public List<Renderer> extraCaptureRenderers = new();
    public InputActionReference captureAction;

    [Range(0f, 1f)] public float lineWidth = 0.01f;
    [Range(0f, 1f)] public float captureRadius = 0.1f;
    [Range(0f, 1f)] public float captureInterval = 0.1f;
    public int maxPoints = 10000;
    public LayerMask captureLayerMask;

    private int _currentPointIndex;
    private float _captureTimer;
    private LineRenderer _lineRenderer;

    private (GaussianSplatRenderer, HideTrigger)[] _splats;

    private IEnumerable<GaussianSplatRenderer> SplatRenderers => _splats?.Select(s => s.Item1);

    private void OnEnable()
    {
        captureAction.action.Enable();
    }
    
    private void OnDisable()
    {
        captureAction.action.Disable();
    }
    
    private void Start()
    {
        _currentPointIndex = 0;
        _captureTimer = 0f;

        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.startWidth = lineWidth;
        _lineRenderer.endWidth = lineWidth;
        
        var splatRenderers = gaussianSplatsScene.GetComponentsInChildren<GaussianSplatRenderer>(true);
        
        _splats = splatRenderers.Select(splatRenderer =>
        {
            var hideTrigger = hideTriggers.transform.Find(splatRenderer.name).GetComponent<HideTrigger>();
            
            return (splatRenderer, hideTrigger);
        }).ToArray();
        
        foreach (var gsRenderer in SplatRenderers)
        {
            gsRenderer.m_RenderMode = GaussianSplatRenderer.RenderMode.Hidden;
        }

        pointCloud.CreatePoint(maxPoints);
        
        if (itemFollow != null)
        {
            itemFollow.shouldUpdate = false;
        }
    }

    private void LateUpdate()
    {
        if (itemFollow != null)
        {
            itemFollow.OnUpdateRequest();
        }
        
        Capture();
        
        pointCloud.RenderPoints();
    }

    private void Capture()
    {
        _lineRenderer.enabled = false;
        
        if (!captureAction.action.IsPressed())
        {
            return;
        }

        _captureTimer -= Time.deltaTime;

        if (_captureTimer > 0)
        {
            return;
        }

        var newCount = 0;
        while (_captureTimer <= 0)
        {
            _captureTimer += captureInterval;
            newCount += 1;
        }

        var cameraSize = new Vector2Int(lidarCamera.pixelWidth, lidarCamera.pixelHeight);
        var radius = captureRadius * cameraSize.y;
        var center = cameraSize / 2;
        var randomPoints = GetRandomPointsInCircle(newCount, center, radius);
        
        var colors = CaptureColors(randomPoints.Select(p => new Vector2Int((int) p.x, (int) p.y)).ToArray());
        var positions = CapturePositions(randomPoints);

        var points = Enumerable.Range(0, newCount)
            .Where(i => positions[i] != null)
            .Select(i => new PointCloud.Point
            {
                Position = positions[i]!.Value,
                Color = colors[i],
            })
            .ToArray();

        newCount = points.Length;
        
        if (newCount == 0)
        {
            _lineRenderer.enabled = false;
            return;
        }
        
        AddPoints(points);

        _lineRenderer.enabled = true;
        DrawLidarLine(points.Last());
    }

    private Color[] CaptureColors(Vector2Int[] pixels)
    {
        if (pixels.Any(pixel => pixel.x < 0 ||
                                pixel.x >= lidarCamera.pixelWidth ||
                                pixel.y < 0 ||
                                pixel.y >= lidarCamera.pixelHeight))
        {
            throw new ArgumentOutOfRangeException(nameof(pixels), "Pixel coordinates are out of camera bounds.");
        }

        var rt = new RenderTexture(lidarCamera.pixelWidth, lidarCamera.pixelHeight, 16);
        var prevRt = lidarCamera.targetTexture;

        lidarCamera.targetTexture = rt;
        
        _lineRenderer.enabled = false;
        pointCloud.enabled = false;

        foreach (var (gsRenderer, hideTrigger) in _splats)
        {
            if (hideTrigger.isHidden)
            {
                continue;
            }
            
            gsRenderer.m_RenderMode = GaussianSplatRenderer.RenderMode.Splats;
        }
        
        foreach (var extraCaptureRenderer in extraCaptureRenderers)
        {
            extraCaptureRenderer.enabled = true;
        }

        var captureTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        lidarCamera.Render();

        RenderTexture.active = rt;

        var colors = new Color[pixels.Length];
        for (var i = 0; i < pixels.Length; i++)
        {
            captureTexture.ReadPixels(new Rect(pixels[i].x, pixels[i].y, 1, 1), 0, 0);
            captureTexture.Apply();

            colors[i] = captureTexture.GetPixel(0, 0);
        }

        RenderTexture.active = null;
        lidarCamera.targetTexture = prevRt;

        _lineRenderer.enabled = true;
        pointCloud.enabled = true;

        foreach (var gsRenderer in SplatRenderers)
        {
            gsRenderer.m_RenderMode = GaussianSplatRenderer.RenderMode.Hidden;
        }
        
        foreach (var extraCaptureRenderer in extraCaptureRenderers)
        {
            extraCaptureRenderer.enabled = false;
        }

        Destroy(rt);
        Destroy(captureTexture);

        return colors;
    }

    private Vector3?[] CapturePositions(Vector2[] pixels)
    {
        var cameraSize = new Vector2Int(lidarCamera.pixelWidth, lidarCamera.pixelHeight);

        var positions = new Vector3?[pixels.Length];
        for (var i = 0; i < pixels.Length; i++)
        {
            var viewportPoint = pixels[i] / cameraSize;
            var ray = lidarCamera.ViewportPointToRay(viewportPoint);

            if (Physics.Raycast(ray, out var hit, 50f, captureLayerMask))
            {
                positions[i] = hit.point;
            }
            else
            {
                positions[i] = null;
            }
        }

        return positions;
    }

    private void AddPoints(PointCloud.Point[] points)
    {
        var newCount = points.Length;
        var newIndex = _currentPointIndex + newCount;
        
        var skippedCount = 0;
        if (newIndex > maxPoints)
        {
            pointCloud.SetPoints(_currentPointIndex, points.Take(maxPoints - _currentPointIndex).ToArray());
            _currentPointIndex = 0;
            newCount = newIndex - maxPoints;
            skippedCount = points.Length - newCount;
        }

        pointCloud.SetPoints(_currentPointIndex, points.Skip(skippedCount).ToArray());
        _currentPointIndex += newCount;
    }

    private void DrawLidarLine(PointCloud.Point point)
    {
        _lineRenderer.SetPositions(
            new[]
            {
                transform.position,
                point.Position
            }
        );
        _lineRenderer.startColor = point.Color;
        _lineRenderer.endColor = point.Color;
    }

    private static Vector2[] GetRandomPointsInCircle(int count, Vector2 center, float radius)
    {
        var points = new Vector2[count];
        for (var i = 0; i < count; i++)
        {
            var angle = Random.value * 2 * Mathf.PI;
            var distance = Mathf.Sqrt(Random.value) * radius;
            points[i] = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
        }

        return points;
    }
}