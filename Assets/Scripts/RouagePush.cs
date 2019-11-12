using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouagePush : MonoBehaviour
{
	public Vector2 pushDirection;
	public float pushPower;

	private string pushId = "RouagePush";

	private void OnTriggerStay2D(Collider2D collision)
	{
		Player player = collision.GetComponent<Player>();
		if (player != null)
		{
			player.AddExternalVelocity(
				pushId,
				pushDirection.normalized * pushPower);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		Player player = collision.GetComponent<Player>();
		if(player != null)
		{
			player.RemoveExternalVelocity(pushId);
		}
	}
}
