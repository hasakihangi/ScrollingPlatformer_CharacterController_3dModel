using System;
using Unity.VisualScripting;
using UnityEngine;

public class ActionController: MonoBehaviour
{
	[SerializeField] private bool debug;
	public Enemy[] enemiesUnderAttack;

	public Bounds attackBox;
	public Vector2 min;
	
	// 引用List<TimelineAsset>和PlayableDirector, 播放Timeline
	
	// 利用传入的AABB数组进行范围检测, 顺便用Debug进行画框
	public void AABBOverlap(AABB[] aabbs)
	{
		// 这些AABB是相对于人物局部坐标的, Debug前需要转换到世界坐标, Debug后需要转换回去
		// if (debug)
		// {
		// 	foreach (var aabb in aabbs)
		// 	{
		// 		Vector3 minInWorld = transform.TransformPoint(aabb.min.x,
		// 			aabb.min.y, 0);
		// 		Vector3 maxInWorld = transform.TransformPoint(aabb.max.x,
		// 			aabb.max.y, 0);
		// 		Color color = new Color(0.7f, 0.4f, 0.05f);
		//
		// 		Vector3 point1 = new Vector3(maxInWorld.x, minInWorld.y, 0);
		// 		Vector3 point3 = new Vector3(minInWorld.x, maxInWorld.y, 0);
		// 		
		// 		Debug.DrawLine(minInWorld, point1, color);
		// 		Debug.DrawLine(point1, maxInWorld, color);
		// 		Debug.DrawLine(maxInWorld, point3);
		// 		Debug.DrawLine(point3, minInWorld);
		// 	}
		// }
	}

	// void OnDrawGizmos()
	// {
	// 	Gizmos.matrix = transform.localToWorldMatrix;
	//
	// 	Vector3 point1 = new Vector3(attackBox.min.x, attackBox.min.y, 0);
	// 	Vector3 point2 = new Vector3(attackBox.max.x, attackBox.min.y, 0);
	// 	Vector3 point3 = new Vector3(attackBox.max.x, attackBox.max.y, 0);
	// 	Vector3 point4 = new Vector3(attackBox.min.x, attackBox.min.y, 0);
	//
	// 	Vector3 center = new Vector3(attackBox.center.x, attackBox.center.y, 0);
	// 	Vector3 size = new Vector3(attackBox.size.x, attackBox.size.y, 0);
	//
	// 	Gizmos.color = new Color(0.8f, 0.4f, 0.1f);
	// 	Gizmos.DrawWireCube(center, size);
	// 	
	// 	Gizmos.matrix = Matrix4x4.identity;
	// }

	// private void OnDrawGizmosSelected()
	// {
	// 	Gizmos.color = new Color(0.8f, 0.4f, 0.4f);
	// 	Gizmos.matrix = transform.localToWorldMatrix;
	// 	Gizmos.DrawSphere(min, 0.05f);
	// }
}
