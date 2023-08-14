using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    public class OutOfBoundsDetector : MonoBehaviour
    {
        public Vector2 playerOffset = new Vector2(0, 0.056f);
        public bool ignoreUngrounded = false;

        private void OnTriggerExit2D(Collider2D collision)
        {
            switch (collision.tag)
            {
                case "PlatformX":
                    Destroy(collision.gameObject);
                    break;
                default:
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            switch (collision.tag)
            {
                case "PlayerX":
                    Animator animator = collision.gameObject.GetComponent<Animator>();
                    bool isGrounded = animator.GetBool("Grounded");
                    if (isGrounded || !ignoreUngrounded)
                        PlayerOutOfBounds(collision.gameObject);
                    break;
                default:
                    break;
            }
        }

        void PlayerOutOfBounds(GameObject player)
        {
            GameManager.Instance.ResetGame();

            GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
            GameObject closestToOrigin = null;
            float minDistance = 0;

            foreach (GameObject platform in platforms)
            {
                float distance = Vector2.Distance(Vector2.zero, platform.gameObject.transform.position);

                if (closestToOrigin == null || distance < minDistance)
                {
                    closestToOrigin = platform;
                    minDistance = distance;
                }
            }

            if (closestToOrigin)
            {
                player.transform.parent = closestToOrigin.transform;
                player.transform.localPosition = playerOffset;
                player.GetComponent<Animator>().SetBool("Grounded", true);
            }
        }
    }
}
