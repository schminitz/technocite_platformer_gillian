using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomNearEnemy : MonoBehaviour
{
	public float circleRadius;
	public LayerMask enemyLayer;
	public float zoomMagnitude;

	CameraController cameraController;

    // Start is called before the first frame update
    void Start()
    {
		cameraController = FindObjectOfType<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
		Collider2D[] enemiesColliders = Physics2D.OverlapCircleAll(transform.position, circleRadius, enemyLayer);
		if (enemiesColliders.Length != 0)
		{
			cameraController.Zoom(zoomMagnitude, 1f);
		}
		else
		{
			cameraController.Dezoom(0.5f);
		}
	}
}
