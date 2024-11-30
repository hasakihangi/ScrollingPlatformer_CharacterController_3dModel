using UnityEngine;
using UnityEngine.Playables;

namespace AnimationBehaviour
{
	public class AnimBehaviour
	{
		public bool enable { get; protected set; }
		public float enterTime { get; protected set; }
		
		protected Playable m_adapterPlayable;
		
		
		public AnimBehaviour() { }

		public AnimBehaviour(PlayableGraph graph, float enterTime = 0f)
		{
			m_adapterPlayable = ScriptPlayable<AnimAdapter>.Create(graph);
			((ScriptPlayable<AnimAdapter>)m_adapterPlayable).GetBehaviour().Init(this);
			this.enterTime = enterTime;
		}
		
		public virtual void Enable()
		{
			enable = true;
		}

		public virtual void Disable()
		{
			enable = false;
		}

		public virtual void Execute(Playable playable, FrameData info)
		{
			if (!enable)
			{
				return;
			}
		}

		public Playable GetAnimAdapterPlayable()
		{
			return m_adapterPlayable;
		}
		
		public virtual void AddInput(Playable playable) { }

		public void AddInput(AnimBehaviour behaviour)
		{
			AddInput(behaviour.GetAnimAdapterPlayable());
		}
	}
}
