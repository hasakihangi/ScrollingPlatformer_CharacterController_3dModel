using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public struct AABBs
{
	public AABB ab0;
	public AABB ab1;
	public AABB ab2;
	public AABB ab3;
	public AABB ab4;
}

// 仅用于产生AABB
[System.Serializable]
public class AABBTrackBehaviour : PlayableBehaviour
{
	[SerializeField] private int totalFrames;
	[SerializeField] private int intervalFrames = 5;
	// 计算得到, 作为数组长度的参考值
	[SerializeField] private int aabbsNumber;
	[SerializeField] private AABBs[] aabbsGroup;

	private ActionController actionController;
	private int currentFrame;
	private double frameTime = 0.02;
	
	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		if (actionController == null)
		{
			actionController = playerData as ActionController;
		}

		double currentTime = playable.GetTime();
		currentFrame = Mathf.FloorToInt((float)(currentTime / frameTime));
		int aabbsIndex = currentFrame / intervalFrames;
		AABBs aabbs = aabbsGroup[aabbsIndex];
		AABB[] aabbArray = new AABB[5]
		{
			aabbs.ab0,
			aabbs.ab1,
			aabbs.ab2,
			aabbs.ab3,
			aabbs.ab4,
		};
		actionController.AABBOverlap(aabbArray);
	}
}
