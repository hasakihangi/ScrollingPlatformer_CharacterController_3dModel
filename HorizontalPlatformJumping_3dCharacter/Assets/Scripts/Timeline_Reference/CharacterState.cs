using System.Collections.Generic;
using UnityEngine;

namespace TimelineReference
{
	public class CharacterState: MonoBehaviour
	{
		private ChaControlState _controlState = ChaControlState.origin;
		
		/// <summary>
		/// TimelineObj的ChaControlState
		/// </summary>
		public ChaControlState TimelineControlState = ChaControlState.origin;

		public ChaControlState ControlState => this._controlState + this.TimelineControlState;

		/// <summary>
		/// 角色的无敌状态时间, 如果在无敌状态中, 子弹不会碰撞, DamageInfo处理无效化
		/// 单位: s
		/// </summary>
		/// <returns></returns>
		public float immuneTime
		{
			get => _immuneTime;
			set => _immuneTime = Mathf.Max(_immuneTime, value);
		}
		private float _immuneTime = 0f;

		/// <summary>
		/// 角色蓄力状态
		/// </summary>
		public bool charging = false;

		/// <summary>
		/// 角色主动期望的移动方向
		/// </summary>
		public float moveDegree => _wishToMoveDegree;

		private float _wishToMoveDegree = 0f;

		private float _wishToFaceDegree = 0f;

		public float faceDegree => _wishToFaceDegree;

		public bool dead = false;
		
		/// <summary>
		/// 移动请求信息
		/// </summary>
		private Vector3 moveOrder = new Vector3();

		private List<MovePreorder> forceMove = new List<MovePreorder>();

		private List<string> animOrder = new List<string>();

		private float rotateToOrder;

		private List<float> forceRotate = new List<float>();

		public ChaResource resource = new ChaResource(1);

		/// <summary>
		/// 阵营
		/// </summary>
		[Tooltip("阵营")] public int side = 0;

		public string[] tags = new string[0];
		
		private ChaProperty _prop = ChaProperty.zero;

		public ChaProperty Property => _prop;

		public float moveSpeed => 0f;

		public float actionSpeed => 0f;

		public ChaProperty baseProp = new ChaProperty(100, 100, 0, 20, 100);
		
		/// <summary>
		/// 通过buff增加的角色属性, 两类属性:plus和times(倍乘), 属性 = (属性 + plus) * times
		/// </summary>
		public ChaProperty[] buffProp = new ChaProperty[2] { ChaProperty.zero, ChaProperty.zero };

		public ChaProperty equipmentProp = ChaProperty.zero;

		public List<SkillObj> skills = new List<SkillObj>();
	}
}