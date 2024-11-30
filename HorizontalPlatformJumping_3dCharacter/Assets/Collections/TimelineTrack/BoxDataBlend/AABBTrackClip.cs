using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class AABBTrackClip: PlayableAsset, ITimelineClipAsset
{
	[SerializeField]
	private AABBTrackBehaviour behaviour = new AABBTrackBehaviour();
	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
	{
		return ScriptPlayable<AABBTrackBehaviour>.Create(graph, behaviour);
	}

	public ClipCaps clipCaps => ClipCaps.None;
}
