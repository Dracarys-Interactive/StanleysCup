using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    public class CollectableResource : MonoBehaviour
    {
        public int points = 1;
        public AudioClip sfxWhenCollected;
        public ParticleSystem vfxWhenCollected;

        private AudioSource _audioSource;
        private bool _collected = false;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player" && !_collected)
            {
                _collected = true;
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
}