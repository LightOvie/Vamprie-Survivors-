using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
	//References
	Animator animator;


	PlayerMove playerMove;
	SpriteRenderer spriteRenderer;


	// Start is called before the first frame update
	void Start()
	{
		animator = GetComponent<Animator>();
		playerMove = GetComponent<PlayerMove>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		if (playerMove.moveDir.x != 0 || playerMove.moveDir.y != 0)
		{
			animator.SetBool("Move", true);
			SpriteDirectionChecker();
		}
		else
		{

			animator.SetBool("Move", false);
		}
	}

	void SpriteDirectionChecker()
	{
		if (playerMove.lastHorizontalVector < 0)
		{
			spriteRenderer.flipX = true;
		}
		else
		{
			spriteRenderer.flipX = false;

		}
	}

	public void SetAnimatorController(RuntimeAnimatorController controller)
	{
		if (!animator)
		{
			animator = GetComponent<Animator>();
		}

		animator.runtimeAnimatorController = controller;

	}
}

