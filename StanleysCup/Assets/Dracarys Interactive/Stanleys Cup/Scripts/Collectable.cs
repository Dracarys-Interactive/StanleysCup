using System;
using System.Collections;
using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    public class Collectable : MonoBehaviour
    {
        public int points = 1;
        public AudioClip sfxWhenCollected;
        public float delayAfterSFX = 0.1f;
        public Action<GameObject> onDestroy;

        private bool _collected = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !_collected)
            {
                _collected = true;
                StartCoroutine(Collected());
            }
        }

        IEnumerator Collected()
        {
            if (sfxWhenCollected)
                GameManager.Instance.PlaySound(sfxWhenCollected);

            GameManager.Instance.AddPoints(points);

            yield return new WaitForSeconds(delayAfterSFX);

            Destroy(gameObject);
        }

        void Disappear()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (onDestroy != null)
                onDestroy.Invoke(gameObject);
        }
    }
}