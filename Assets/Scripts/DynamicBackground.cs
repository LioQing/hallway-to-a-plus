using UnityEngine;

public class DynamicBackground : MonoBehaviour
{
    public float maxOffset = 0.5f;

    private Vector2 _initPosition;
    
    private void Start()
    {
        _initPosition = transform.localPosition;
    }

    private void Update()
    {
        var normOffset = new Vector2(Input.mousePosition.x, Input.mousePosition.y) / Screen.height * 2 - Vector2.one;
        var offset = normOffset * maxOffset;
        var newPosition = _initPosition + offset;
        
        transform.localPosition = new Vector3(
            newPosition.x,
            newPosition.y,
            transform.localPosition.z
        );
    }
}
