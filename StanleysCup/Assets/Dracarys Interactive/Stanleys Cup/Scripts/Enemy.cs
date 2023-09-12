using System.Collections;
using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    public class Enemy : MonoBehaviour
    {
        public AudioClip sfxUponPlayerCollision;
        public float delayAfterSFX = 0.1f;

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

            yield return new WaitForSeconds(delayAfterSFX);
            
            GameManager.Instance.ResetGame();

            Destroy(gameObject);
        }

        void Disappear()
        {
            Destroy(gameObject);
        }
    }
}