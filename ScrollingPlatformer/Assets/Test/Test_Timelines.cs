using System;
using UnityEngine;

namespace Test
{
	public class Test_Timelines : MonoBehaviour
	{
		private TimelineManager timelineManager;

		public uint currentStopId = 100;
		
		private bool stopInput;
		private bool addInput;

		private void StartHandle(TimelineObj timeline)
		{
			Debug.Log($"timeline {timeline.id}: start!");
		}

		private void Expire(TimelineObj timeline)
		{
			Debug.Log($"timeline {timeline.id}: expire!");
			timeline.manager.AddToAddList(timeline.model, this.gameObject);
		}

		private void Stop(TimelineObj timeline)
		{
			Debug.Log($"timeline {timeline.id}: stop!");
		}

		private void Tick(GameObject caster, TimelineObj timeline, float deltaTime)
		{
			float current = Mathf.Repeat(timeline.currentTime, 1f);
			float last = Mathf.Repeat(timeline.lastTime, 1f);

			if (last > current)
			{
				caster.transform.Translate(new Vector3(0,0,0.1f));
				Debug.Log($"currentTime = {timeline.currentTime}, tick!");
			}
		}

		private void NodeEventOn3F(TimelineObj timeline)
		{
			Debug.Log($"currentTime = {timeline.currentTime}, timeline Event!");
		}
		
		private TimelineNode node;
		private TimelineModel model;
		
		private void Awake()
		{
			timelineManager = GetComponent<TimelineManager>();
			node = new TimelineNode(3f, NodeEventOn3F);
			model = new TimelineModel("Test", 5f, StartHandle, Expire, Stop, 
                Tick,
				new[] { node });
		}

		void Start()
		{
			timelineManager.AddToAddList(model, this.gameObject);
		}

		private void FixedUpdate()
		{
			timelineManager.UpdateTimelines(Time.fixedDeltaTime);

			if (stopInput)
			{
				foreach (var id in timelineManager.timelines.Keys)
				{
					timelineManager.StopTimeline(id);
				}
			}

			if (addInput)
			{
				timelineManager.AddToAddList(model, this.gameObject);
			}
			
			stopInput = false;
			addInput = false;
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.S))
			{
				stopInput = true;
			}

			if (Input.GetKeyDown(KeyCode.A))
			{
				addInput = true;
			}
		}
	}
}


