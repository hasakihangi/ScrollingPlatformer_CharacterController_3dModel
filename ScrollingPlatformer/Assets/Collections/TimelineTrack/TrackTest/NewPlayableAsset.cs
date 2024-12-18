using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace TimelineTrack
{
	[System.Serializable]
	public class NewPlayableAsset : PlayableAsset
	{
		private NewPlayableBehaviour newPlayableBehaviour;
		public string str;
		public float number;
		public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
		{
			var scriptPlayable = ScriptPlayable<NewPlayableBehaviour>.Create
				(graph);
			newPlayableBehaviour = scriptPlayable.GetBehaviour();
			newPlayableBehaviour.str = str;
			newPlayableBehaviour.number = number;
			return scriptPlayable;
		}
	}
}

