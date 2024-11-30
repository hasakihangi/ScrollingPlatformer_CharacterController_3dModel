using UnityEngine;

namespace TimelineReference
{
	/// <summary>
	/// 角色数值
	/// </summary>
	public class ChaProperty
	{
		public int hp;
		public int attack;
		public int moveSpeed;
		
		/// <summary>
		/// timeline和动画播放的scale
		/// </summary>
		public int actionSpeed;

		/// <summary>
		/// 弹仓
		/// </summary>
		public int ammo;

		public float bodyRadius;

		public float hitRadius;

		public MoveType moveType;

		public ChaProperty(
			int moveSpeed,
			int hp = 0,
			int ammo = 0,
			int attack = 0,
			int actionSpeed = 100,
			float bodyRadius = 0.25f,
			float hitRadius = 0.25f,
			MoveType moveType = MoveType.ground
		)
		{
			this.moveSpeed = moveSpeed;
			this.hp = hp;
			this.ammo = ammo;
			this.attack = attack;
			this.actionSpeed = actionSpeed;
			this.bodyRadius = bodyRadius;
			this.hitRadius = hitRadius;
			this.moveType = moveType;
		}

		public static ChaProperty zero = new ChaProperty(0, 0, 0, 0, 0, 0, 0, 0);
		
		public void Zero(MoveType moveType = MoveType.ground){
			this.hp = 0;
			this.moveSpeed = 0;
			this.ammo = 0;
			this.attack = 0;
			this.actionSpeed = 0;
			this.bodyRadius = 0;
			this.hitRadius = 0;
			this.moveType = moveType;
		}
		
		/// <summary>
		/// 可以将一个ChaProperty对象表达为一个倍乘属性, 通过 * 进行运算
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static ChaProperty operator *(ChaProperty a, ChaProperty b){
			return new ChaProperty(
				Mathf.RoundToInt(a.moveSpeed * (1.0000f + Mathf.Max(b.moveSpeed, -0.9999f))),
				Mathf.RoundToInt(a.hp * (1.0000f + Mathf.Max(b.hp, -0.9999f))),
				Mathf.RoundToInt(a.ammo * (1.0000f + Mathf.Max(b.ammo, -0.9999f))),
				Mathf.RoundToInt(a.attack * (1.0000f + Mathf.Max(b.attack, -0.9999f))),
				Mathf.RoundToInt(a.actionSpeed * (1.0000f + Mathf.Max(b.actionSpeed, -0.9999f))),
				a.bodyRadius * (1.0000f + Mathf.Max(b.bodyRadius, -0.9999f)),
				a.hitRadius * (1.0000f + Mathf.Max(b.hitRadius, -0.9999f)),
				a.moveType == MoveType.fly || b.moveType == MoveType.fly ? MoveType.fly : MoveType.ground
			);
		}
	}

	/// <summary>
	/// 角色资源
	/// </summary>
	public class ChaResource
	{
		public int hp;
		public int ammo;
		/// <summary>
		/// 耐力
		/// </summary>
		public int stamina;

		public ChaResource(int hp, int ammo = 0, int stamina = 0)
		{
			this.hp = hp;
			this.ammo = ammo;
			this.stamina = stamina;
		}

		public bool Enough(ChaResource requirement)
		{
			return (
				this.hp >= requirement.hp &&
				this.ammo >= requirement.ammo &&
				this.stamina >= requirement.stamina
				);
		}

		public static ChaResource operator +(ChaResource a, ChaResource b)
		{
			return new ChaResource(
				a.hp + b.hp,
				a.ammo + b.ammo,
				a.stamina + b.stamina
			);
		}

		public static ChaResource Null = new ChaResource(0);
	}
}