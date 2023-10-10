using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DracarysInteractive.StanleysCup
{
    public class Player : MonoBehaviour
    {
        [Range(0.0f, 10.0f)]
        public float moveSpeed = 3f;
        public float jumpForce = 600f;
        public RectTransform bounds;
        public bool inBounds;
        public Vector2 playerOffset = new Vector2(0, 0.056f);
        public bool winState = false;

        private Animator _animator;
        private Rigidbody2D _rigidBody;
        private float _vx;
        private bool _canDoubleJump = false;
        private bool _doJump = false;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (winState)
            {
                _animator.SetBool("Grounded", true);
                GameState.Instance.Clear();
                return;
            }

            bool isGrounded = _animator.GetBool("Grounded");

            if (GameManager.Instance.enableDoubleJump && isGrounded)
                _canDoubleJump = true;
            else if (_rigidBody.velocity.y < 0) // Can't jump if falling
                _canDoubleJump = false;

            if ((isGrounded || _canDoubleJump) && _doJump)
            {
                _rigidBody.AddForce(new Vector2(0, jumpForce));
                _canDoubleJump = GameManager.Instance.enableDoubleJump && isGrounded;
            }
            else
            {
                _animator.SetBool("Walking", _vx != 0);
                _rigidBody.velocity = new Vector2(_vx * moveSpeed, _rigidBody.velocity.y);
            }

            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Platform"),
                !_animator.GetBool("Grounded") && _rigidBody.velocity.y > 0.0f);

            // Player can only be out-of-bounds if not grounded and above bounds.
            if (!(inBounds = isInBounds()))
            {
                if (_animator.GetBool("Grounded") || !isAboveBounds())
                {
                    GameManager.Instance.ResetGame();
                }
            }

            _doJump = false;
        }

        void LateUpdate()
        {
            if (_vx == 0)
                return;

            Vector3 localScale = transform.localScale;
            bool facingRight = _vx > 0;

            if (facingRight && localScale.x < 0 || !facingRight && localScale.x > 0)
                localScale.x *= -1;

            transform.localScale = localScale;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<Platform>())
            {
                transform.parent = collision.transform;
                _animator.SetBool("Grounded", true);
            }
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            transform.parent = other.transform;
            _animator.SetBool("Grounded", true);
        }

        // if the player exits a collision with a moving platform, then unchild it
        void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<Platform>())
            {
                transform.parent = null;
                _animator.SetBool("Grounded", false);
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

        public void OnJump(InputAction.CallbackContext context)
        {
            _doJump = context.performed;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _vx = context.ReadValue<Vector2>().x;
        }
    }
}
