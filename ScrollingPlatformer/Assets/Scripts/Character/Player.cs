using System;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] private bool debug = true;
	[SerializeField] private bool debugState = true;
	[SerializeField] private bool debugVelocityY = true;

	#region Configuration
	public float jumpHeight = 4;
	public float timeToJumpApex = 0.4f;
	public float moveSpeed = 6;
	private float accelerationTimeAirborne = 0.1f;
	private float accelerationTimeGrounded = 0.05f;
	#endregion

	#region Configuration calculation
	private float gravity;
	private float jumpVelocity;
	#endregion

	#region Run-time Paramters

	private Vector2 velocity;
	private float _velocityXSmoothing;

	// 1表示向右
	private int facingDirection = 1;
	public int FacingDirection => facingDirection;

	#endregion

	#region Input Parameters
	private float moveInputValue;
	private bool jumpButtonBool;
	#endregion

	// 移动值
	private float moveValue;

	#region Reference Component

	private PhysicsController physicsController;
	private InputController inputController;
	private AnimationController animationController;
	private new Rigidbody2D rigidbody;
	private EffectController effectController;

	#endregion

	#region External Use Variables

	private bool isOnGround;
	public bool IsOnGround => isOnGround;

	#endregion

	[SerializeField] private Vector2 speedYRange = new Vector2(-5, 10);

	void Start()
	{
		// 组件引用获取
		physicsController = GetComponent<PhysicsController>();
		inputController = GetComponent<InputController>();
		animationController = GetComponentInChildren<AnimationController>();
		effectController = GetComponent<EffectController>();

		// 计算配置数据
		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		rigidbody = GetComponent<Rigidbody2D>();

		if (debug)
			print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);

		// 逻辑初始化
		physicsController.collisionInfo.Reset();
	}

	private void FixedUpdate()
	{
		if (physicsController.collisionInfo.below)
		{
			if (jumpButtonBool)
			{
				velocity.y = jumpVelocity;
				physicsController.collisionInfo.below = false;
				jumpButtonBool = false;
				isOnGround = false;
			}
			else
			{
				velocity.y = 0;
				isOnGround = true;
			}
		}
		else
		{
			isOnGround = false;
		}

		velocity.y += gravity * Time.fixedDeltaTime;

		float targetVelocityX = moveInputValue * moveSpeed;

		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX,
			ref _velocityXSmoothing,
			(physicsController.collisionInfo.below)
				? accelerationTimeGrounded
				: accelerationTimeAirborne
		);

		Vector2 displaceThisFrame = velocity * Time.fixedDeltaTime;

		physicsController.UpdateRaycastOrigins();

		physicsController.collisionInfo.Reset();

		physicsController.HorizontalCollisions(ref displaceThisFrame);
		physicsController.VerticalCollisions(ref displaceThisFrame);

		rigidbody.MovePosition(rigidbody.position + displaceThisFrame);

		inputController.ResetButton(ref jumpButtonBool);
	}

	private bool debugLastFrame;

	void Update()
	{
		#region Debug GroundState

		if (debugState)
		{
			if (IsOnGround != debugLastFrame)
			{
				print(IsOnGround);
				debugLastFrame = IsOnGround;
			}
		}

		#endregion

		inputController.CheckMoveInput(ref moveInputValue, ref jumpButtonBool);

		#region Debug Rotate
		// if ( debug && Input.GetKeyDown(KeyCode.R))
		// {
		// 	facingDirection = -facingDirection;
		// 	animationController.SetTargetDirction(facingDirection);
		// }
		#endregion

		CheckFacingDirection();

		animationController.SetMoveAnimatorParameters(velocity.x, moveSpeed);

		if (!isOnGround)
		{
			animationController.SetJumpAnimatorParameters(true, velocity.y,
				speedYRange);
		}
		else
		{
			animationController.SetJumpAnimatorParameters(false, 0,
				speedYRange);
		}

		effectController.SetEffectState(ref velocity);

		#region Debug velocity.y

		if (debugVelocityY)
		{
			print(velocity.y);
		}

		#endregion
	}

	void CheckFacingDirection()
	{
		if (facingDirection == 1 && velocity.x < 0f)
		{
			facingDirection = -1;
			animationController.SetTargetDirection(facingDirection);
		}
		else if (facingDirection == -1 && velocity.x > 0f)
		{
			facingDirection = 1;
			animationController.SetTargetDirection(facingDirection);
		}
	}

}