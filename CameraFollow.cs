using UnityEngine;

namespace WaterProstecting
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target; //Грибочек
        public float distance = 5f;
        public float height = 2f;
        public float followSpeed = 5f; //Плавность
        public float rotationSpeed = 5f; //Поворот

        void LateUpdate()
        {
            if (target == null) return;

            //Расположение камеры позади грибочка
            Vector3 desiredPos = target.position
                                 - target.forward * distance
                                 + Vector3.up * height;

            //Плавно движение камеры
            transform.position = Vector3.Lerp(
                transform.position,
                desiredPos,
                followSpeed * Time.deltaTime
            );

            //Смотрим на грибочка
            Vector3 lookPoint = target.position + Vector3.up * 1f;
            Quaternion desiredRot = Quaternion.LookRotation(lookPoint - transform.position);

            //Плавный поворот камеры
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                desiredRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}