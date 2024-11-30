using UnityEngine;

namespace TimelineReference
{
	public class MovePreorder
	{
		public Vector3 velocity;

		/// <summary>
		/// 完成该移动的时间
		/// </summary>
		private float inTime;

		/// <summary>
		/// 还剩多久时间完成该移动
		/// </summary>
		public float duration;

		public MovePreorder(Vector3 velocity, float duration)
		{
			this.velocity = velocity;
			this.duration = duration;
			this.inTime = duration;
		}

		/// <summary>
		/// 在
		/// </summary>
		/// <param name="deltaTime"></param>
		/// <returns></returns>
		public Vector3 VeloInTime(float deltaTime)
		{
			if (deltaTime >= duration)
			{
				this.duration = 0;
				// 为什么?
				// 如果duration小于该帧时间, 则该帧处理之后变为duration变为0
				
			}
			else
			{
				this.duration -= deltaTime;
			}

			return inTime <= 0 ? velocity : (velocity / inTime);
			// 若inTime=0, 则是瞬间移动
		}
	}

	public enum MoveType
	{
		ground = 0,
		fly = 1
	}
}