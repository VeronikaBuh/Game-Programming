using UnityEngine;

namespace WaterProstecting
{
    public class DewDrop : MonoBehaviour
    {
        [Header("Анимация")]
        public float rotationSpeed = 90f;
        public float bobSpeed = 2f;
        public float bobHeight = 0.15f;

        [Header("Спрятанная росинка")]
        public bool isHidden = false;
        public bool isSpecial = false;

        private Vector3 startPos;
        private bool collected = false;

        void Start()
        {
            startPos = transform.position;

            if (isHidden)
            {
                gameObject.SetActive(false);
            }
        }

        void Update()
        {
            //Вращение
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

            //Покачивание
            float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        void OnTriggerEnter(Collider other)
        {
            if (collected) return;
            if (!other.CompareTag("Player")) return;

            collected = true;

            //GameManager - +росинка
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddDew(1);
            }
            else
            {
                Debug.Log("Росинка собрана (GameManager не найден)");
            }

            //Если для квеста
            if (isSpecial && SheepQuest.Instance != null)
            {
                SheepQuest.Instance.OnSpecialDewCollected();
            }

            Destroy(gameObject);
        }
    }
}