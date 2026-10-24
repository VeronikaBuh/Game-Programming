using UnityEngine;
using UnityEngine.InputSystem;

namespace WaterProstecting
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 7f;
        public float jumpForce = 6f;
        public float jumpCooldown = 0.3f;
        public float dashSpeed = 15f;
        public float dashTime = 0.2f;
        public float dashCooldown = 2f;

        public Transform groundCheck;
        public float groundRadius = 0.2f;
        public LayerMask groundLayer; //Слой земли (не цеплять кусты)
        public TrailRenderer dashTrail;

        private Rigidbody rb;
        private bool isGrounded;
        private bool isDashing;
        private float dashTimer;
        private float cooldownTimer;
        private float jumpTimer;
        private bool canJump = true;

        private PlayerControls controls;
        private Vector2 moveInput;

        void Awake()
        {
            controls = new PlayerControls();
        }

        void OnEnable()
        {
            controls.Player.Enable();
            controls.Player.Jump.performed += OnJump;
            controls.Player.Dash.performed += OnDash;
        }

        void OnDisable()
        {
            controls.Player.Jump.performed -= OnJump;
            controls.Player.Dash.performed -= OnDash;
            controls.Player.Disable();
        }

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            if (dashTrail != null) dashTrail.emitting = false;
        }

        void Update()
        {
            moveInput = controls.Player.Move.ReadValue<Vector2>();

            //Проверка земли
            isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer);
            
            if (!isGrounded && !isDashing) 
            {
                canJump = false;
            }

            if (dashTimer > 0) dashTimer -= Time.deltaTime;
            if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;
            if (jumpTimer > 0) jumpTimer -= Time.deltaTime;

            //Прыгать снова если грибок на земле и таймер прошел
            if (isGrounded && jumpTimer <= 0) 
            {
                canJump = true;
            }

            if (isDashing && dashTimer <= 0)
            {
                isDashing = false;
                cooldownTimer = dashCooldown;
                if (dashTrail != null) dashTrail.emitting = false;
            }
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (isGrounded && canJump && !isDashing && jumpTimer <= 0)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
                canJump = false;
                jumpTimer = jumpCooldown;
            }
        }

        private void OnDash(InputAction.CallbackContext context)
        {
            if (cooldownTimer <= 0 && !isDashing)
            {
                StartDash();
            }
        }

        void FixedUpdate()
        {
            if (isDashing)
            {
                rb.linearVelocity = transform.forward * dashSpeed;
                return;
            }

            Vector3 input = new Vector3(moveInput.x, 0, moveInput.y).normalized;

            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0;
            camForward.Normalize();

            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0;
            camRight.Normalize();

            Vector3 move = camForward * input.z + camRight * input.x;

            rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);

            if (move.magnitude > 0.1f)
            {
                Quaternion target = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, 10f * Time.fixedDeltaTime);
            }
        }

        void StartDash()
        {
            isDashing = true;
            dashTimer = dashTime;
            if (dashTrail != null) dashTrail.emitting = true;
        }
        void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
            }
        }
    }
}