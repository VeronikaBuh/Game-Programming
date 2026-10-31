using UnityEngine;

namespace WaterProstecting
{
    public class EnemyPatrol : MonoBehaviour
    {
        [Header("Настройки патрулирования")]
        public Transform pointA;
        public Transform pointB;
        public float speed = 3f;

        private Vector3 targetPos;

        void Start()
        {
            targetPos = pointB.position;
        }

        void Update()
        {
            //Движение к цели
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            //Дошли = смена цели
            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                targetPos = (targetPos == pointA.position) ? pointB.position : pointA.position;
            }
        }

        //Косание игрока
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //- время через GameManager
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddTime(-5f);
                    GameManager.Instance.ShowMessage("Ой! Жук укусил! -5 секунд!");
                }

                //Откидывание
                Rigidbody playerRb = other.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    Vector3 pushDir = (other.transform.position - transform.position).normalized;
                    playerRb.AddForce(pushDir * 10f, ForceMode.Impulse);
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            if (pointA != null && pointB != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(pointA.position, pointB.position);
            }
        }
    }
}