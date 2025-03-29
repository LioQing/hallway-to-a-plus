using UnityEngine;

public class ItemFollow : MonoBehaviour
{
    public Transform target;
    public float rotationFollowSpeed = 20f;
    public bool shouldUpdate = true;

    private void LateUpdate()
    {
        if (!shouldUpdate)
        {
            return;
        }
        
        OnUpdateRequest();
    }
    
    public void OnUpdateRequest()
    {
        transform.position = target.position;
        
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            target.rotation,
            rotationFollowSpeed * Time.deltaTime
        );
    }
}