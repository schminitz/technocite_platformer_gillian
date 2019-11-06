using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class Player : MonoBehaviour
{
	delegate void MyDelegate();
	MyDelegate UpdateAnimation;

	[Tooltip("Number of meter by second")]
	public float maxSpeed;
	public float timeToMaxSpeed;

	public uint maxAirJump;

	public bool animationByParameters;

	int jumpCount;

	float acceleration;
	float minSpeedThreshold;

	[Tooltip("Unity value of max jump height")]
	public float jumpHeight;
	[Tooltip("Time in seconds to reach the jump height")]
	public float timeToMaxJump;
	[Tooltip("Can i change direction in air?")]
	[Range(0, 1)]
	public float airControl;

	float gravity;
	float jumpForce;
	float maxFallingSpeed;
	int horizontal = 0;
	bool doubleJumping;

	Animator anim;
	SpriteRenderer spriteRenderer;

	Vector2 velocity = new Vector2();
	MovementController movementController;

    // Start is called before the first frame update
    void Start()
    {
		// Math calculation acceleration
		// s = distance
		// a = acceleration
		// t = time
		// s = 1 / 2 at²
		// a = 2s / t²
		acceleration = (2f * maxSpeed) / Mathf.Pow(timeToMaxSpeed, 2);

		Debug.Log(acceleration);
		minSpeedThreshold = acceleration / Application.targetFrameRate * 2f;
		movementController = GetComponent<MovementController>();
		anim = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();

		// Math calculation for gravity and jumpForce
		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToMaxJump, 2);
		jumpForce = Mathf.Abs(gravity) * timeToMaxJump;
		maxFallingSpeed = -jumpForce;

		if(animationByParameters)
			UpdateAnimation = UpdateAnimationByParameters;
		else
			UpdateAnimation = UpdateAnimationByCode;
	}

	// Update is called once per frame
	void Update()
    {
		if(movementController.collisions.bottom || movementController.collisions.top)
			velocity.y = 0;

		horizontal = 0;

		if(Input.GetKey(KeyCode.D))
		{
			horizontal += 1;
		}
		if(Input.GetKey(KeyCode.Q))
		{
			horizontal -= 1;
		}

		UpdateJump();
		UpdateFlip();

		float controlModifier = 1f;
		if (!movementController.collisions.bottom)
		{
			controlModifier = airControl;
		}

		velocity.x += horizontal * acceleration * controlModifier * Time.deltaTime;

		if(Mathf.Abs(velocity.x) > maxSpeed)
			velocity.x = maxSpeed * horizontal;


		if (horizontal == 0)
		{
			if(velocity.x > minSpeedThreshold)
				velocity.x -= acceleration * Time.deltaTime;
			else if(velocity.x < -minSpeedThreshold)
				velocity.x += acceleration * Time.deltaTime;
			else
				velocity.x = 0;
		}

		velocity.y += gravity * Time.deltaTime;
		if(velocity.y < maxFallingSpeed)
			velocity.y = maxFallingSpeed;

		movementController.Move(velocity * Time.deltaTime);

		if(Input.GetKeyDown(KeyCode.H))
		{
			anim.SetTrigger("hit");
		}

		UpdateAnimation();

	}

	void UpdateAnimationByCode()
	{
		// Au sol
		if (movementController.collisions.bottom)
		{
			if(velocity.x == 0)
				anim.Play("FrogIdle");
			else if(velocity.x != 0)
				anim.Play("FrogRun");
		}
		// En l'air
		else
		{
			if (!doubleJumping)
			{
				if(velocity.y > 0)
					anim.Play("FrogJumping");
				else if(velocity.y < 0)
					anim.Play("FrogFalling");
			}
		}
	}

	void UpdateAnimationByParameters()
	{
		int vertical = (int)Mathf.Sign(velocity.y);
		if(movementController.collisions.bottom)
			vertical = 0;

		anim.SetInteger("horizontal", horizontal);
		anim.SetInteger("vertical", vertical);
		anim.SetBool("grounded", movementController.collisions.bottom);
		anim.SetBool("doubleJumping", doubleJumping);
	}

	void UpdateFlip()
	{
		if(horizontal > 0)
		{
			// regarde vers la droite
			spriteRenderer.flipX = false;
		}
		else if (horizontal < 0)
		{
			// regarde vers la gauche
			spriteRenderer.flipX = true;
		}
	}

	void UpdateJump()
	{
		if (movementController.collisions.bottom)
		{
			jumpCount = 0;
			doubleJumping = false;
		}

		if (Input.GetKeyDown(KeyCode.Space) &&
		    jumpCount <= maxAirJump)
		{
			Jump();
		}
	}

	IEnumerator DoubleJumpCoroutine()
	{
		doubleJumping = true;
		anim.Play("FrogDoubleJumping");

		while(!anim.GetCurrentAnimatorStateInfo(0).IsName("FrogDoubleJumping"))
		{
			yield return null;
		}

		while (true)
		{
			if(!anim.GetCurrentAnimatorStateInfo(0).IsName("FrogDoubleJumping") ||
				movementController.collisions.bottom)
				break;
			yield return null;
		}
		doubleJumping = false;
	}

	void Jump()
	{
		if(!movementController.collisions.bottom)
		{
			StartCoroutine(DoubleJumpCoroutine());

			// Add one more jumpCount if falling without previous jump
			if(jumpCount == 0)
				jumpCount++;
		}

		jumpCount++;
		velocity.y = jumpForce;
	}
}
