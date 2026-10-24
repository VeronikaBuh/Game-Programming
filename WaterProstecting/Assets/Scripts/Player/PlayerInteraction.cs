using UnityEngine;
using UnityEngine.InputSystem;

namespace WaterProstecting
{
    public class PlayerInteraction : MonoBehaviour
    {
        public float interactRange = 2f;
        public LayerMask interactableLayers;

        private PlayerControls controls; //Класс управления

        void Awake()
        {
            controls = new PlayerControls();
        }

        void OnEnable()
        {
            //Карта действий для игрока
            controls.Player.Enable();
            
            //Подписка на нажатие кнопки E
            controls.Player.Interact.performed += OnInteract;
        }

        void OnDisable()
        {
            //Отписка от события и выкл контролы
            controls.Player.Interact.performed -= OnInteract;
            controls.Player.Disable();
        }

        //Метод вызывается автоматически, когда нажимается E
        private void OnInteract(InputAction.CallbackContext context)
        {
            TryInteract();
        }

        void TryInteract()
        {
            //Поиск коллайдеров в радиусе interactRange на слое interactableLayers
            Collider[] hits = Physics.OverlapSphere(
                transform.position,
                interactRange,
                interactableLayers
            );

            foreach (Collider hit in hits)
            {
                //Есть ли на объекте скрипт цветка
                Flower flower = hit.GetComponent<Flower>();
                if (flower != null)
                {
                    flower.TryWater();
                    return;
                }

                //Есть ли на объекте скрипт барашка
                SheepQuest sheep = hit.GetComponent<SheepQuest>();
                if (sheep != null)
                {
                    sheep.Interact();
                    return;
                }
            }

            //Если нет
            Debug.Log("Рядом нет объектов для взаимодействия");
        }

        //Желтая сфера в редакторе, радиус взаимодействия
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }
    }
}