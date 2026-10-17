using UnityEngine;

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

            //Вращение зажата пкм
            if (Input.GetMouseButton(1))
            {
                yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
                pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
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