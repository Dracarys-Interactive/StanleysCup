using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.StanleysCup {
    public class RandomSpawner : MonoBehaviour
    {
        public GameObject prefab;
        public float secondsBetweenSpawns = 0.5f;
        public int maximumInstances = -1;
        public RectTransform spawningRect;
        public RectangleSO spawningRectSO;

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

        void Spawn()
        {
            GameObject spawn = Instantiate(prefab, transform);

            if (maximumInstances > 0)
                queue.Enqueue(spawn);

            if (spawningRectSO)
            {
                spawn.transform.position = new Vector3(
                    Random.Range(spawningRectSO.position.x - spawningRectSO.width / 2, spawningRectSO.position.x + spawningRectSO.width / 2),
                    Random.Range(spawningRectSO.position.y - spawningRectSO.height / 2, spawningRectSO.position.y + spawningRectSO.height / 2),
                    0);
            }
            else
            {
                spawn.transform.position = new Vector3(Random.Range(spawningRect.rect.xMin, spawningRect.rect.xMax),
                    Random.Range(spawningRect.rect.yMin, spawningRect.rect.yMax), 0) + spawningRect.transform.position;
            }

            timeOfLastSpawn = Time.time;
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
