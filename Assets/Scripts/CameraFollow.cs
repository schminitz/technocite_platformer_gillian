using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	Player player;

	public Vector2 maxStageLimit;
	public Vector2 minStageLimit;

	float height;
	float width;
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
				Vector2 target = new Vector2(player.transform.position.x, player.transform.position.y);

				if(target.x < cameraLimit.left)
					target.x = cameraLimit.left;
				else if(target.x > cameraLimit.right)
					target.x = cameraLimit.right;

				if(target.y < cameraLimit.down)
					target.y = cameraLimit.down;
				else if(target.y > cameraLimit.up)
					target.y = cameraLimit.up;

				transform.position = new Vector3(
					target.x,
					target.y,
					transform.position.z);
			}

			yield return null;
		}
	}
}
