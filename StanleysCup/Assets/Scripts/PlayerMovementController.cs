using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Range(0.0f, 10.0f)]
    public float moveSpeed = 3f;
    public float jumpForce = 600f;

    private Animator animator;
    private Rigidbody2D rbody;
	private float vx;
    private bool jumping = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        vx = Input.GetAxisRaw("Horizontal");

        if (animator.GetBool("Grounded") && Input.GetButtonDown("Jump"))
        {
            rbody.AddForce(new Vector2(0, jumpForce));
        }
        else
        {
            animator.SetBool("Walking", vx != 0);
            rbody.velocity = new Vector2(vx * moveSpeed, rbody.velocity.y);
        }

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Platform"),
            !animator.GetBool("Grounded") && rbody.velocity.y > 0.0f);
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
        if (other.gameObject.tag == "MovingPlatform")
        {
            transform.parent = other.transform;
            animator.SetBool("Grounded", true);
        }
    }

    // if the player exits a collision with a moving platform, then unchild it
    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "MovingPlatform")
        {
            transform.parent = null;
            animator.SetBool("Grounded", false);
        }
    }
}
