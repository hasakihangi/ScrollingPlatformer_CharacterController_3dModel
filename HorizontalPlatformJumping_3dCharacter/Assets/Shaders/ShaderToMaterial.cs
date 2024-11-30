using UnityEngine;

public class ShaderToMaterial
{
	private Shader shader;
	private Material _material;

	public Material Material
	{
		get
		{
			if (_material == null && shader != null)
			{
				_material = new Material(shader);
				_material.hideFlags = HideFlags.HideAndDontSave;
			}

			return _material;
		}
		private set => _material = value;
	}

	public ShaderToMaterial(Shader shader)
	{
		this.shader = shader;
	}
}
