using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class Enemy : MonoBehaviour
{
	public float speed;
	public bool facingRight;
	public float stopTimeOnFlip;

	MovementController movementController;
	SpriteRenderer spriteRenderer;
	Vector2 velocity = new Vector2();
	Coroutine flipCoroutine;
	Animator anim;

	// Start is called before the first frame update
	void Start()
    {
		movementController = GetComponent<MovementController>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();
		velocity.x = speed;

		if(!facingRight)
			Flip();
	}

	// Update is called once per frame
	void Update()
    {
		UpdateMove();
		UpdateFlip();
	}

	void UpdateMove()
	{
		movementController.Move(velocity * Time.deltaTime);
	}

	void UpdateFlip()
	{
		// Si on se déplace vers la droite, et touche un mur vers la droite OU BIEN
		// si on se déplace vers la gauche, et touche un mur vers la gauche
		if((velocity.x > 0 && movementController.collisions.right) ||
		   (velocity.x < 0 && movementController.collisions.left))
		{
			Flip();
		}
		// Si j'atteind un ravin, je flip
		else if (movementController.collisions.frontPit)
		{
			Flip();
		}
	}

	/// <summary>
	/// Retourne le sprite et la velocity
	/// </summary>
	void Flip()
	{
		if (flipCoroutine == null)
			flipCoroutine = StartCoroutine(FlipCoroutine());
	}

	IEnumerator FlipCoroutine()
	{
		float actualVelocity = velocity.x;
		velocity.x = 0;

		anim.Play("idle");

		yield return new WaitForSeconds(stopTimeOnFlip);

		anim.Play("run");

		spriteRenderer.flipX = !spriteRenderer.flipX;
		velocity.x = actualVelocity * -1f;
		flipCoroutine = null;
	}
}
