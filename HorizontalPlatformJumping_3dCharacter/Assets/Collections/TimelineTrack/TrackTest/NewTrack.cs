using UnityEngine;
using UnityEngine.Timeline;

namespace TimelineTrack
{
	[TrackColor(0.1f,0.8f,0.4f), TrackClipType(typeof(NewPlayableAsset)), 
	TrackBindingType(typeof(GameObject))]
	public class NewTrack: TrackAsset
	{
		
	}
}
	
