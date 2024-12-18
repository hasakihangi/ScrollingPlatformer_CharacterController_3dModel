using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace URP
{
	[System.Serializable, VolumeComponentMenu("Example/Greyscale")]
	public class GreyscaleSettings: VolumeComponent, IPostProcessComponent
	{
		public ClampedFloatParameter strength =
			new ClampedFloatParameter(0f, 0f, 1f);
		public bool IsActive()
		{
			return strength.value > 0f && active;
		}

		public bool IsTileCompatible()
		{
			return false;
		}
	}
}