/*
 * The FollowCamera script smoothly follows a target Transform (character) while maintaining a specified offset.
 * In the LateUpdate method, the script calculates the desired position of the camera by adding the offset to the target's position.
 * The camera's position is then smoothly interpolated using Vector3.Lerp, with the camera looking at the target.
 */

using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    // The target that the camera will follow
    [SerializeField] private Transform target;

    // The offset from the target's position that the camera will maintain
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -5);

    // The offset from the target's position that the camera will maintain
    [SerializeField] private float smoothSpeed = 1f;

    private void LateUpdate()
    {
        // Calculate the desired position of the camera based on the target's position and the offset
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }
}
