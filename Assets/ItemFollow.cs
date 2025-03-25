using UnityEngine;

public class ItemFollow : MonoBehaviour
{
    public Transform target;
    public float rotationFollowSpeed = 20f;

    private void LateUpdate()
    {
        transform.position = target.position;
        
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            target.rotation,
            rotationFollowSpeed * Time.deltaTime
        );
    }
}