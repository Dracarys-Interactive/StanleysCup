using System.Collections;
using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    public class Enemy : MonoBehaviour
    {
        public AudioClip sfxUponPlayerCollision;
        public float delayAfterSFX = 0.1f;

        void Update()
        {
            Player player = transform.parent.GetComponentInChildren<Player>();

            if (player)
            {
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