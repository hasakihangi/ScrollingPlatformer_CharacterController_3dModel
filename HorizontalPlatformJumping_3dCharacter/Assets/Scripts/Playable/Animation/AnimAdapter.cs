using UnityEngine;
using UnityEngine.Playables;

namespace AnimationBehaviour
{
	public class AnimAdapter : PlayableBehaviour
	{
		private AnimBehaviour m_behaviour;

		public void Init(AnimBehaviour behaviour)
		{
			m_behaviour = behaviour;
		}

		public void Enable()
		{
			m_behaviour?.Enable();
		}

		public void Disable()
		{
			m_behaviour?.Disable();
		}

		public override void PrepareFrame(Playable playable, FrameData info)
		{
			base.PrepareFrame(playable, info);
			m_behaviour?.Execute(playable, info);
		}

		public float GetEnterTime()
		{
			return m_behaviour.enterTime;
		}
	}
}

