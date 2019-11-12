using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
	public float speed;
	public float distance;
	public Vector2 direction;

	Vector2 velocity;
	Vector2 initialPosition;
	Vector2 targetPosition;

	string movingPlatformId = "MovingPlatform";

	// Start is called before the first frame update
	void Start()
    {
		initialPosition = transform.position;
		targetPosition = initialPosition + direction.normalized * distance;
    }

    // Update is called once per frame
    void Update()
    {
		velocity = direction * speed;
		transform.Translate(velocity * Time.deltaTime);

		float difference = Mathf.Abs(transform.position.x - targetPosition.x);

		if(difference > distance)
		{
			direction *= -1;
		}
		if(difference < 0)
		{
			direction *= -1;
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		Player player = collision.GetComponent<Player>();
		if(player != null && player.movementController.collisions.bottom)
		{
			player.AddExternalVelocity(movingPlatformId, velocity);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		Player player = collision.GetComponent<Player>();
		if(player != null)
		{
			player.RemoveExternalVelocity(movingPlatformId);
		}
	}
}
