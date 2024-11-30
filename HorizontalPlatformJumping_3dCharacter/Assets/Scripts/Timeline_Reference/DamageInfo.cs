using System.Collections.Generic;
using UnityEngine;

namespace TimelineReference
{
	public class DamageInfo
	{
		public GameObject attacker;
		public GameObject defender;
		public DamageInfoTag[] tags;

		public Damage damage;
		public float criticalRate;

		public float degree;
		public List<AddBuffInfo> addBuffs = new List<AddBuffInfo>();

		public DamageInfo(GameObject attacker, GameObject defender, Damage damage, float damageDegree, float
			baseCriticalRate, DamageInfoTag[] tags)
		{
			this.attacker = attacker;
			this.defender = defender;
			this.damage = damage;
			this.criticalRate = baseCriticalRate;
			this.degree = damageDegree;
			this.tags = new DamageInfoTag[tags.Length];
			for (int i = 0; i < tags.Length; i++){
				this.tags[i] = tags[i];
			}
		}
		
	}

	/// <summary>
	/// 既作为伤害, 也作为治疗; 伤害为正值, 治疗为负值
	/// </summary>
	public struct Damage
	{
		// 三种类型: 子弹, 爆炸, 精神
		public int bullet;
		public int explosion;
		public int mental;

		public Damage(int bullet, int explosion = 0, int mental = 0)
		{
			this.bullet = bullet;
			this.explosion = explosion;
			this.mental = mental;
		}

		public int Overall(bool asHeal = false)
		{
			return (asHeal == false)
				? (Mathf.Max(0, bullet) + Mathf.Max(0, explosion) + Mathf.Max(0, mental))
				: Mathf.Min(0, bullet) + Mathf.Min(0, explosion) + Mathf.Min(0, mental);
		}
		
		public static Damage operator +(Damage a, Damage b){
			return new Damage(
				a.bullet + b.bullet,
				a.explosion + b.explosion,
				a.mental + b.mental
			);
		}
		
		public static Damage operator *(Damage a, float b){
			return new Damage(
				Mathf.RoundToInt(a.bullet * b),
				Mathf.RoundToInt(a.explosion * b),
				Mathf.RoundToInt(a.mental * b)
			);
		} 
	}
	
	public enum DamageInfoTag
	{
		directDamage = 0,
		periodDamage = 1,
		reflectDamage = 2,
		directHeal = 10,
		periodHeal = 11,
		//...
	}
}

