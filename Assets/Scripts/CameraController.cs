using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	enum ZoomingState { isZooming, isDezooming, zoomed, dezoomed }
	ZoomingState zoomingState;

	Player player;

	Vector3 velocity;
	Camera cam;
	Scene scene;

	float height;
	float width;

	float minXLimit;
	float maxXLimit;
	float minYLimit;
	float maxYLimit;

	float initialOrthographicSize;

	// Start is called before the first frame update
	void Start()
    {
		zoomingState = ZoomingState.dezoomed;
		cam = GetComponent<Camera>();
		scene = FindObjectOfType<Scene>();

		initialOrthographicSize = cam.orthographicSize;
		height = cam.orthographicSize * 2f;
		width = height * cam.aspect;

		minXLimit = scene.bottomLeftLimit.position.x + width / 2f;
		maxXLimit = scene.topRightLimit.position.x - width / 2f;
		minYLimit = scene.bottomLeftLimit.position.y + height / 2f;
		maxYLimit = scene.topRightLimit.position.y - height / 2f;

		StartCoroutine(FindPlayerCoroutine());
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

	private void LateUpdate()
	{
		if(player != null)
		{
			MoveToPlayer();
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
			Mathf.Clamp(player.transform.position.x, minXLimit, maxXLimit),
			Mathf.Clamp(player.transform.position.y, minYLimit, maxYLimit),
			transform.position.z);

		transform.position = target;
	}

	Coroutine zoomCoroutine;
	public void Zoom(float magnitude, float duration = 1f)
	{
		if(zoomingState == ZoomingState.zoomed || zoomingState == ZoomingState.isZooming)
			return;

		if (zoomingState == ZoomingState.isDezooming)
		{
			if(dezoomCoroutine != null)
			{
				StopCoroutine(dezoomCoroutine);
				dezoomCoroutine = null;
			}
		}
		if (zoomCoroutine == null)
			zoomCoroutine = StartCoroutine(ZoomCoroutine(magnitude, duration));
	}

	IEnumerator ZoomCoroutine(float magnitude, float duration)
	{
		zoomingState = ZoomingState.isZooming;
		float finalValue = initialOrthographicSize / magnitude;
		float time = 0;

		while (time < duration)
		{
			time += Time.deltaTime;
			cam.orthographicSize = Mathf.Lerp(initialOrthographicSize, finalValue, time / duration);
			yield return null;
		}

		cam.orthographicSize = finalValue;
		zoomingState = ZoomingState.zoomed;
		zoomCoroutine = null;
	}

	Coroutine dezoomCoroutine;

	public void Dezoom(float duration = 1f)
	{
		if(zoomingState == ZoomingState.dezoomed || zoomingState == ZoomingState.isDezooming)
			return;

		if(zoomingState == ZoomingState.isZooming)
		{
			if(zoomCoroutine != null)
			{
				StopCoroutine(zoomCoroutine);
				zoomCoroutine = null;
			}
		}

		if(dezoomCoroutine == null)
			dezoomCoroutine = StartCoroutine(DezoomCoroutine(duration));
	}

	IEnumerator DezoomCoroutine(float duration)
	{
		zoomingState = ZoomingState.isDezooming;
		float initialValue = cam.orthographicSize;
		float finalValue = initialOrthographicSize;
		float time = 0;

		while(time < duration)
		{
			time += Time.deltaTime;
			cam.orthographicSize = Mathf.Lerp(initialValue, finalValue, time / duration);
			yield return null;
		}

		cam.orthographicSize = finalValue;
		zoomingState = ZoomingState.dezoomed;
		dezoomCoroutine = null;
	}
}
