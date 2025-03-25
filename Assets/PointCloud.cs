using UnityEngine;

public class PointCloud : MonoBehaviour
{
    public struct Point
    {
        public Vector3 Position;
        public Color Color;
    }
    
    public Mesh pointMesh;
    public float pointSize = 0.1f;
    
    private Material _pointMaterial;
    private ComputeBuffer _pointBuffer;
    private int _pointCount;
    
    private static readonly int PointBuffer = Shader.PropertyToID("_PointBuffer");
    private static readonly int PointSize = Shader.PropertyToID("_PointSize");
    
    private void Awake()
    {
        _pointMaterial = new Material(Shader.Find("Custom/PointCloud"));
        _pointMaterial.SetFloat(PointSize, pointSize);
    }
    
    private void OnDestroy()
    {
        _pointBuffer?.Release();
        
        if (_pointMaterial != null)
            Destroy(_pointMaterial);
    }
    
    public void RenderPoints()
    {
        if (_pointBuffer != null && _pointCount > 0)
        {
            Graphics.RenderMeshPrimitives(
                new RenderParams(_pointMaterial)
                {
                    worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one),
                },
                pointMesh,
                0,
                _pointCount
            );
        }
    }
    
    public void CreatePoint(int count)
    {
        _pointBuffer?.Release();

        _pointCount = count;
        
        _pointBuffer = new ComputeBuffer(_pointCount, sizeof(float) * 7);
        _pointBuffer.SetData(new float[_pointCount * 7]);
    
        _pointMaterial.SetBuffer(PointBuffer, _pointBuffer);
    }

    public void SetPoints(int startIndex, Point[] points)
    {
        if (_pointBuffer == null)
        {
            Debug.LogError("Point cloud not initialized");
            return;
        }
        
        if (startIndex < 0)
        {
            Debug.LogError($"Invalid start index: {startIndex}");
            return;
        }
        
        if (startIndex + points.Length > _pointCount)
        {
            Debug.LogError($"Invalid point count: given {points.Length}, expected <= {_pointCount - startIndex}");
            return;
        }
        
        _pointBuffer.SetData(points, 0, startIndex, points.Length);
    }
}