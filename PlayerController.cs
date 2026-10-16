using UnityEngine;

namespace WaterProstecting
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 7f;       //Скорость ходьбы
        public float jumpForce = 6f;       //Высота прыжка
        public float jumpCooldown = 0.3f;  //Задержка между прыжками
        public float dashSpeed = 15f;      //Скорость рывка
        public float dashTime = 0.2f;      //Длительность рывка
        public float dashCooldown = 2f;    //Перезарядка рывка

        public Transform groundCheck;      //Точка под ногами
        public TrailRenderer dashTrail;    //След при рывке

        private Rigidbody rb;
        private bool isGrounded;           //Проверка земли
        private bool isDashing;            //Проверка рывка
        private float dashTimer;           //Таймер рывка
        private float cooldownTimer;       //Таймер перезарядки рывка
        private float jumpTimer;           //Таймер задержки прыжка
        private bool canJump = true;       //Можно ли сейчас прыгать

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            if (dashTrail != null)
            {
                dashTrail.emitting = false;
            }
        }

        void Update()
        {
            //Проверка, на земле ли
            isGrounded = Physics.CheckSphere(groundCheck.position, 0.3f);
            //Сбрасываем, если игрок оторвался от земли
            if (!isGrounded)
            {
                canJump = false;
            }

            //Прыжок разрешён только если
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded && canJump && !isDashing && jumpTimer <= 0)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
                canJump = false;              //Запрет второго прыжка
                jumpTimer = jumpCooldown;     //Запуск задержки
            }

            //Рывок
            if (Input.GetKeyDown(KeyCode.LeftShift) && cooldownTimer <= 0 && !isDashing)
            {
                StartDash();
            }

            //Таймер
            if (dashTimer > 0) dashTimer -= Time.deltaTime;
            if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;
            if (jumpTimer > 0) jumpTimer -= Time.deltaTime; //Задержка прыжка
            //Разрешаем прыгать снова
            if (isGrounded && jumpTimer <= 0)
            {
                canJump = true;
            }
            //Конец рывка
            if (isDashing && dashTimer <= 0)
            {
                isDashing = false;
                cooldownTimer = dashCooldown;
                if (dashTrail != null)
                {
                    dashTrail.emitting = false;
                }
            }
        }

        void FixedUpdate()
        {
            //Если рывок увел скорость
            if (isDashing)
            {
                rb.linearVelocity = transform.forward * dashSpeed;
                return;
            }

            //WASD
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 move = new Vector3(h, 0, v).normalized;
            rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);

            //Поворот грибочка в сторону движения
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
            if (dashTrail != null)
            {
                dashTrail.emitting = true;
            }
        }
    }
}