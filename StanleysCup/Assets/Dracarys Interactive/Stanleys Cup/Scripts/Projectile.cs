using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    public class Projectile : Moveable
    {
        public AudioClip sfxUponHit;
        public ParticleSystem vfxUponHit;

        private bool _hit = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Player>() && !_hit)
            {
                _hit = true;
                StartCoroutine(Hit());
            }
        }

        IEnumerator Hit()
        {
            if (sfxUponHit)
                GameManager.Instance.PlaySound(sfxUponHit);

            yield return new WaitForSeconds(.1f);

            Destroy(gameObject);
            GameManager.Instance.ResetGame();
        }
    }
}
