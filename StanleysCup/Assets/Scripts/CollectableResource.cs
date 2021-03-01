using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableResource : MonoBehaviour
{
    public int points = 1;
    public AudioClip sfxWhenCollected;
    public ParticleSystem vfxWhenCollected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            StartCoroutine(Collected());
    }

    IEnumerator Collected()
    {
        if (GameManager.gm)
            GameManager.gm.AddPoints(points);
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
