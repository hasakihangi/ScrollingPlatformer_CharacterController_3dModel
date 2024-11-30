using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void TimelineEvent(TimelineObj timeline);

// 因为timelineObj里面已经有了caster, 所以这里不需要
public delegate void TimelineTick(GameObject caster, TimelineObj timeline,
	float deltaTime);

public class TimelineNode
{
	public float triggerTime;
	public TimelineEvent timelineEvent;

	public TimelineNode(float triggerTime, TimelineEvent timelineEvent)
	{
		this.triggerTime = triggerTime;
		this.timelineEvent = timelineEvent;
	}
}

public class TimelineModel
{
	public string id;
	public float duration;
	public List<TimelineNode> nodes;
	public TimelineEvent startEvent;
	public TimelineEvent expireEvent;
	public TimelineEvent stopEvent;
	public TimelineTick tick;

	public TimelineModel(string id, float duration, 
		TimelineEvent startEvent = null,
		TimelineEvent expireEvent = null,
		TimelineEvent stopEvent = null,
		TimelineTick tick = null,
		params TimelineNode[] nodes
		)
	{
		this.id = id;
		this.duration = duration;
		this.startEvent = startEvent;
		this.expireEvent = expireEvent;
		this.stopEvent = stopEvent;
		this.tick = tick;
		this.nodes = new List<TimelineNode>(nodes);
		this.nodes.Sort((a, b) 
			=>  a.triggerTime.CompareTo(b.triggerTime)
			);
	}
}

// 在TimelineManager中创建
public class TimelineObj
{
	public TimelineModel model;
	public GameObject caster;
	public TimelineManager manager; // 如果不打算使用单例, 就需要该字段
	public uint id;
	public float currentTime;
	public float lastTime;
	
	public TimelineObj(TimelineModel model, TimelineManager manager, uint 
            id, 
		GameObject caster = null,
        float currentTime = 0f)
	{
		this.model = model;
		this.caster = caster;
		this.manager = manager;
		this.id = id;
		this.currentTime = currentTime;
		this.lastTime = this.currentTime;
	}
	
	public void Update(float deltaTime)
	{
		lastTime = currentTime;
		currentTime += deltaTime;
		
		if (currentTime > model.duration)
		{
			// 当Timeline执行完毕时
			model.expireEvent?.Invoke(this);
			manager.AddToRemoveList(id);
			return;
		}
		
		if (lastTime == 0f)
		{
			// 当Timeline开始执行时
			model.startEvent?.Invoke(this);
		}
		
		model.tick?.Invoke(caster, this, deltaTime);
		
		for (int i = 0; i < this.model.nodes.Count; i++)
		{
			TimelineNode node = this.model.nodes[i];
			if (lastTime < node.triggerTime &&
			    node.triggerTime <= currentTime)
			{
				node.timelineEvent?.Invoke(this);
			}
		}
	}
}
