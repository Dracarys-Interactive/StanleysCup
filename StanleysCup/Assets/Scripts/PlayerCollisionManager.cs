using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionManager : MonoBehaviour
{
	private Animator animator;

    private void Awake()
    {
		animator = GetComponent<Animator>();
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
