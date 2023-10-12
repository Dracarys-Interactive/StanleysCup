using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    public class RandomSpawner : MonoBehaviour
    {
        public SpawnableSO spawnableSO;

        private float _timeOfLastSpawn = 0f;
        private Queue<GameObject> _queue = new Queue<GameObject>();

        void Start()
        {
            if (!spawnableSO.persistent)
            {
                while (_queue.Count < Mathf.Min(5, spawnableSO.maximumInstances))
                {
                    Spawn();
                }
            }
        }

        void Update()
        {
            if ((_queue.Count < spawnableSO.maximumInstances || !spawnableSO.persistent) && spawnableSO.secondsBetweenSpawns > 0 && Time.time - _timeOfLastSpawn > spawnableSO.secondsBetweenSpawns)
                Spawn();

            if (!spawnableSO.persistent && spawnableSO.maximumInstances > 0 && _queue.Count > spawnableSO.maximumInstances)
                Disappear(_queue.Dequeue());
        }

        public GameObject Spawn()
        {
            GameObject spawn = Instantiate(spawnableSO.prefab, transform);

            if (spawnableSO.maximumInstances > 0)
                _queue.Enqueue(spawn);

            spawn.transform.position = new Vector3(
                Random.Range(spawnableSO.spawningRect.center.x - spawnableSO.spawningRect.width / 2, spawnableSO.spawningRect.center.x + spawnableSO.spawningRect.width / 2),
                Random.Range(spawnableSO.spawningRect.center.y - spawnableSO.spawningRect.height / 2, spawnableSO.spawningRect.center.y + spawnableSO.spawningRect.height / 2),
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
                else if (platformSO.movement == PlatformSO.Movement.stationary)
                {
                    platform.speed = 0;
                    platform.speedVariance = 0;
                }
                else if (platformSO.movement == PlatformSO.Movement.waypoints)
                {
                    platform.waypoints = platformSO.waypoints;
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
                    shuffle(platforms);

                    for (int i = 0; i < platforms.Length || !enemy.transform.parent; i++)
                    {
                        Platform parent = platforms[i];

                        if (!parent.GetComponentInChildren<Enemy>() && !parent.GetComponentInChildren<Player>())
                        {
                            enemy.transform.position = Vector2.zero;
                            enemy.transform.parent = parent.gameObject.transform;
                            enemy.transform.localPosition = new Vector2(0, 0.25f);
                        }
                    }

                    if (!enemy.transform.parent)
                    {
                        Debug.Log("can't place Enemy on platform, destroying...");
                        Destroy(spawn);
                    }
                }
            }

            Collectable collectable = spawn.GetComponent<Collectable>();

            if (collectable)
            {
                collectable.onDestroy = RemoveSpawn;
            }
            /*
            else
            {
                spawn.transform.position = new Vector3(Random.Range(spawningRect.rect.xMin, spawningRect.rect.xMax),
                    Random.Range(spawningRect.rect.yMin, spawningRect.rect.yMax), 0) + spawningRect.transform.position;
            }
            */

            _timeOfLastSpawn = Time.time;

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

        private static void shuffle(Platform[] platforms)
        {
            for (int i = 0; i < platforms.Length; i++)
            {
                var tmp = platforms[i];
                int index = Random.Range(i, platforms.Length);
                platforms[i] = platforms[index];
                platforms[index] = tmp;
            }
        }

        public void RemoveSpawn(GameObject spawn)
        {
            if (_queue.Contains(spawn))
            {
                List<GameObject> list = new List<GameObject>();
                while (_queue.Count > 0)
                {
                    GameObject go = _queue.Dequeue();

                    if (go != spawn)
                    {
                        list.Add(go);
                    }
                }

                foreach (GameObject go in list)
                {
                    _queue.Enqueue(go);
                }
            }
        }
    }
}
