using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitJump : MonoBehaviour
{
	MovementController movementController;
	Player player;

	// Start is called before the first frame update
	void Start()
    {
		movementController = GetComponentInParent<MovementController>();
		player = GetComponentInParent<Player>();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Enemy enemy = collision.gameObject.GetComponent<Enemy>();

		if(enemy != null)
		{
			HitEnemy(enemy);
		}
	}

	void HitEnemy(Enemy enemy)
	{
		if (!movementController.collisions.bottom && !player.freeze)
		{
			player.Jump();
			player.ResetDoubleJumpCount();
			enemy.Die();
		}
	}
}
