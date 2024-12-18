using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.SocialPlatforms;

namespace AnimationBehaviour
{
	public class RandomSelector: AnimBehaviour
	{
		public int currentIndex { get; private set; }
		public int clipCount { get; private set; }
		
		private AnimationMixerPlayable m_mixer;
		
		public RandomSelector(PlayableGraph graph) : base(graph)
		{
			m_mixer = AnimationMixerPlayable.Create(graph);
			m_adapterPlayable.AddInput(m_mixer, 0, 1f);
			currentIndex = -1;
		}

		public override void AddInput(Playable playable)
		{
			m_mixer.AddInput(playable, 0, 0f);
			clipCount++;
		}

		public override void Enable()
		{
			base.Enable();
			if (currentIndex < 0 || currentIndex >= clipCount)
				return;
			AnimHelper.Enable(m_mixer.GetInput(currentIndex));
			m_adapterPlayable.SetTime(0f);
			m_adapterPlayable.Play();
			m_mixer.SetTime(0f);
			m_mixer.Play();
		}

		public override void Disable()
		{
			base.Disable();
			if (currentIndex < 0 || currentIndex >= clipCount)
				return;
			m_adapterPlayable.Pause();
			m_mixer.Pause();
			currentIndex = -1;
		}

		public int Select()
		{
			currentIndex = Random.Range(0, clipCount);
			return currentIndex;
		}
	}
}
