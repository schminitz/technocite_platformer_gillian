using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MovementController : MonoBehaviour
{
	public int horizontalRayCount;
	public int verticalRayCount;
	public LayerMask layerObstacle;
	public LayerMask layerOneWayPlatform;
	public Collisions collisions;

	float skinWidth;

	BoxCollider2D boxCollider;
	Vector2 bottomLeft, bottomRight, topLeft, topRight;

	float verticalRaySpacing;
	float horizontalRaySpacing;

	public struct Collisions
	{
		public bool top, bottom, left, right;

		public void Reset()
		{
			top = bottom = left = right = false;
		}
	}

	// Start is called before the first frame update
	void Start()
    {
		boxCollider = GetComponent<BoxCollider2D>();
		skinWidth = 1 / 16f;
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

		transform.Translate(velocity);
	}

	void HorizontalMove(ref Vector2 velocity)
	{
		// XXX brique sort du mur, reassign valeur de distance dans le boucle

		float direction = Mathf.Sign(velocity.x);
		float distance = Mathf.Abs(velocity.x) + skinWidth;

		Vector2 baseOrigin = direction == 1 ? bottomRight : bottomLeft;

		for(int i = 0; i < verticalRayCount; i++)
		{
			Vector2 origin = baseOrigin + new Vector2(0, verticalRaySpacing * i);

			//Debug.DrawLine(origin, origin + new Vector2(direction * 1, 0));
			Debug.DrawLine(origin, origin + new Vector2(direction * distance, 0));
			RaycastHit2D hit = Physics2D.Raycast(
				origin,
				new Vector2(direction, 0),
				distance,
				layerObstacle
				);

			if (hit)
			{
				velocity.x = (hit.distance - skinWidth) * direction;
				distance = hit.distance - skinWidth;

				if(direction < 0)
					collisions.left = true;
				else if(direction > 0)
					collisions.right = true;
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

			Debug.DrawLine(origin, origin + new Vector2(0, direction * distance));
			RaycastHit2D hit = Physics2D.Raycast(
				origin,
				new Vector2(0, direction),
				distance,
				layerObstacle
				);

			if(hit)
			{
				// Je ne suis PAS en train de passer à travers un layer onewayplatform
				//   donc c'est un obstacle
				// XXX detecter en utilisant tag ou getcomponent
				if(!(layerOneWayPlatform == (layerOneWayPlatform | (1 << hit.transform.gameObject.layer)) &&
					 direction > 0))
				{
					velocity.y = (hit.distance - skinWidth) * direction;
					distance = hit.distance - skinWidth;

					if(direction < 0)
						collisions.bottom = true;
					else if(direction > 0)
						collisions.top = true;
				}
			}
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
}
