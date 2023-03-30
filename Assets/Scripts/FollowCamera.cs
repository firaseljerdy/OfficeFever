using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -5);
    [SerializeField] private float smoothSpeed = 1f;

    private void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }
}
