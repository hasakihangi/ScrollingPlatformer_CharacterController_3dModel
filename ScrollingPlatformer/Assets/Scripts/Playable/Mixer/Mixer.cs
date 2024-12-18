using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationBehaviour
{
	public class Mixer: AnimBehaviour
	{
		public int inputCount { get; private set; }

		public int currentIndex => m_currentIndex;
		public bool isTransition => m_isTransition;
		
		private AnimationMixerPlayable m_animMixer;

		private int m_currentIndex;
		private int m_targetIndex;
		private bool m_isTransition;

		private List<int> m_declineIndex;
		private float m_timeToNext;

		private float m_currentSpeed;
		private float m_declineSpeed;

		private float m_declineWeight;

		public Mixer(PlayableGraph graph) : base(graph)
		{
			m_animMixer = AnimationMixerPlayable.Create(graph);
			m_adapterPlayable.AddInput(m_animMixer, 0, 1f);
			m_currentIndex = 0;
			m_targetIndex = -1;

			m_declineIndex = new List<int>();
		}

		public override void AddInput(Playable playable)
		{
			m_animMixer.AddInput(playable, 0, inputCount == 0? 1f:0f);
			inputCount++;
		}

		public override void Enable()
		{
			base.Enable();
			
			m_adapterPlayable.SetTime(0f);
			m_animMixer.SetTime(0f);
			m_adapterPlayable.Play();
			m_animMixer.Play();

			m_currentIndex = 0;
			m_targetIndex = -1;
			m_animMixer.SetInputWeight(0, 1f);
			AnimHelper.Enable(m_animMixer, 0);
		}

		public override void Disable()
		{
			base.Disable();
			m_adapterPlayable.Pause();
			m_animMixer.Pause();
			for (int i = 0; i < inputCount; i++)
			{
				AnimHelper.Disable(m_animMixer, i);
			}
		}

		public override void Execute(Playable playable, FrameData info)
		{
			base.Execute(playable, info);
			if (!m_isTransition || m_targetIndex < 0) return;

			if (m_timeToNext > 0f)
			{
				m_timeToNext -= info.deltaTime;

				m_declineWeight = 0f;
				for (int i = 0; i < m_declineIndex.Count; i++)
				{
					var w = ModifyWeight(m_declineIndex[i], -info.deltaTime);
					if (w <= 0)
					{
						AnimHelper.Disable(m_animMixer, m_declineIndex[i]);
						m_declineIndex.RemoveAt(i--);
					}
					else
					{
						m_declineWeight += w;
					}
				}
				
				SetWeight(m_targetIndex, 1-m_declineWeight-ModifyWeight(m_currentIndex, -info
                    .deltaTime*m_currentSpeed));
				return;
			}
			AnimHelper.Disable(m_animMixer, m_currentIndex);
			m_currentIndex = m_targetIndex;
			m_targetIndex = -1;
			m_isTransition = false;
		}

		public void SetWeight(int index, float weight)
		{
			if (index >= 0 && index < inputCount)
			{
				m_animMixer.SetInputWeight(index, weight);
			}
		}
		public void TransitionTo(int target)
		{
			if (m_isTransition && m_targetIndex >= 0)
			{
				if (target == m_targetIndex) return;
				
				if (m_currentIndex == target)
				{
					m_currentIndex = m_targetIndex;
				}
				else if (GetWeight(m_currentIndex) > GetWeight(m_targetIndex))
				{
					m_declineIndex.Add(m_targetIndex);
				}
				else
				{
					m_declineIndex.Add(m_currentIndex);
					m_currentIndex = m_targetIndex;
				}
			}

			m_targetIndex = target;
			m_declineIndex.Remove(m_targetIndex);
			AnimHelper.Enable(m_animMixer, m_targetIndex);
			m_timeToNext = GetTargetEnter(m_targetIndex);
			m_currentSpeed = GetWeight(m_currentIndex) / m_timeToNext;
			m_declineSpeed = 2f / m_timeToNext;
			m_isTransition = true;
		}

		public float GetWeight(int index)
		{
			return index>=0 && index<inputCount ? m_animMixer.GetInputWeight(index): 0f;
		}

		private float GetTargetEnter(int index)
		{
			return ((ScriptPlayable<AnimAdapter>)m_animMixer.GetInput(index)).GetBehaviour().GetEnterTime();
		}

		private float ModifyWeight(int index, float delta)
		{
			var weight = GetWeight(index) + delta;
			m_animMixer.SetInputWeight(index, weight);
			return weight;
		}
	}
}
