using System;
using UnityEngine;

namespace AnimatorBrain
{
	public class AnimatorBrain
	{
		private readonly static int[] animations =
		{
			// Animator.StringToHash("Change this to state name
		};

		private Animator animator;
		// 不同层当前的动画
		private E_Animation[] currentAnimation;
		// 标明锁定某一层
		private bool[] layerLocked;
		private Action<int> DefaultAnimation;

		protected void Initialize(int layers, E_Animation startingAnimation, Animator animator,
			Action<int> DefaultAnimation)
		{
			layerLocked = new bool[layers];
			currentAnimation = new E_Animation[layers];
			this.animator = animator;
			this.DefaultAnimation = DefaultAnimation;
			for (int i = 0; i < layers; i++)
			{
				layerLocked[i] = false;
				currentAnimation[i] = startingAnimation;
			}
		}

		public E_Animation GetCurrentAnimation(int layer)
		{
			return currentAnimation[layer];
		}

		public void SetLocked(bool isLock, int layer)
		{
			layerLocked[layer] = isLock;
		}

		public void Play(E_Animation animation, int layer, bool lockLayer, bool bypassLock, float crossFade = 0.2f)
		{
			if (animation == E_Animation.None)
			{
				DefaultAnimation(layer);
				return;
			}

			if (layerLocked[layer] && !bypassLock) return;

			layerLocked[layer] = lockLayer;
			
			if (bypassLock)
				foreach (var item in animator.GetBehaviours<OnExit>())	
				{
					
				}
		}
	}
}

public enum E_Animation
{
	None,
}