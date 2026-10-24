using UnityEngine;
using UnityEngine.InputSystem; // <--- Добавлено для новой системы ввода

namespace WaterProstecting
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public float distance = 5f;
        public float height = 2f;
        public float followSpeed = 8f;
        public float mouseSensitivity = 3f;
        public float minPitch = -30f;
        public float maxPitch = 60f;

        private float yaw = 0f;
        private float pitch = 20f;

        void LateUpdate()
        {
            if (target == null) return;

            //Вращение, зажата ПКМ
            if (Mouse.current != null && Mouse.current.rightButton.isPressed)
            {
                // Умножаем, чтобы чувств ощущалась
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                yaw += mouseDelta.x * mouseSensitivity * 0.1f;
                pitch -= mouseDelta.y * mouseSensitivity * 0.1f;
                
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            }

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
            Vector3 desiredPos = target.position
                                 + rotation * new Vector3(0, 0, -distance)
                                 + Vector3.up * height;

            transform.position = Vector3.Lerp(
                transform.position,
                desiredPos,
                followSpeed * Time.deltaTime
            );

            Vector3 lookPoint = target.position + Vector3.up * 1f;
            transform.rotation = Quaternion.LookRotation(lookPoint - transform.position);
        }
    }
}