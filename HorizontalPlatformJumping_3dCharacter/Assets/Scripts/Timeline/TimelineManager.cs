using System;
using System.Collections.Generic;
using UnityEngine;

public class TimelineManager: MonoBehaviour
{
	// 因为有查找需求, 所以用字典要好一点? 但是字典内部是通过数组实现的, 并不是很适合频繁插入和删除
	// 字典不允许键值重复
	// 游戏开发中, 为了高效查找一般还是使用的数组
	// 字典应该还是要好一点, 只需要设定一个合适的初始容量
	// public LinkedList<TimelineObj> timelines;
	public Dictionary<uint, TimelineObj> timelines;

	[SerializeField] private int initSize = 4;

	private List<uint> removeList;
	private List<TimelineObj> addList;
	
	// 0-99存储永久性timeline, 100- 存储暂时性timeline
	private uint _currentId = 100;
	

	public uint GetAndUpdateId()
	{
		uint id = _currentId;
		_currentId++;
		if (_currentId == UInt32.MaxValue)
			_currentId = 100;
		return id;
	}
	
	private void Awake()
	{
		int capacity = initSize < 4 ? 4 : initSize;
		timelines = new Dictionary<uint, TimelineObj>(capacity);
		removeList = new List<uint>();
		addList = new List<TimelineObj>();
	}

	public void UpdateTimelines(float deltaTime)
	{
		foreach (var timeline in addList)
		{
			AddTimeline(timeline);
		}
		
		addList.Clear();
		
		foreach (var timeline in timelines.Values)
		{
			timeline.Update(deltaTime);
		}

		foreach (var id in removeList)
		{
			RemoveTimeline(id);
		}
		
		removeList.Clear();
	}

	public TimelineObj AddToAddList(TimelineModel model, GameObject go = null)
	{
		TimelineObj timeline = new TimelineObj(model, this, GetAndUpdateId(), go);
		addList.Add(timeline);
		return timeline;
	}
	
	private void AddTimeline(TimelineObj obj)
	{
		if (!timelines.ContainsKey(obj.id))
		{
			timelines.Add(obj.id, obj);
		}
		else
		{
			Debug.Log("AddTimeline Fail! Timelines already have the same key!");
		}
	}

	private void RemoveTimeline(uint id)
	{
		if (timelines.ContainsKey(id))
			timelines.Remove(id);
		else
		{
			Debug.Log("RemoveTimeline Fail! Timelines dont have the key!");
		}
	}

	public void StopTimeline(uint id)
	{
		if (timelines.ContainsKey(id))
		{
			TimelineObj tl = timelines[id];
			tl.model.stopEvent?.Invoke(tl);
			AddToRemoveList(id);
		}
		else
		{
			Debug.Log("StopTimeline Fail! Timeline dont have the key!");
		}
	}

	public void AddToRemoveList(uint id)
	{
		removeList.Add(id);
	}
}
