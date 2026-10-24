using UnityEngine;

namespace WaterProstecting
{
    public class PlayerCustomization : MonoBehaviour
    {
        [Header("Точки крепления")]
        public Transform headAnchor;
        public Transform faceAnchor;

        [Header("Тело грибочка")]
        public Renderer bodyRenderer;

        [Header("Отладка")]
        public bool showDebugLogs = true;

        private GameObject currentHat;
        private GameObject currentGlasses;

        public void ApplyItem(ShopItem item)
        {
            if (item == null) return;

            //Цвет
            if (item.isColorChange)
            {
                if (bodyRenderer != null)
                {
                    bodyRenderer.material.color = item.bodyColor;

                    if (showDebugLogs)
                        Debug.Log("Цвет тела изменён на " + item.bodyColor);
                }
                return;
            }

            //Кастомизация
            if (item.prefab == null)
            {
                if (showDebugLogs)
                    Debug.Log("У предмета " + item.itemName + " нет префаба");
                return;
            }

            string nameLower = item.prefab.name.ToLower();

            //Очки?
            if (nameLower.Contains("glasses") || nameLower.Contains("очки"))
            {
                if (currentGlasses != null) Destroy(currentGlasses);

                if (faceAnchor != null)
                {
                    currentGlasses = Instantiate(item.prefab, faceAnchor);
                    currentGlasses.transform.localPosition = Vector3.zero;
                    currentGlasses.transform.localRotation = Quaternion.identity;
                }
            }
            else
            {
                //Шляпка, бантик.
                if (currentHat != null) Destroy(currentHat);

                if (headAnchor != null)
                {
                    currentHat = Instantiate(item.prefab, headAnchor);
                    currentHat.transform.localPosition = Vector3.zero;
                    currentHat.transform.localRotation = Quaternion.identity;
                }
            }

            if (showDebugLogs)
                Debug.Log("Применён предмет: " + item.itemName);
        }
    }
}