using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class Player : MonoBehaviour
{
	public float acceleration;
	[Tooltip("Number of meter by second")]
	public float maxSpeed;
	float minSpeedThreshold;

	[Tooltip("Unity value of max jump height")]
	public float jumpHeight;
	[Tooltip("Time in seconds to reach the jump height")]
	public float timeToMaxJump;
	[Tooltip("Can i change direction in air?")]
	[Range(0, 1)]
	public float airControl;

	float gravity;
	float jumpForce;
	int horizontal = 0;

	Vector2 velocity = new Vector2();
	MovementController movementController;

    // Start is called before the first frame update
    void Start()
    {
		acceleration *= 60f;
		minSpeedThreshold = acceleration / Application.targetFrameRate * 2f;
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

		horizontal = 0;

		if(Input.GetKey(KeyCode.D))
		{
			horizontal += 1;
		}
		if(Input.GetKey(KeyCode.Q))
		{
			horizontal -= 1;
		}

		if (Input.GetKeyDown(KeyCode.Space) && movementController.collisions.bottom)
		{
			Jump();
		}

		float controlModifier = 1f;
		if (!movementController.collisions.bottom)
		{
			controlModifier = airControl;
		}

		velocity.x += horizontal * acceleration * controlModifier * Time.deltaTime;
		if(velocity.x > maxSpeed)
			velocity.x = maxSpeed;
		if(velocity.x < -maxSpeed)
			velocity.x = -maxSpeed;

		if (horizontal == 0)
		{
			if(velocity.x > minSpeedThreshold)
				velocity.x -= acceleration * Time.deltaTime;
			else if(velocity.x < -minSpeedThreshold)
				velocity.x += acceleration * Time.deltaTime;
			else
				velocity.x = 0;
		}

		velocity.y += gravity * Time.deltaTime;

		movementController.Move(velocity * Time.deltaTime);
	}

	void Jump()
	{
		velocity.y = jumpForce;
	}
}
