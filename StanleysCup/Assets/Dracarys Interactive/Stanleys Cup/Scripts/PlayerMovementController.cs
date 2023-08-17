using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DracarysInteractive.StanleysCup
{
    public class PlayerMovementController : MonoBehaviour
    {
        public UnityEvent<GameObject> playerOutOfBounds;
        [Range(0.0f, 10.0f)]
        public float moveSpeed = 3f;
        public float jumpForce = 600f;
        public RectTransform bounds;
        public bool inBounds;
        public Vector2 playerOffset = new Vector2(0, 0.056f);

        private Animator animator;
        private Rigidbody2D rbody;
        private float vx;
        private bool canDoubleJump = false;
        private bool doJump = false;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            rbody = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            bool isGrounded = animator.GetBool("Grounded");

            if (GameManager.Instance.enableDoubleJump && isGrounded)
                canDoubleJump = true;
            else if (rbody.velocity.y < 0) // Can't jump if falling
                canDoubleJump = false;

            if ((isGrounded || canDoubleJump) && doJump)
            {
                rbody.AddForce(new Vector2(0, jumpForce));
                canDoubleJump = GameManager.Instance.enableDoubleJump && isGrounded;
            }
            else
            {
                animator.SetBool("Walking", vx != 0);
                rbody.velocity = new Vector2(vx * moveSpeed, rbody.velocity.y);
            }

            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Platform"),
                !animator.GetBool("Grounded") && rbody.velocity.y > 0.0f);

            // Player can only be out-of-bounds if not grounded and above bounds.
            if (!(inBounds = isInBounds()))
            {
                if (animator.GetBool("Grounded") || !isAboveBounds())
                {
                    playerOutOfBounds.Invoke(gameObject);
                    DestroyPlayer();
                }
            }

            doJump = false;
        }

        void LateUpdate()
        {
            if (vx == 0)
                return;

            Vector3 localScale = transform.localScale;
            bool facingRight = vx > 0;

            if (facingRight && localScale.x < 0 || !facingRight && localScale.x > 0)
                localScale.x *= -1;

            transform.localScale = localScale;
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<Platform>())
            {
                transform.parent = other.transform;
                animator.SetBool("Grounded", true);
            }
        }

        // if the player exits a collision with a moving platform, then unchild it
        void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<Platform>())
            {
                transform.parent = null;
                animator.SetBool("Grounded", false);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Projectile>())
            {
                Destroy(other.gameObject);
                DestroyPlayer();
            }
        }

        bool isInBounds()
        {
            return bounds.rect.Contains(transform.position);
        }

        bool isAboveBounds()
        {
            return transform.position.y > bounds.rect.yMax;
        }

        void DestroyPlayer()
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
                gameObject.transform.parent = closestToOrigin.transform;
                gameObject.transform.localPosition = playerOffset;
                gameObject.GetComponent<Animator>().SetBool("Grounded", true);
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            doJump = context.performed;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            vx = context.ReadValue<Vector2>().x;
        }
    }
}
