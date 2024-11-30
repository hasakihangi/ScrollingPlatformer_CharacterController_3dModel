using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PhysicsController : MonoBehaviour
{
	[SerializeField] private bool debug = false;

	public LayerMask collisionMask;
	public float skinWidth = 0.015f;

	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	private float horizontalRaySpacing;
	private float verticalRaySpaceing;

	private new BoxCollider2D collider;
	private RaycastOrigins raycastOrigins;
	public CollisionInfo collisionInfo;

	private bool belowLastDebug;

	void Start()
	{
		collider = GetComponent<BoxCollider2D>();
		CalculateRaySpacing();
	}

	private void Update()
	{
		if (debug)
		{
			if (belowLastDebug != collisionInfo.below)
			{
				print("collisionInfo.below: " + collisionInfo.below);
				belowLastDebug = collisionInfo.below;
			}
		}
	}

	//public void Move(Vector2 velocity)
	//{
	//	UpdateRaycastOrigins();
	//	// collisionInfo.Reset();
	//	if (velocity.x != 0)
	//	{
	//		HorizontalCollisions(ref velocity);
	//	}

	//	if (velocity.y != 0)
	//	{
	//		VerticalCollisions(ref velocity);
	//	}

	//	transform.Translate(velocity);
	//	// 如果检测到地面,则将velocity置为刚好到达地面的值并且将collisionInfo.below置为true
	//}

	// 根据velocity和acceleration, 修改position的值
	//public void UpdatePosition(ref Vector2 position, Vector2 velocity,
	//	Vector2 acceleration)
	//{
	//}

	//public void UpdateVelocity(ref Vector2 velocity, Vector2 acceleration)
	//{
	//}

	public void HorizontalCollisions(ref Vector2
		displacement)
	{
		float direction = Mathf.Sign(displacement.x);
		float rayLength = Mathf.Abs(displacement.x) + skinWidth;
		Vector2 origin = direction < 0f
			? raycastOrigins
				.bottomLeft
			: raycastOrigins.bottomRight;
		Vector2 rayOrigin;
		for (int i = 0; i < horizontalRayCount; i++)
		{
			rayOrigin = origin + Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction *
				Vector2.right, rayLength, collisionMask);
			if (debug)
			{
				Vector2 end = rayOrigin + direction * rayLength * Vector2.right;
				Debug.DrawLine(new Vector3(rayOrigin.x, rayOrigin.y, 0),
					new Vector3(end.x, end.y, 0));
			}

			if (hit)
			{
				displacement.x = (hit.distance - skinWidth) * direction;
				collisionInfo.left = direction < 0;
				collisionInfo.right = direction > 0;
				return;
			}
		}
	}


	public void VerticalCollisions(ref Vector2
		displacement)
	{
		float direction = Mathf.Sign(displacement.y);
		float rayLength = Mathf.Abs(displacement.y) + skinWidth;
		Vector2 origin = direction < 0
			? raycastOrigins.bottomLeft
			: raycastOrigins.topLeft;
		Vector2 rayOrigin;
		for (int i = 0; i < verticalRayCount; i++)
		{
			rayOrigin = origin + Vector2.right * (verticalRaySpaceing * i +
												  displacement.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up *
				direction, rayLength, collisionMask);

			if (debug)
			{
				Vector2 end = rayOrigin + direction *
					rayLength * Vector2.up;
				Debug.DrawLine(new Vector3(rayOrigin.x, rayOrigin.y, 0), new
					Vector3(end.x, end.y, 0));
			}

			if (hit)
			{
				displacement.y =
					(hit.distance - skinWidth) * direction;
				collisionInfo.below = direction < 0;
				collisionInfo.above = direction > 0;
				return;
			}
		}
	}

	public void UpdateRaycastOrigins()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(skinWidth * -2);
		raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	private void CalculateRaySpacing()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(skinWidth * -2); // 'AABB'向中间缩一圈skinWidth
		horizontalRayCount = Mathf.Max(2, horizontalRayCount);
		verticalRayCount = Mathf.Max(2, verticalRayCount);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpaceing = bounds.size.x / (verticalRayCount - 1);
	}

	// 该数据是向内缩进后的bounds
	struct RaycastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0f, 0.5f, 0.2f, 1f);
		Gizmos.DrawLine(raycastOrigins.bottomLeft, raycastOrigins.bottomRight);
		Gizmos.DrawLine(raycastOrigins.bottomRight, raycastOrigins.topRight);
		Gizmos.DrawLine(raycastOrigins.topRight, raycastOrigins.topLeft);
		Gizmos.DrawLine(raycastOrigins.topLeft, raycastOrigins.bottomLeft);
	}
}


public struct CollisionInfo
{
	public bool above, below;
	public bool left, right;

	public void Reset()
	{
		above = below = false;
		left = right = false;
	}
}