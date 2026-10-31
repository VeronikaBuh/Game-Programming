using UnityEngine;

namespace WaterProstecting
{
    public class PlayerCustomization : MonoBehaviour
    {
        [Header("Точки крепления")]
        public Transform headAnchor;
        public Transform faceAnchor;

        [Header("Визуал грибочка")]
        public Transform bodyParent; // Объект, куда мы будем ставить модель (например, Pos)
        public GameObject currentBodyModel; // Ссылка на текущую модель

        [Header("Отладка")]
        public bool showDebugLogs = true;

        private GameObject currentHat;
        private GameObject currentGlasses;

        public void ApplyItem(ShopItem item)
        {
            if (item == null) return;

            // 1. Если это смена всей модели (Скина)
            if (item.prefab != null && item.isColorChange == false && item.itemName.Contains("Скин"))
            {
                ChangeSkin(item.prefab);
                return;
            }

            // 2. Если это смена цвета (оставляем старую логику, если она нужна)
            if (item.isColorChange)
            {
                // Здесь можно оставить смену цвета материала, если она тебе нужна
                return;
            }

            // 3. Смена одежды (шляпки / очки)
            if (item.prefab == null) return;

            string nameLower = item.prefab.name.ToLower();

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
                if (currentHat != null) Destroy(currentHat);
                if (headAnchor != null)
                {
                    currentHat = Instantiate(item.prefab, headAnchor);
                    currentHat.transform.localPosition = Vector3.zero;
                    currentHat.transform.localRotation = Quaternion.identity;
                }
            }
        }

        // Метод для смены всего тела
        private void ChangeSkin(GameObject newSkinPrefab)
        {
            if (newSkinPrefab == null || bodyParent == null) return;

            // Удаляем старую модель
            if (currentBodyModel != null)
            {
                Destroy(currentBodyModel);
            }

            // Создаем новую
            currentBodyModel = Instantiate(newSkinPrefab, bodyParent);
            currentBodyModel.transform.localPosition = Vector3.zero;
            currentBodyModel.transform.localRotation = Quaternion.identity;

            if (showDebugLogs)
                Debug.Log("Скин изменен на: " + newSkinPrefab.name);
        }
    }
}