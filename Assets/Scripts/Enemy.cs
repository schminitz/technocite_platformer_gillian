using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChickenType { white, red, black }

[RequireComponent(typeof(MovementController))]
public class Enemy : MonoBehaviour
{
	public float speed;
	public bool facingRight;
	public float stopTimeOnFlip;
	public float pushBackForce;
	[HideInInspector]
	public bool dangerous;
	public string sentence;
	public ChickenType chickenType;
	public bool cinematicMode;

	MovementController movementController;
	SpriteRenderer spriteRenderer;
	Vector2 velocity = new Vector2();
	Coroutine flipCoroutine;
	Animator anim;
	AnimationTimes animationTimes;

	// Start is called before the first frame update
	void Start()
    {
		dangerous = true;
		movementController = GetComponent<MovementController>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();
		animationTimes = GetComponent<AnimationTimes>();
		StartFacing();
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
		if(cinematicMode)
			return;

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

	void StartFacing()
	{
		if(facingRight)
		{
			velocity.x = speed;
		}
		else
		{
			velocity.x = -speed;
			spriteRenderer.flipX = !spriteRenderer.flipX;
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

	Coroutine dieCoroutine;
	public void Die()
	{
		if(dieCoroutine == null)
			dieCoroutine = StartCoroutine(DieCoroutine());
	}

	IEnumerator DieCoroutine()
	{
		dangerous = false;

		anim.Play("die");
		velocity.x = 0;

		// Wait for the time of the FrogHit animation to be finished
		yield return new WaitForSeconds(animationTimes.GetTime("ChickenDie"));

		Destroy(gameObject);
		dieCoroutine = null;
	}
}
