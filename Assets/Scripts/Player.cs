using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(AnimationTimes))]
public class Player : MonoBehaviour
{
	delegate void MyDelegate();
	MyDelegate UpdateAnimation;

	[Header("Run speed")]
	[Tooltip("Number of meter by second")]
	public float maxSpeed;
	public float timeToMaxSpeed;

	[Header("Jump")]
	public uint maxAirJump;
	[Tooltip("Unity value of max jump height")]
	public float jumpHeight;
	[Tooltip("Time in seconds to reach the jump height")]
	public float timeToMaxJump;
	[Tooltip("Can i change direction in air?")]
	[Range(0, 1)]
	public float airControl;
	[Tooltip("Minimum time on ascension in jump")]
	public float minAscensionTime;

	[Tooltip("Minimum y position before dying")]
	public float holeLimit;

	[Header("Sounds")]
	public List<string> FootstepSounds;
	string lastFootstep;

	[Header("Other")]
	public bool animationByParameters;

	int doubleJumpCount;

	float acceleration;
	float minSpeedThreshold;

	float gravity;
	float jumpForce;
	float maxFallingSpeed;
	int horizontal = 0;
	bool doubleJumping;

	Animator anim;
	SpriteRenderer spriteRenderer;
	LifeCountGUI lifeCountGUI;

	[HideInInspector]
	public MovementController movementController;
	AnimationTimes animationTimes;

	public bool freeze { get; private set; }

	Vector2 velocity = new Vector2();
	private Dictionary<string, Vector2> externalVelocity = new Dictionary<string, Vector2>();
	private Vector2 velocityCalculated
	{
		get
		{
			Vector2 _velocity = velocity;
			foreach (Vector2 vel in externalVelocity.Values)
			{
				_velocity += vel;
			}
			return _velocity;
		}
	}

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

		minSpeedThreshold = acceleration / Application.targetFrameRate * 2f;
		movementController = GetComponent<MovementController>();
		anim = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animationTimes = GetComponent<AnimationTimes>();
		lifeCountGUI = FindObjectOfType<LifeCountGUI>();

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
		UpdateHorizontalControl();
		UpdateGravity();
		UpdateJump();
		UpdateFlip();
		UpdateFallInHOle();

		movementController.Move(velocityCalculated * Time.deltaTime);

		UpdateAnimation();
	}

	void UpdateHorizontalControl()
	{
		// Reset velocity at start of frame is hitting wall
		// Then I will add one frame of velocity to stay sticking on wall for example
		// But I want my speed to stop when reaching wall
		if((velocity.x > 0 && movementController.collisions.right) ||
			(velocity.x < 0 && movementController.collisions.left))
		{
			velocity.x = 0;
		}

		horizontal = 0;

		if((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && !freeze)
		{
			horizontal += 1;
		}
		if((Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) && !freeze)
		{
			horizontal -= 1;
		}
		
		float controlModifier = 1f;
		if(!movementController.collisions.bottom)
		{
			controlModifier = airControl;
		}

		velocity.x += horizontal * acceleration * controlModifier * Time.deltaTime;

		if(Mathf.Abs(velocity.x) > maxSpeed)
			velocity.x = maxSpeed * horizontal;


		if(horizontal == 0)
		{
			if(velocity.x > minSpeedThreshold)
				velocity.x -= acceleration * Time.deltaTime;
			else if(velocity.x < -minSpeedThreshold)
				velocity.x += acceleration * Time.deltaTime;
			else
				velocity.x = 0;
		}
	}

	void UpdateGravity()
	{
		if(movementController.collisions.bottom || movementController.collisions.top)
			velocity.y = 0;


		if((movementController.collisions.left || movementController.collisions.right) && velocity.y < 0)
		{
			velocity.y += gravity * Time.deltaTime / 3f;
		}
		else
		{
			velocity.y += gravity * Time.deltaTime;
		}

		if(velocity.y < maxFallingSpeed)
		{
			velocity.y = maxFallingSpeed;
		}
	}

	void UpdateAnimationByCode()
	{
		if(freeze)
			return;

		// Au sol
		if (movementController.collisions.bottom)
		{
			if(horizontal == 0)
				anim.Play("FrogIdle");
			else if(horizontal != 0)
				anim.Play("FrogRun");
		}
		// En l'air
		else
		{
			if (!doubleJumping)
			{
				if (movementController.collisions.left ||
					movementController.collisions.right)
					anim.Play("FrogWallSticking");
				else if(velocity.y > 0)
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
		if(freeze)
			return;

		if(velocity.x > 0)
		{
			// regarde vers la droite
			spriteRenderer.flipX = false;
		}
		else if (velocity.x < 0)
		{
			// regarde vers la gauche
			spriteRenderer.flipX = true;
		}
	}

	void UpdateJump()
	{
		if (movementController.collisions.bottom)
		{
			ResetDoubleJumpCount();
			doubleJumping = false;
		}

		if (Input.GetKeyDown(KeyCode.Space) && !freeze)
		{
			// Normal jump
			if(movementController.collisions.bottom || movementController.collisions.bottomBuffer)
			{
				Jump();
			}
			// Wall jump
			else if(
				!movementController.collisions.bottom &&
				(movementController.collisions.left ||
				 movementController.collisions.leftBuffer ||
				 movementController.collisions.right ||
				 movementController.collisions.rightBuffer))
			{
				WallJump();
			}
			// Normal or airJump
			else if(doubleJumpCount < maxAirJump && !movementController.collisions.bottom)
			{
				DoubleJump();
			}
		}
	}

	Coroutine jumpCoroutine;
	public void Jump()
	{
		if (jumpCoroutine != null)
		{
			StopCoroutine(jumpCoroutine);
		}

		jumpCoroutine = StartCoroutine(JumpCoroutine());
	}

	IEnumerator JumpCoroutine()
	{
		SoundManager.Instance.PlaySoundEffect("player_jump");
		velocity.y = jumpForce;

		float time = 0;
		while (time < minAscensionTime)
		{
			time += Time.deltaTime;
			yield return null;
		}

		while (true)
		{
			if (!Input.GetKey(KeyCode.Space))
			{
				break;
			}
			if (velocity.y <= 0)
			{
				break;
			}
			yield return null;
		}

		velocity.y = 0;
	}

	void WallJump()
	{
		int direction = movementController.collisions.leftBuffer ? 1 : -1;
		velocity.x = maxSpeed * direction;
		Jump();
	}

	void DoubleJump()
	{
		StartCoroutine(DoubleJumpCoroutine());
	}

	public void ResetDoubleJumpCount()
	{
		doubleJumpCount = 0;
	}

	IEnumerator DoubleJumpCoroutine()
	{
		Jump();
		doubleJumpCount++;
		doubleJumping = true;
		anim.Play("FrogDoubleJumping");

		while(!anim.GetCurrentAnimatorStateInfo(0).IsName("FrogDoubleJumping"))
		{
			yield return null;
		}

		while(true)
		{
			if(!anim.GetCurrentAnimatorStateInfo(0).IsName("FrogDoubleJumping") ||
				movementController.collisions.bottom)
				break;
			yield return null;
		}
		doubleJumping = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Enemy enemy = collision.gameObject.GetComponent<Enemy>();

		if (enemy != null)
		{
			HitEnemy(enemy);
		}
	}

	Coroutine hitEnemy;
	void HitEnemy(Enemy enemy)
	{
		if (hitEnemy == null)
			hitEnemy = StartCoroutine(HitEnemyCoroutine(enemy));
	}

	IEnumerator HitEnemyCoroutine(Enemy enemy)
	{
		yield return new WaitForEndOfFrame();

		if(enemy.dangerous)
		{
			SoundManager.Instance.LowerMusicPitch(animationTimes.GetTime("FrogHit"));
			SoundManager.Instance.PlaySoundEffect("player_die");

			velocity.x = enemy.pushBackForce * Mathf.Sign(transform.position.x - enemy.transform.position.x);
			anim.Play("FrogHit");
			freeze = true;

			// Wait for the time of the FrogHit animation to be finished
			yield return new WaitForSeconds(animationTimes.GetTime("FrogHit"));

			freeze = false;
			Die();
		}
		hitEnemy = null;
	}

	void Die()
	{
		Game.Instance.lifeCount--;
		if(Game.Instance.lifeCount <= 0)
		{
			SceneManager.LoadScene("game_over");
			return;
		}

		SpawnPlayer spawnPlayer = FindObjectOfType<SpawnPlayer>();
		SoundManager.Instance.SetMusicPitch(1f);
		spawnPlayer.Spawn();

		lifeCountGUI.RefreshLifeCount();
		Destroy(gameObject);
	}

	void UpdateFallInHOle()
	{
		if (transform.position.y < holeLimit)
		{
			Die();
		}
	}

	public void Freeze()
	{
		freeze = true;
		anim.Play("FrogIdle");
	}

	public void AddExternalVelocity(string id, Vector2 _velocity)
	{
		// Add new value
		if (!externalVelocity.ContainsKey(id))
		{
			externalVelocity.Add(id, _velocity);
		}
		// Set existing value
		else
		{
			externalVelocity[id] = _velocity;
		}
	}

	public void RemoveExternalVelocity(string id)
	{
		if(externalVelocity.ContainsKey(id))
		{
			externalVelocity.Remove(id);
		}
	}

	void AnimationPlayFootStep()
	{
		// Do not play the previous footstep sound
		FootstepSounds.Remove(lastFootstep);

		string randomFootStep = FootstepSounds[Random.Range(0, FootstepSounds.Count)];
		SoundManager.Instance.PlaySoundEffect(randomFootStep);

		if (lastFootstep != null)
			FootstepSounds.Add(lastFootstep);

		lastFootstep = randomFootStep;
	}
}
