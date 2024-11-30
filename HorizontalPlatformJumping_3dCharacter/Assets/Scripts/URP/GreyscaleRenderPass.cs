using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace URP
{
	public class GreyscaleRenderPass: ScriptableRenderPass
	{
		private Material material;
		private GreyscaleSettings settings;
		// private 
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			
		}
	}
}