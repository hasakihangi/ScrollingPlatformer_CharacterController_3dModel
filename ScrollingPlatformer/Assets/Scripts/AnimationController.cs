using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class AnimationController : MonoBehaviour
{
	[SerializeField] private bool debug = true;
	
	private Animator animator;
	private InputController input;
	private Player player;
	private CameraController cameraController;
	
	public float rotateTime = 0.2f;
    
	private Quaternion rightRotation = Quaternion.Euler(0, 90, 0);
	private Quaternion leftRotation = Quaternion.Euler(0, 270, 0);

	private float time = 0f;
	private bool isRotating = false;
	private Quaternion targetRotation;
	private Quaternion startRotation;

	#region Animator Parameter

	private int speedXParameter;
	private int speedYParameter;
	private int JumpParameter;

	#endregion
	
	#region Observe IK
	
	private bool isObserveInput;
	private float observeInputValue;
	private float ikWeight;
	private bool ikOn;

	[SerializeField] private Vector2 ikPointHeadUp;
	[SerializeField] private Vector2 ikPointHeadDown;
	private Vector2 ikPointWorld;
	private Vector2 ikPointLocal;
	
	[SerializeField] private float ikWeightChangeSpeed = 2f;
	[SerializeField] private float ikPointYChangeSpeed = 3f;
	[SerializeField, Range(0,1)] private float bodyWeight = 0.1f;
	[SerializeField, Range(0,1)] private float headWeight = 1f;
	
	#endregion
	
	

	
	
	void OnAnimatorIK()
	{
		animator.SetLookAtWeight(ikWeight, bodyWeight, headWeight);
		animator.SetLookAtPosition(ikPointWorld);
	}
	
	private void Start()
	{
		animator = GetComponent<Animator>();
		player = GetComponentInParent<Player>();
		input = GetComponentInParent<InputController>();
		if (cameraController == null)
			cameraController = Camera.main.GetComponent<CameraController>();
		
		speedXParameter = Animator.StringToHash("SpeedX");
		speedYParameter = Animator.StringToHash("SpeedY");
		JumpParameter = Animator.StringToHash("Jump");
		
		ikOn = true;
	}

	private void Update()
	{
		#region Rotate

		if (isRotating)
		{
			RotateObject();
		}

		#endregion

		#region Observe IK

		input.CheckObserveInput(ref isObserveInput);
		input.CheckObserveInput(ref observeInputValue);
		
		if (player.IsOnGround && isObserveInput)
		{
			ikOn = true;
			if (observeInputValue > 0.1f)
			{
				ikPointLocal.y = Mathf.Min(ikPointHeadUp.y, ikPointLocal.y + 
					ikPointYChangeSpeed * Time.deltaTime);
				ikPointLocal.x = ikPointHeadUp.x;
			}
			else if (observeInputValue < -0.1f)
			{
				ikPointLocal.y = Mathf.Max(ikPointHeadDown.y, ikPointLocal.y - 
					ikPointYChangeSpeed * Time.deltaTime);
				ikPointLocal.x = ikPointHeadDown.x;
			}
		}
		else
		{
			ikOn = false;
			if (ikPointLocal.y > 0f)
			{
				ikPointLocal.y = Mathf.Max(0,
					ikPointLocal.y - ikPointYChangeSpeed * Time.deltaTime);
			}
			else if (ikPointLocal.y < 0f)
			{
				ikPointLocal.y = Mathf.Min(0,
					ikPointLocal.y + ikPointYChangeSpeed * Time.deltaTime);
			}
		}

		ikPointWorld = new Vector2(transform.position.x, transform.position
			.y) + new Vector2(ikPointLocal.x * player.FacingDirection,
			ikPointLocal.y);
		
		if (ikOn)
			ikWeight += ikWeightChangeSpeed * Time.deltaTime;
		else
			ikWeight -= ikWeightChangeSpeed * Time.deltaTime;

		ikWeight = Mathf.Clamp(ikWeight, 0, 1);

		#endregion
		
		
	}
	
	private void RotateObject()
	{
		if (time <= rotateTime)
		{
			Quaternion currentRotation = Quaternion.Slerp(startRotation, targetRotation, time / rotateTime);
			transform.localRotation = currentRotation;
			time += Time.deltaTime;
		}
		else
		{
			isRotating = false;
			transform.localRotation = targetRotation;
		}
	}
    
	public void SetTargetDirection(int direction)
	{
		time = 0f;
		isRotating = true;
		startRotation = transform.localRotation;
		targetRotation = direction == 1 ? rightRotation : leftRotation;
	}

	public void SetMoveAnimatorParameters(float velocityX,float 
            maxSpeed)
	{
		float speed = Mathf.Abs(velocityX) * (1f / maxSpeed);
		animator.SetFloat(speedXParameter, speed);
	}

	public void SetJumpAnimatorParameters(bool isJump, float velocityY,
		Vector2 speedRange)
	{
		animator.SetBool(JumpParameter, isJump);
		float speed = (velocityY - speedRange.x) * (1f - 0f) / 
			(speedRange.y - speedRange.x) + 0f;
		// speed = Mathf.Clamp01(speed);
		animator.SetFloat(speedYParameter, speed);
	}

	#region Animation Events

	internal void FootL()
	{
		
	}

	internal void FootR()
	{
		
	}

	#endregion
	
	void OnDrawGizmos()
	{
		if (debug)
		{
			
		}
	}
}
