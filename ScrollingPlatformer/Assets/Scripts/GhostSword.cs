using System;
using System.Collections;
using UnityEngine;

public class GhostSword: PoolObject
{
	[SerializeField] Material mat;
	
	float fadeTime;
	private int alphaId;
	private float originAlpha;
	private MaterialPropertyBlock block;
	private MeshRenderer renderer;
	
	private void Awake()
	{
		if (mat == null)
			mat = GetComponent<Material>();
		alphaId = Shader.PropertyToID("_Alpha");
		originAlpha = mat.GetFloat(alphaId);
		block = new MaterialPropertyBlock();
		renderer = GetComponent<MeshRenderer>();
	}
	

	public void Initialize(Vector3 position, Quaternion rotation, float 
            fadeTime = 0.2f)
	{
		transform.position = position;
		transform.rotation = rotation;
		this.fadeTime = fadeTime;
		mat.SetFloat(alphaId, originAlpha);
		StartCoroutine(FadeAway());
	}

	IEnumerator FadeAway()
	{
		float elapsedTime = 0f;
		float alpha;
		while (elapsedTime < fadeTime)
		{
			elapsedTime += Time.deltaTime;
			alpha = Mathf.Lerp(originAlpha, 0f, elapsedTime / fadeTime);
			alpha = alpha < 0 ? 0 : alpha;
			
			block.SetFloat(alphaId, alpha);
			renderer.SetPropertyBlock(block);
			
			yield return null;
		}
		Release();
	}
}
