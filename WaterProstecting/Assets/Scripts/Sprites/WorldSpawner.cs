using UnityEngine;

namespace WaterProstecting
{
    public class WorldSpawner : MonoBehaviour
    {
        [Header("Префабы для спавна")]
        public GameObject dewPrefab;
        public GameObject[] flowerPrefabs;

        [Header("Количество")]
        public int dewCount = 10;
        public int flowerCount = 5;

        [Header("Область спавна (Полянка)")]
        public Transform spawnArea;
        public float spawnRadius = 10f; 
        
        [Header("Слои (Layers)")]
        public LayerMask groundLayer;    //Слой земли
        public LayerMask obstacleLayer;  //Слой препятствий

        [Header("Настройки высоты")]
        public float dewHeight = 0.3f;    //Высота росинок
        public float flowerHeight = 0f;   //Высота цветов

        public int maxAttempts = 30;      //Попытки найти место

        void Start()
        {
            SpawnDews();
            SpawnFlowers();
        }

        void SpawnDews()
        {
            if (dewPrefab == null || spawnArea == null) return;

            for (int i = 0; i < dewCount; i++)
            {
                for (int attempt = 0; attempt < maxAttempts; attempt++)
                {
                    if (TryGetSpawnPosition(dewHeight, out Vector3 spawnPos))
                    {
                        Instantiate(dewPrefab, spawnPos, dewPrefab.transform.rotation, transform);
                        break; 
                    }
                }
            }
        }

        void SpawnFlowers()
        {
            if (flowerPrefabs.Length == 0 || spawnArea == null) return;

            for (int i = 0; i < flowerCount; i++)
            {
                for (int attempt = 0; attempt < maxAttempts; attempt++)
                {
                    if (TryGetSpawnPosition(flowerHeight, out Vector3 spawnPos))
                    {
                        GameObject randomFlower = flowerPrefabs[Random.Range(0, flowerPrefabs.Length)];
                        Instantiate(randomFlower, spawnPos, randomFlower.transform.rotation, transform);
                        break;
                    }
                }
            }
        }

        //Поиск места
        private bool TryGetSpawnPosition(float heightOffset, out Vector3 resultPos)
        {
            resultPos = Vector3.zero;

            //Случайная точка в радиусе
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 randomPoint = spawnArea.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            //Ллуч с +50м вниз
            Vector3 rayStart = randomPoint + Vector3.up * 50f;
            
            //Поиск земли (groundLayer)
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 100f, groundLayer))
            {
                //Луч попал в землю, проверка препятствий
                if (obstacleLayer != 0 && Physics.CheckSphere(hit.point, 1.5f, obstacleLayer))
                {
                    return false; //Место занято, повтор
                }

                //Если всё чисто, ровно на поверхность + нужная высота
                resultPos = hit.point + Vector3.up * heightOffset;
                return true;
            }

            //Луч в пустоте, возврат false
            return false; 
        }

        void OnDrawGizmosSelected()
        {
            if (spawnArea == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnArea.position, spawnRadius);
        }
    }
}