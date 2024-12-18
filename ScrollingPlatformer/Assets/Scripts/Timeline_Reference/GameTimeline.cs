using System.Collections.Generic;
using UnityEngine;

namespace TimelineReference
{
	public class TimelineObj
	{
		public TimelineModel model;
		public GameObject caster;
		
		private float _timeScale = 1.0f;
		public float timeScale
		{
			get
			{
				return _timeScale;
			}
			set
			{
				_timeScale = Mathf.Max(0.1f, value);
					// 0.1f是倍速的最小值
			}
		}
		
		/// <summary>
		/// 该timeline的处理对象
		/// </summary>
		public object param;

		public float timeElapsed = 0;
		
		/// <summary>
		/// 处理游戏逻辑需要的参数
		/// </summary>
		public Dictionary<string, object> values;

		public TimelineObj(TimelineModel model, GameObject caster, object param)
		{
			this.model = model;
			this.caster = caster;
			this.values = new Dictionary<string, object>();
			this._timeScale = 1.0f;
			if (caster)
			{
				
			}
		}
	}

	public struct TimelineModel
	{
		public string id;
		public TimelineNode[] nodes;
		
	}

	public struct TimelineNode
	{
		public float timeElapsed;
		public TimelineEvent doEvent;
	}

	public struct TimelineGoTo
	{
		public float atDuration;
		public float gotoDuration;

		public TimelineGoTo(float atDuration, float gotoDuration)
		{
			this.atDuration = atDuration;
			this.gotoDuration = gotoDuration;
		}

		public static TimelineGoTo Null = new TimelineGoTo(float.MaxValue, float.MaxValue);
	}

	public delegate void TimelineEvent(TimelineObj timeline, params object[] args);
}