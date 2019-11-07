using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class Enemy : MonoBehaviour
{
	public float speed;
	public bool facingRight;

	MovementController movementController;
	SpriteRenderer spriteRenderer;
	Vector2 velocity = new Vector2();

	// Start is called before the first frame update
	void Start()
    {
		movementController = GetComponent<MovementController>();
		spriteRenderer = GetComponent<SpriteRenderer>();
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
	}

	/// <summary>
	/// Retourne le sprite et la velocity
	/// </summary>
	void Flip()
	{
		spriteRenderer.flipX = !spriteRenderer.flipX;
		velocity.x *= -1;
	}
}
