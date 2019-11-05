using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class Player : MonoBehaviour
{
	[Tooltip("Number of meter by second")]
	public float speed;
	[Tooltip("Unity value of max jump height")]
	public float jumpHeight;
	[Tooltip("Time in seconds to reach the jump height")]
	public float timeToMaxJump;
	[Tooltip("Can i change direction in air?")]
	public bool airControl;

	float gravity;
	float jumpForce;
	int horizontal = 0;

	Vector2 velocity = new Vector2();
	MovementController movementController;

    // Start is called before the first frame update
    void Start()
    {
		movementController = GetComponent<MovementController>();

		// Math calculation for gravity and jumpForce
		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToMaxJump, 2);
		jumpForce = Mathf.Abs(gravity) * timeToMaxJump;
	}

	// Update is called once per frame
	void Update()
    {
		if(movementController.collisions.bottom || movementController.collisions.top)
			velocity.y = 0;

		if (movementController.collisions.bottom || airControl)
		{
			horizontal = 0;

			if(Input.GetKey(KeyCode.D))
			{
				horizontal += 1;
			}
			if(Input.GetKey(KeyCode.Q))
			{
				horizontal -= 1;
			}
		}

		if (Input.GetKeyDown(KeyCode.Space) && movementController.collisions.bottom)
		{
			Jump();
		}

		velocity.x = horizontal * speed;

		velocity.y += gravity * Time.deltaTime;

		movementController.Move(velocity * Time.deltaTime);
	}

	void Jump()
	{
		velocity.y = jumpForce;
	}
}
