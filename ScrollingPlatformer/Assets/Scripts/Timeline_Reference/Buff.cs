using System.Collections.Generic;
using UnityEngine;

namespace TimelineReference
{
	public struct AddBuffInfo
	{
		public GameObject caster;
		public GameObject target;
		public BuffModel buffModel;
		
		/// <summary>
		/// 要添加的层数, 负数即减少
		/// </summary>
		public int addStack;
		
		/// <summary>
		/// 持续时间, true表示"设置为", false表示"改变(增加或减少)"
		/// </summary>
		public bool durationSetTo;
		// 啥玩意?
		
		public bool permanent;

		public float duration;

		/// <summary>
		/// buff参数
		/// </summary>
		public Dictionary<string, object> buffParam;
		// string用于索引, 但是object不是完全没有指定做什么吗?
		// 可以作用到任何物体身上, 不同物体对参数的处理有不同的方式

		public AddBuffInfo(
			BuffModel model,
			GameObject caster,
			GameObject target,
			int stack,
			float duration,
			bool durationSetTo = true,
			bool permanent = false,
			Dictionary<string, object> buffParam = null
			)
		{
			this.buffModel = model;
			this.caster = caster;
			this.target = target;
			this.addStack = stack;
			this.duration = duration;
			this.durationSetTo = durationSetTo;
			this.buffParam = buffParam;
			this.permanent = permanent;
		}
		
	}

	/// <summary>
	/// 运行中实际存在的buff
	/// </summary>
	public class BuffObj
	{
		public BuffModel model;
		public float duration;
		public bool permanent;
		public int stack;
		public GameObject caster;
		
		/// <summary>
		/// buff的携带者
		/// </summary>
		public GameObject carrier;

		/// <summary>
		/// buff的存在时间
		/// </summary>
		public float timeElapsed = 0f;

		/// <summary>
		/// buff执行了多少次onTick
		/// </summary>
		public int tecked = 0;

		public Dictionary<string, object> buffParam = new Dictionary<string, object>();

		public BuffObj(
			BuffModel model, 
			GameObject caster,
			GameObject carrier,
			float duration,
			int stack,
			bool permanent = false,
			Dictionary<string, object> buffParam = null)
		{
			this.model = model;
			this.caster = caster;
			this.carrier = carrier;
			this.duration = duration;
			this.stack = stack;
			this.permanent = permanent;
			if (buffParam != null)
			{
				foreach (var variable in buffParam)
				{
					this.buffParam.Add(variable.Key, variable.Value);
				}
			}
		}
	}

	public class BuffModel
	{
		public string id;
		public string name;
		
		/// <summary>
		/// 优先级越低的buff越后面执行
		/// </summary>
		public int priority;

		/// <summary>
		/// buff堆叠的层数
		/// </summary>
		public int maxStack;

		public string[] tags;

		/// <summary>
		/// buff的工作周期, 单位: s
		/// >0时的最小值为Time.fixedDeltaTime
		/// </summary>
		public float tickTime;

		/// <summary>
		/// 给角色添加的属性, plus类型和times类型
		/// </summary>
		public ChaProperty[] propMod;

		public BuffOnOccur OnOccur;
		public object[] onOccurParams;
		
		
	}

	public delegate void BuffOnOccur(BuffObj buff, int modifyStack);

	public delegate void BuffOnRemoved(BuffObj buff);

	public delegate void BuffOnTick(BuffObj buff);

	public delegate void BuffOnHit(BuffObj buff, ref DamageInfo damageInfo, GameObject target);

	public delegate void BuffOnBeHurt(BuffObj buff, ref DamageInfo damageInfo, GameObject attacker);

	public delegate void BuffOnKill(BuffObj buff, DamageInfo damageInfo, GameObject target);

	public delegate void BuffOnBeKilled(BuffObj buff, DamageInfo damageInfo, GameObject attacker);

	public delegate TimelineObj BuffOnCast(BuffObj buff, SkillObj skill, TimelineObj timeline);
}