using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableResource : MonoBehaviour
{
    public int points = 1;
    public AudioClip sfxWhenCollected;
    public ParticleSystem vfxWhenCollected;

    private AudioSource audioSource;
    private bool collected = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !collected)
        {
            collected = true;
            StartCoroutine(Collected());
        }
    }

    IEnumerator Collected()
    {
        if (sfxWhenCollected)
            GameManager.gm.PlaySound(sfxWhenCollected);

        GameManager.gm.AddPoints(points);

        yield return new WaitForSeconds(.1f);

        Destroy(gameObject);
    }

    void Disappear()
    {
        Destroy(gameObject);
    }
}
