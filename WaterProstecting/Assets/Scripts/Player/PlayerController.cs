using UnityEngine;
using UnityEngine.InputSystem;

namespace WaterProstecting
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Движение")]
        public float moveSpeed = 7f;
        
        [Header("Прыжок")]
        public float jumpForce = 6f;
        public float jumpCooldown = 0.3f;
        
        [Header("Рывок")]
        public float dashSpeed = 15f;
        public float dashTime = 0.2f;
        public float dashCooldown = 2f;

        [Header("Ссылки (References)")]
        public Transform groundCheck;
        public float groundRadius = 0.2f;
        public LayerMask groundLayer;
        public TrailRenderer dashTrail;

        [Header("Звуки (Audio)")]
        public AudioSource sfxSource;      // Ссылка на компонент AudioSource
        public AudioClip jumpSound;        // Звук прыжка
        public AudioClip dashSound;        // Звук рывка
        public AudioClip stepSound;        // Звук шага
        public AudioClip fallSound;        // Звук приземления/падения

        // Внутренние переменные
        private Rigidbody rb;
        private PlayerControls controls;
        private Vector2 moveInput;

        private bool isGrounded;
        private bool wasGrounded;          // Нужна для отслеживания приземления
        private bool isDashing;
        private bool canJump = true;

        private float dashTimer;
        private float cooldownTimer;
        private float jumpTimer;

        // Таймер для шагов
        private float stepTimer;
        private float stepInterval = 0.9f; // Интервал между шагами

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
            
            wasGrounded = true; // При старте считаем, что мы на земле
        }

        void Update()
        {
            // 1. Читаем ввод
            moveInput = controls.Player.Move.ReadValue<Vector2>();

            // 2. Проверка земли
            isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer);

            // 3. Логика звука приземления (если летели вниз и коснулись земли)
            if (!wasGrounded && isGrounded)
            {
                if (fallSound != null && sfxSource != null && rb.linearVelocity.y < -1f)
                {
                    sfxSource.PlayOneShot(fallSound, 0.05f);
                }
            }
            wasGrounded = isGrounded;

            // 4. Логика прыжка (запрет спама)
            if (!isGrounded && !isDashing) 
            {
                canJump = false;
            }

            // 5. Таймеры
            if (dashTimer > 0) dashTimer -= Time.deltaTime;
            if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;
            if (jumpTimer > 0) jumpTimer -= Time.deltaTime;

            if (isGrounded && jumpTimer <= 0) 
            {
                canJump = true;
            }

            // 6. Конец рывка
            if (isDashing && dashTimer <= 0)
            {
                isDashing = false;
                cooldownTimer = dashCooldown;
                if (dashTrail != null) dashTrail.emitting = false;
            }

            // =========================================================
            // 7. ЛОГИКА ЗВУКА ШАГОВ (ВОТ ЗДЕСЬ ПРОИЗОШЛИ ИЗМЕНЕНИЯ)
            // =========================================================
            
            // Считаем реальную скорость по горизонтали
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            
            // Проверяем, что игрок реально жмет кнопки и физически движется
            bool isActuallyMoving = moveInput.magnitude > 0.1f && horizontalVelocity.magnitude > 0.3f;

            if (isGrounded && isActuallyMoving && !isDashing)
            {
                stepTimer -= Time.deltaTime;
                if (stepTimer <= 0f)
                {
                    PlayStepSound();
                    stepTimer = stepInterval;
                }
            }
            else
            {
                // Если стоим на месте — жестко сбрасываем таймер шагов
                stepTimer = 0f; 
            }
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (isGrounded && canJump && !isDashing && jumpTimer <= 0)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
                canJump = false;
                jumpTimer = jumpCooldown;

                // Звук прыжка
                if (jumpSound != null && sfxSource != null)
                {
                    sfxSource.PlayOneShot(jumpSound, 0.04f);
                }
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
            // Если идет рывок — двигаемся строго вперед
            if (isDashing)
            {
                rb.linearVelocity = transform.forward * dashSpeed;
                return;
            }

            // Движение относительно камеры
            Vector3 input = new Vector3(moveInput.x, 0, moveInput.y).normalized;

            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0;
            camForward.Normalize();

            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0;
            camRight.Normalize();

            Vector3 move = camForward * input.z + camRight * input.x;

            rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);

            // Поворот в сторону движения
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

            // Звук рывка
            if (dashSound != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(dashSound, 0.05f); 
            }
        }

        // Метод для шагов (со случайным тоном, чтобы не приедалось)
        private void PlayStepSound()
        {
            if (stepSound != null && sfxSource != null)
            {
                sfxSource.pitch = Random.Range(0.9f, 1.1f); 
                sfxSource.PlayOneShot(stepSound, 0.05f);
            }
        }
    }
}