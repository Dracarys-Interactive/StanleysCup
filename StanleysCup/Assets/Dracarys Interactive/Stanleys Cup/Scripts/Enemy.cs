using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    public class Enemy : MonoBehaviour
    {
        public AudioClip sfxUponPlayerCollision;
        public ParticleSystem vfxUponPlayerCollision;

        private AudioSource _audioSource;
        private bool _collidedWithPlayer = false;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player" && !_collidedWithPlayer)
            {
                _collidedWithPlayer = true;
                StartCoroutine(CollidedWithPlayer());
            }
        }

        IEnumerator CollidedWithPlayer()
        {
            if (sfxUponPlayerCollision)
                GameManager.Instance.PlaySound(sfxUponPlayerCollision);

            yield return new WaitForSeconds(.1f);
            
            GameManager.Instance.ResetGame();
        }

        public void Disappear()
        {
            Destroy(gameObject);
        }
    }
}