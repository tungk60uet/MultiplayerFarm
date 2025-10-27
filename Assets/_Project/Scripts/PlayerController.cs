namespace _Project.Scripts
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;

        [SerializeField] private Transform graphicsTransform;
        [SerializeField] private Transform indicatorTransform;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float gridSize = 2f;
        private void FixedUpdate()
        {
            var h = Input.GetAxisRaw("Horizontal");
            var v = Input.GetAxisRaw("Vertical");
            var input = new Vector3(h, 0f, v);
            if (input.sqrMagnitude > 1f) input.Normalize();
            var desiredVelocity = transform.TransformDirection(input) * moveSpeed;
            var current = rb.linearVelocity;
            var newVelocity = new Vector3(desiredVelocity.x, current.y, desiredVelocity.z);
            rb.linearVelocity = newVelocity;
        }

        private void Update()
        {
            var vel = rb.linearVelocity;
            var lookDir = new Vector3(vel.x, 0f, vel.z);
            if (lookDir.sqrMagnitude > 0.0001f)
            {
                var target = Quaternion.LookRotation(lookDir);
                var yawOnly = Quaternion.Euler(0f, target.eulerAngles.y, 0f);
                graphicsTransform.rotation = Quaternion.Slerp(graphicsTransform.rotation, yawOnly, rotationSpeed * Time.deltaTime);
            }
            // Compute face point (use graphicsTransform if available)
            Vector3 facePos = transform.position;
            // Snap X and Z to nearest multiple of gridSize (keep Y unchanged)
            float snappedX = Mathf.Round(facePos.x / gridSize) * gridSize;
            float snappedZ = Mathf.Round(facePos.z / gridSize) * gridSize;
            Vector3 snapped = new Vector3(snappedX, facePos.y, snappedZ);
            indicatorTransform.position = Vector3.Lerp(indicatorTransform.position,snapped,0.2f);
        }
    }
}
