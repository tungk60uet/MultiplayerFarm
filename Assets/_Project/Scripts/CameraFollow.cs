using UnityEngine;

namespace _Project.Scripts
{
    public class CameraFollow : MonoBehaviour
    {
        // Target to follow
        public Transform target;

        // Offset from the target (initialized automatically in Start if left as zero)
        public Vector3 offset = Vector3.zero;

        // Smooth time for movement
        [Tooltip("Time for SmoothDamp smoothing")]
        public float smoothTime = 0.15f;

        // Internal velocity used by SmoothDamp
        private Vector3 velocity = Vector3.zero;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // If an offset wasn't provided in the inspector, calculate the current offset
            if (target != null && offset == Vector3.zero)
                offset = transform.position - target.position;
        }

        // Use LateUpdate so camera follows after character movement updates
        void LateUpdate()
        {
            if (target == null)
                return;

            // Desired position matches target X and Z + offset, but keeps camera's current Y
            Vector3 desiredPosition = new Vector3(
                target.position.x + offset.x,
                target.position.y + offset.y,
                target.position.z + offset.z
            );

            // Smoothly move the camera towards the desired position
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
            transform.position = smoothedPosition;
        }
    }
}
