using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public GameObject prefab;
    public float secondsBetweenSpawns = 0.5f;
    public int maximumInstances = -1;
    public int width = 20;
    public int height = 40;
    public RectTransform spawningRect;

    private float timeOfLastSpawn = 0f;
    private Queue<GameObject> queue = new Queue<GameObject>();

    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - timeOfLastSpawn > secondsBetweenSpawns)
            Spawn();

        if (maximumInstances > 0 && queue.Count > maximumInstances)
            StartCoroutine(Disappear(queue.Dequeue()));
    }

    void Spawn()
    {
        GameObject spawn = Instantiate(prefab);

        if (maximumInstances > 0)
            queue.Enqueue(spawn);

        if (spawningRect)
        {
            spawn.transform.position = new Vector3(Random.Range(spawningRect.rect.xMin, spawningRect.rect.xMax),
                Random.Range(spawningRect.rect.yMin, spawningRect.rect.yMax), 0) + spawningRect.transform.position;
        }
        else
        {
            Vector2 randomPositionOnScreen = new Vector2();
            randomPositionOnScreen.x = Random.Range(-width / 2, width / 2);
            randomPositionOnScreen.y = Random.Range(-height / 2, height / 2);
            spawn.transform.position = randomPositionOnScreen;
        }

        timeOfLastSpawn = Time.time;
    }

    IEnumerator Disappear(GameObject gameObject)
    {
        gameObject.GetComponent<Animator>().SetTrigger("Disappear");
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
