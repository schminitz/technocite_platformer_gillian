using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow_old : MonoBehaviour
{
	Player player;

	public float xmin;
	public float xmax;
	public float ymin;
	public float ymax;

	float height;
	float width;
	private Vector3 velocity = Vector3.zero;

	public float xDeadZone;
	public float yDeadZone;

	// Start is called before the first frame update
	void Start()
    {
		height = 2f * Camera.main.orthographicSize;
		width = height * Camera.main.aspect;

		xmin = xmin + width / 2f;
		xmax = xmax - width / 2f;
		ymin = ymin + height / 2f;
		ymax = ymax - height / 2f;

		StartCoroutine(FindPlayerCoroutine());	
    }

    // Update is called once per frame
    void LateUpdate()
    {
		if (player != null)
		{
			Vector3 target = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

			Rect deadZone = new Rect(transform.position.x, transform.position.y, xDeadZone, yDeadZone);

			if (!deadZone.Contains(target))
				transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, 1f);


			//transform.position = new Vector3( player.transform.position.x, player.transform.position.y, transform.position.z);

			if(transform.position.x < xmin)
			{
				transform.position = new Vector3(xmin, transform.position.y, transform.position.z);
			}
			else if(transform.position.x > xmax)
			{
				transform.position = new Vector3(xmax, transform.position.y, transform.position.z);
			}
			if(transform.position.y < ymin)
			{
				transform.position = new Vector3(transform.position.x, ymin, transform.position.z);
			}
			else if(transform.position.y > ymax)
			{
				transform.position = new Vector3(transform.position.x, ymax, transform.position.z);
			}

		}
	}

	IEnumerator FindPlayerCoroutine()
	{
		while (player == null)
		{
			player = FindObjectOfType<Player>();
			yield return null;
		}
	}
}
