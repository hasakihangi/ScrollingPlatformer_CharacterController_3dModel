using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace URP
{
	public class GreyscaleFeature: ScriptableRendererFeature
	{
		private GreyscaleRenderPass pass;
		public override void Create()
		{
			name = "Greyscale";
			pass = new GreyscaleRenderPass();
		}

		public override void AddRenderPasses(ScriptableRenderer renderer,
			ref RenderingData renderingData)
		{
			// pass.;
		}
	}
}

