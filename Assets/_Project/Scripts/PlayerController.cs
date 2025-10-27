namespace _Project.Scripts
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;

        [SerializeField] private Transform graphicsTransform;
        [SerializeField] private Rigidbody rb;

        public Vector2 InputDirection => new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
        private void FixedUpdate()
        {
            var input = new Vector3(InputDirection.x, 0f, InputDirection.y);
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
        }
    }
}
