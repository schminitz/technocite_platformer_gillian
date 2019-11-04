using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MovementController : MonoBehaviour
{
	BoxCollider2D boxCollider;
	Vector2 bottomLeft, bottomRight, topLeft, topRight;

	public int horizontalRayCount;
	public int verticalRayCount;

	float verticalRaySpacing;

	// Start is called before the first frame update
	void Start()
    {
		boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
		CalculateBounds();

		verticalRaySpacing = boxCollider.bounds.size.y / (verticalRayCount - 1);

		for (int i = 0; i < verticalRayCount; i++)
		{
			Debug.DrawRay(bottomRight + new Vector2(0, verticalRaySpacing * i), Vector2.right, Color.red);
		}
	}

	void CalculateBounds()
	{
		bottomLeft = new Vector2(boxCollider.bounds.min.x, boxCollider.bounds.min.y);
		bottomRight = new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y);
		topLeft = new Vector2(boxCollider.bounds.min.x, boxCollider.bounds.max.y);
		topRight = new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.max.y);
	}
}
