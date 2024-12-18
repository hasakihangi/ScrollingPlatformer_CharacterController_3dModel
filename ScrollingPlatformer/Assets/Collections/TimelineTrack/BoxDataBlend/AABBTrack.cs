using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public struct AABB
{
	public Vector2 min;
	public Vector2 max;

	public AABB(Vector2 min, Vector2 max)
	{
		this.min = min;
		this.max = max;
	}
}

[TrackColor(0.8f,0.1f,0.3f)]
[TrackBindingType(typeof(ActionController))]
[TrackClipType(typeof(AABBTrackClip))]
public class AABBTrack: TrackAsset
{
	public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
	{
		return ScriptPlayable<AABBTrackBehaviour>.Create(graph, inputCount);
	}
}
