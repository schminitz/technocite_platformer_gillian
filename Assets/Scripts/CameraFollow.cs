using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	Player player;

	public Vector2 maxStageLimit;
	public Vector2 minStageLimit;

	public float xDeadZone;
	public float yDeadZone;

	float height;
	float width;
	Vector3 velocity;
	Camera cam;

	CameraLimit cameraLimit;

	struct CameraLimit
	{
		public float left, right, up, down;
	}

    // Start is called before the first frame update
    void Start()
    {
		cam = GetComponent<Camera>();

		height = 2f * cam.orthographicSize;
		width = height * cam.aspect;

		cameraLimit.left = minStageLimit.x + width / 2;
		cameraLimit.right = maxStageLimit.x - width / 2;
		cameraLimit.down = minStageLimit.y + height / 2;
		cameraLimit.up = maxStageLimit.y - height / 2;

		StartCoroutine(FindPlayerCoroutine());
		StartCoroutine(FollowPlayer());
    }

	IEnumerator FindPlayerCoroutine()
	{
		while (true)
		{
			if(player == null)
				player = FindObjectOfType<Player>();

			yield return null;
		}
	}

	IEnumerator FollowPlayer()
	{
		// XXX use lateupdate
		while(true)
		{
			if(player != null)
			{
				Rect deadZone = new Rect(
					transform.position.x - xDeadZone / 2f,
					transform.position.y - yDeadZone / 2f,
					xDeadZone,
					yDeadZone);

				DebugDrawRect(deadZone, Color.red);
				Debug.Log(deadZone);
				if (!deadZone.Contains(player.transform.position))
				{
					MoveToPlayer();
				}
			}

			yield return null;
		}
	}

	void DebugDrawRect(Rect rect, Color color)
	{
		// Bot
		Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMax, rect.yMin, 0), color);
		// Top
		Debug.DrawLine(new Vector3(rect.xMin, rect.yMax, 0), new Vector3(rect.xMax, rect.yMax, 0), color);
		// Left
		Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMin, rect.yMax, 0), color);
		// Right
		Debug.DrawLine(new Vector3(rect.xMax, rect.yMin, 0), new Vector3(rect.xMax, rect.yMax, 0), color);
	}

	void MoveToPlayer()
	{
		Vector3 target = new Vector3(
			player.transform.position.x,
			player.transform.position.y,
			transform.position.z);

		transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, 1f);

		// Do not go out of bounds
		if(target.x < cameraLimit.left)
			target.x = cameraLimit.left;
		else if(target.x > cameraLimit.right)
			target.x = cameraLimit.right;

		if(target.y < cameraLimit.down)
			target.y = cameraLimit.down;
		else if(target.y > cameraLimit.up)
			target.y = cameraLimit.up;
	}

}
