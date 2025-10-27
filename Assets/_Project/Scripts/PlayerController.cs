using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace _Project.Scripts
{
    using UnityEngine;

    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;

        [SerializeField] private Transform graphicsTransform;
        [SerializeField] private Rigidbody rb;


        private readonly SyncVar<float> rotationY = new SyncVar<float>();
        
        public Vector2 InputDirection { get; set; }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsOwner)
            {
                Camera.main.GetComponent<CameraFollow>().target = transform;
                GameController.Instance.SetLocalPlayer(this);
            }
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;
            var input = new Vector3(InputDirection.x, 0f, InputDirection.y);
            if (input.sqrMagnitude > 1f) input.Normalize();
            var desiredVelocity = transform.TransformDirection(input) * moveSpeed;
            var current = rb.linearVelocity;
            var newVelocity = new Vector3(desiredVelocity.x, current.y, desiredVelocity.z);
            rb.linearVelocity = newVelocity;
        }

        private void Update()
        {
            if (IsOwner)
            {
                var vel = rb.linearVelocity;
                var lookDir = new Vector3(vel.x, 0f, vel.z);
                if (lookDir.sqrMagnitude > 0.0001f)
                {
                    var target = Quaternion.LookRotation(lookDir);
                    SetRotationY(target.eulerAngles.y);
                }
            }
            var yawOnly = Quaternion.Euler(0f, rotationY.Value, 0f);
            graphicsTransform.rotation = Quaternion.Slerp(graphicsTransform.rotation, yawOnly, rotationSpeed * Time.deltaTime);
        }
        [ServerRpc]
        private void SetRotationY(float newY)
        {
            rotationY.Value = newY;
        }
    }
}
