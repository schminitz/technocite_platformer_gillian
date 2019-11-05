using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class Player : MonoBehaviour
{
	[Tooltip("Number of meter by second")]
	public float speed;
	public float gravity;
	public float jumpForce;

	Vector2 velocity = new Vector2();
	MovementController movementController;

    // Start is called before the first frame update
    void Start()
    {
		movementController = GetComponent<MovementController>();
	}

    // Update is called once per frame
    void Update()
    {
		int horizontal = 0;

		if(movementController.collisions.bottom || movementController.collisions.top)
			velocity.y = 0;

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

		velocity.x = horizontal * speed;

		velocity.y += gravity * Time.deltaTime * -1f;

		movementController.Move(velocity * Time.deltaTime);
	}

	void Jump()
	{
		velocity.y = jumpForce;
	}
}
