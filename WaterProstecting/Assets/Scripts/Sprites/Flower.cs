using UnityEngine;

namespace WaterProstecting
{
    public enum FlowerType
    {
        Pink,
        Blue,
        Gold
    }

    public class Flower : MonoBehaviour
    {
        [Header("Тип цветка")]
        public FlowerType type = FlowerType.Pink;

        [Header("Материалы")]
        public Material deadMaterial;
        public Material aliveMaterial;

        [Header("Настройки")]
        public int dewCost = 1;
        public float bonusTime = 0f;

        private bool isAlive = false;
        private Renderer rend;

        void Start()
        {
            rend = GetComponentInChildren<Renderer>();

            if (rend != null && deadMaterial != null)
            {
                rend.material = deadMaterial;
            }

            //Тип
            switch (type)
            {
                case FlowerType.Pink:
                    dewCost = 1;
                    bonusTime = 0f;
                    break;
                case FlowerType.Blue:
                    dewCost = 2;
                    bonusTime = 0f;
                    break;
                case FlowerType.Gold:
                    dewCost = 3;
                    bonusTime = 10f;
                    break;
            }
        }

        public bool TryWater()
        {
            if (isAlive) return false;

            //GameManager
            if (GameManager.Instance == null)
            {
                Debug.Log("GameManager не найден — нельзя полить цветок");
                return false;
            }

            if (GameManager.Instance.SpendDew(dewCost))
            {
                isAlive = true;

                if (rend != null && aliveMaterial != null)
                {
                    rend.material = aliveMaterial;
                }

                if (bonusTime > 0f)
                {
                    GameManager.Instance.AddTime(bonusTime);
                }

                GameManager.Instance.OnFlowerWatered(this);
                return true;
            }
            else
            {
                GameManager.Instance.ShowMessage("Не хватает росинок!");
                return false;
            }
        }

        public bool IsAlive
        {
            get { return isAlive; }
        }

        public FlowerType Type
        {
            get { return type; }
        }
    }
}