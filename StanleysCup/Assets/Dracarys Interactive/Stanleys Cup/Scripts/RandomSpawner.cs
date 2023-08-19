using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    public class RandomSpawner : MonoBehaviour
    {
        public GameObject prefab;
        public float secondsBetweenSpawns = 0.5f;
        public int maximumInstances = -1;
        public RectTransform spawningRect;
        public SpawnableSO spawnableSO;

        private float timeOfLastSpawn = 0f;
        private Queue<GameObject> queue = new Queue<GameObject>();

        void Start()
        {
            Spawn();
        }

        void Update()
        {
            if (Time.time - timeOfLastSpawn > secondsBetweenSpawns)
                Spawn();

            if (maximumInstances > 0 && queue.Count > maximumInstances)
                Disappear(queue.Dequeue());
        }

        public GameObject Spawn()
        {
            GameObject spawn = Instantiate(prefab, transform);

            if (maximumInstances > 0)
                queue.Enqueue(spawn);

            if (spawnableSO)
            {
                spawn.transform.position = new Vector3(
                    Random.Range(spawnableSO.spawningRect.position.x - spawnableSO.spawningRect.width / 2, spawnableSO.spawningRect.position.x + spawnableSO.spawningRect.width / 2),
                    Random.Range(spawnableSO.spawningRect.position.y - spawnableSO.spawningRect.height / 2, spawnableSO.spawningRect.position.y + spawnableSO.spawningRect.height / 2),
                    0);

                Platform platform = spawn.GetComponent<Platform>();

                if (platform)
                {
                    PlatformSO platformSO = (PlatformSO)spawnableSO;

                    if (platformSO.movement == PlatformSO.Movement.down || platformSO.movement == PlatformSO.Movement.left)
                    {
                        platform.speed *= -1;
                        platform.speedVariance *= -1;
                    }

                    platform.yaxis = platformSO.movement == PlatformSO.Movement.up || platformSO.movement == PlatformSO.Movement.down;
                }

                Enemy enemy = spawn.GetComponent<Enemy>();

                if (enemy)
                {
                    EnemySO enemySO = (EnemySO)spawnableSO;

                    if (enemySO.placeOnPlatform)
                    {
                        Platform[] platforms = FindObjectsByType<Platform>(FindObjectsSortMode.None);

                        enemy.transform.parent = null;

                        while (!enemy.transform.parent)
                        {
                            Platform parent = platforms[Random.Range(0, platforms.Length)];

                            if (!parent.GetComponentInChildren<Enemy>() && !parent.GetComponentInChildren<Player>())
                            {
                                enemy.transform.position = Vector2.zero;
                                enemy.transform.parent = parent.gameObject.transform;
                                enemy.transform.localPosition = new Vector2(0, 0.25f);
                            }
                        }
                    }
                }
            }
            else
            {
                spawn.transform.position = new Vector3(Random.Range(spawningRect.rect.xMin, spawningRect.rect.xMax),
                    Random.Range(spawningRect.rect.yMin, spawningRect.rect.yMax), 0) + spawningRect.transform.position;
            }

            timeOfLastSpawn = Time.time;

            return spawn;
        }

        void Disappear(GameObject go)
        {
            if (go)
            {
                Animator animator = go.GetComponent<Animator>();

                if (animator)
                    animator.SetTrigger("Disappear");
                else
                    Destroy(go);
            }
        }
    }
}
