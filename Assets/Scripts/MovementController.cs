﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MovementController : MonoBehaviour
{
	public int horizontalRayCount;
	public int verticalRayCount;
	public LayerMask layerObstacle;
	public Collisions collisions;

	float skinWidth;
	float pitDistance;
	float horizontalCollisionBufferDelay;
	float verticalCollisionBufferDelay;

	BoxCollider2D boxCollider;
	Vector2 bottomLeft, bottomRight, topLeft, topRight;

	float verticalRaySpacing;
	float horizontalRaySpacing;

	public struct Collisions
	{
		public bool top, bottom, left, right;
		public bool topBuffer, bottomBuffer, leftBuffer, rightBuffer;
		// Est ce que j'ai un ravin en face de moi?
		public bool frontPit;

		public void Reset()
		{
			top = bottom = left = right = false;
			frontPit = false;
		}
	}

	// Start is called before the first frame update
	void Start()
    {
		horizontalCollisionBufferDelay = 0.11f; // +/- 6.5 frames
		verticalCollisionBufferDelay = 0.06f; // +/- 3.5 frames
		boxCollider = GetComponent<BoxCollider2D>();
		skinWidth = 1 / 16f;
		pitDistance = 0.5f;
		CalculateRaySpacings();
	}

	// Update is called once per frame
	void Update()
    {
	}

	public void Move(Vector2 velocity)
	{
		collisions.Reset();

		CalculateBounds();
		if (velocity.x != 0)
			HorizontalMove(ref velocity);
		if (velocity.y != 0)
			VerticalMove(ref velocity);

		DetectFrontPit(velocity);

		transform.Translate(velocity);
	}

	void HorizontalMove(ref Vector2 velocity)
	{
		float direction = Mathf.Sign(velocity.x);
		float distance = Mathf.Abs(velocity.x) + skinWidth;

		Vector2 baseOrigin = direction == 1 ? bottomRight : bottomLeft;

		for(int i = 0; i < verticalRayCount; i++)
		{
			Vector2 origin = baseOrigin + new Vector2(0, verticalRaySpacing * i);

			//Debug.DrawLine(origin, origin + new Vector2(direction * distance, 0));
			RaycastHit2D hit = Physics2D.Raycast(
				origin,
				new Vector2(direction, 0),
				distance,
				layerObstacle
				);

			if (hit)
			{
				if(!(hit.transform.gameObject.tag == "oneWayPlatform"))
				{
					velocity.x = (hit.distance - skinWidth) * direction;
					distance = hit.distance - skinWidth;

					if(direction < 0)
					{
						collisions.left = true;
						LaunchLeftBuffer();
					}
					else if(direction > 0)
					{
						collisions.right = true;
						LaunchRightBuffer();
					}
				}
			}
		}
	}

	void VerticalMove(ref Vector2 velocity)
	{
		float direction = Mathf.Sign(velocity.y);
		float distance = Mathf.Abs(velocity.y) + skinWidth;

		Vector2 baseOrigin = direction == 1 ? topLeft : bottomLeft;

		for(int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 origin = baseOrigin + new Vector2(horizontalRaySpacing * i, 0);

			//Debug.DrawLine(origin, origin + new Vector2(0, direction * distance));
			RaycastHit2D hit = Physics2D.Raycast(
				origin,
				new Vector2(0, direction),
				distance,
				layerObstacle
				);

			if(hit)
			{
				// Je ne suis PAS en train de passer à travers un layer onewayplatform vers le haut
				//   donc c'est un obstacle
				if(!(hit.transform.gameObject.tag == "oneWayPlatform" &&
					 direction > 0))
				{
					velocity.y = (hit.distance - skinWidth) * direction;
					distance = hit.distance - skinWidth;

					if(direction < 0)
					{
						collisions.bottom = true;
						LaunchBottomBuffer();
					}
					else if(direction > 0)
					{
						collisions.top = true;
						// No need to calculate that for the moment
						//LaunchTopBuffer();
					}
				}
			}
		}
	}

	void DetectFrontPit(Vector2 velocity)
	{
		Vector2 origin = velocity.x > 0 ? bottomRight : bottomLeft;

		//Debug.DrawLine(origin, origin + Vector2.down * pitDistance);
		RaycastHit2D hit = Physics2D.Raycast(
			origin,
			Vector2.down,
			pitDistance,
			layerObstacle
			);

		if(!hit)
		{
			collisions.frontPit = true;
		}
	}

	void CalculateRaySpacings()
	{
		Bounds bounds = boxCollider.bounds;
		bounds.Expand(skinWidth * -2f);

		verticalRaySpacing = bounds.size.y / (verticalRayCount - 1);
		horizontalRaySpacing = bounds.size.x / (horizontalRayCount - 1);
	}

	void CalculateBounds()
	{
		Bounds bounds = boxCollider.bounds;
		bounds.Expand(skinWidth * -2f);

		bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		topLeft = new Vector2(bounds.min.x, bounds.max.y);
		topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	Coroutine delayBottomBufferCoroutine;
	void LaunchBottomBuffer()
	{
		if(delayBottomBufferCoroutine != null)
			StopCoroutine(delayBottomBufferCoroutine);

		delayBottomBufferCoroutine = StartCoroutine(DelayBottomBufferCoroutine());
	}

	IEnumerator DelayBottomBufferCoroutine()
	{
		collisions.bottomBuffer = true;

		yield return new WaitForSeconds(verticalCollisionBufferDelay);

		collisions.bottomBuffer = false;

		delayBottomBufferCoroutine = null;
	}

	Coroutine delayTopBufferCoroutine;
	void LaunchTopBuffer()
	{
		if(delayTopBufferCoroutine != null)
			StopCoroutine(delayTopBufferCoroutine);

		delayTopBufferCoroutine = StartCoroutine(DelayTopBufferCoroutine());
	}

	IEnumerator DelayTopBufferCoroutine()
	{
		collisions.topBuffer = true;

		yield return new WaitForSeconds(verticalCollisionBufferDelay);

		collisions.topBuffer = false;

		delayTopBufferCoroutine = null;
	}

	Coroutine delayLeftBufferCoroutine;
	void LaunchLeftBuffer()
	{
		if(delayLeftBufferCoroutine != null)
			StopCoroutine(delayLeftBufferCoroutine);

		delayLeftBufferCoroutine = StartCoroutine(DelayLeftBufferCoroutine());
	}

	IEnumerator DelayLeftBufferCoroutine()
	{
		collisions.leftBuffer = true;

		yield return new WaitForSeconds(horizontalCollisionBufferDelay);

		collisions.leftBuffer = false;

		delayLeftBufferCoroutine = null;
	}

	Coroutine delayRightBufferCoroutine;
	void LaunchRightBuffer()
	{
		if(delayRightBufferCoroutine != null)
			StopCoroutine(delayRightBufferCoroutine);

		delayRightBufferCoroutine = StartCoroutine(DelayRightBufferCoroutine());
	}

	IEnumerator DelayRightBufferCoroutine()
	{
		collisions.rightBuffer = true;

		yield return new WaitForSeconds(horizontalCollisionBufferDelay);

		collisions.rightBuffer = false;

		delayRightBufferCoroutine = null;
	}
}
