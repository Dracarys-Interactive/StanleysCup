using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    public class Enemy : MonoBehaviour
    {
        public AudioClip sfxUponPlayerCollision;
        public ParticleSystem vfxUponPlayerCollision;

        private bool _collidedWithPlayer = false;

        void Update()
        {
            Player player = transform.parent.GetComponentInChildren<Player>();

            if (player)
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