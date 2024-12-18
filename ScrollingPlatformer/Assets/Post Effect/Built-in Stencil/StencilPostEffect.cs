using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;


// 步骤:
// 1 maskTexture
// 2 bloomTexture
// 3 bloomTexture + cameraTexture

[ExecuteAlways]
public class StencilPostEffect : MonoBehaviour
{
	#region Mask
	[Header("Mask")]
	[SerializeField] private Shader maskShader;
	public Color maskColor;
	[SerializeField] private Color toColor;
	public float alpha = 1f;
	private Material _maskMat;

	private Material MaskMat
	{
		get
		{
			if (_maskMat == null && maskShader != null)
			{
				_maskMat = new Material(maskShader);
				_maskMat.hideFlags = HideFlags.HideAndDontSave;
			}

			return _maskMat;
		}
	}

	private int colorId;
	private int alphaId;

	#endregion



	#region Bloom
	[Space]
	[Header("Bloom")]
	[SerializeField]
	private Shader bloomShader;

	[SerializeField, Range(0, 4)] private int iterations = 3;
	[SerializeField, Range(0.2f, 4.0f)] private float blurSpread = 0.6f;
	[SerializeField] private int downSample = 2;

	private Material _bloomMat;

	private Material BloomMat
	{
		get
		{
			if (_bloomMat == null && bloomShader != null)
			{
				_bloomMat = new Material(bloomShader);
				_bloomMat.hideFlags = HideFlags.HideAndDontSave;
			}

			return _bloomMat;
		}
	}

	private int blurSizeId;

	#endregion
	
	#region Flow
	[Space]
	[Header("Flow")]
	[SerializeField]
	private Shader flowShader;

	private ShaderToMaterial flowMaterial;

	#endregion

	

	#region Combine
	[Space]
	[Header("Combine")]
	[SerializeField]
	private Shader combineColorShader;

	private Material _combineMat;

	private Material CombineMat
	{
		get
		{
			if (_combineMat == null && combineColorShader != null)
			{
				_combineMat = new Material(combineColorShader);
				_combineMat.hideFlags = HideFlags.HideAndDontSave;
			}

			return _combineMat;
		}
	}

	private int bloomTexId;

	#endregion

	private RenderTexture cameraRT;
	private RenderTexture stencilColorRT;

	[Space]
	[Space]
	[SerializeField] private new Camera camera;


	// 为什么在游戏中禁用再开启,RenderTexture的尺寸会不一样呢?
	// 刚进入游戏时,会采用设置的一个默认尺寸; 在游戏中,会使用窗口的尺寸; 所以要预设一个合适的尺寸, 或者进入游戏后尺寸不再变化
	private void OnEnable()
	{
		// const int width = 1920;
		// const int height = 1080;

		int width = Screen.width;
		int height = Screen.height;

		cameraRT = new RenderTexture(width, height, 24);
		stencilColorRT =
			new RenderTexture(width, height, 0);
		if (camera == null)
		{
			camera = Camera.main;
		}

		colorId = Shader.PropertyToID("_Color");
		blurSizeId = Shader.PropertyToID("_BlurSize");
		bloomTexId = Shader.PropertyToID("_BloomTex");
		flowMaterial = new ShaderToMaterial(flowShader);
		alphaId = Shader.PropertyToID("_Alpha");
	}

	private void Start()
	{
	}

	private Color col;
	private const float colFlowSpeed = 0.5f;
	void Update()
	{
		float t = Mathf.PingPong(Time.time * colFlowSpeed, 1);
		col = Color.Lerp(maskColor, toColor, t);
	}

	private void OnPreRender()
	{
		camera.targetTexture = cameraRT;
	}

	private void OnPostRender()
	{
		#region Mask

		camera.targetTexture = null;

		RenderTexture.active = stencilColorRT;
		// Graphics.SetRenderTarget(stencilColorRT); // 是同样的方法
		GL.Clear(true, true, Color.clear); // 清除stencilColorRT中上一帧残留的内容

		Graphics.SetRenderTarget(stencilColorRT.colorBuffer,
			cameraRT
				.depthBuffer); // 表示使用cameraRT的depthBuffer, 但是输出到stencilColorRT的colorBuffer
		MaskMat.SetColor(colorId, col);
		MaskMat.SetFloat(alphaId, alpha);
		Graphics.Blit(cameraRT,
			MaskMat); // 将cameraRT作为_MainTex传入到Mat中,输出内容到RenderTarget

		#endregion

		#region Bloom

		RenderTexture buffer0 = RenderTexture.GetTemporary(stencilColorRT
			.width, stencilColorRT.height, 0);
		Bloom(stencilColorRT, buffer0, BloomMat, iterations, blurSpread,
			downSample);

		#endregion

		#region Flow

		RenderTexture buffer1 = RenderTexture.GetTemporary(buffer0.width,
			buffer0.height, 0);
		if (flowMaterial != null)
			Graphics.Blit(buffer0, buffer1,
				flowMaterial.Material);
		else
			Graphics.Blit(buffer0, buffer1);

		#endregion

		#region Combine

		if (CombineMat != null)
		{
			CombineMat.SetTexture(bloomTexId, buffer1);
			Graphics.Blit(cameraRT, null as RenderTexture, CombineMat);
		}
		else
		{
			Graphics.Blit(cameraRT, null as RenderTexture);
		}
		#endregion

		RenderTexture.ReleaseTemporary(buffer0);
		RenderTexture.ReleaseTemporary(buffer1);
	}

	private void OnDisable()
	{
		cameraRT.Release();
		stencilColorRT.Release();
	}

	void Bloom(RenderTexture inTexture, RenderTexture outTexture, Material
		material, int
		iterations, float blurSpread, int downSample)
	{
		if (material != null)
		{
			int rtW = inTexture.width / downSample;
			int rtH = inTexture.height / downSample;

			RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
			buffer0.filterMode = FilterMode.Bilinear;

			Graphics.Blit(inTexture, buffer0);

			for (int i = 0; i < iterations; i++)
			{
				material.SetFloat(blurSizeId, 1.0f + i * blurSpread);

				RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

				// Render the vertical pass
				Graphics.Blit(buffer0, buffer1, material, 0);

				RenderTexture.ReleaseTemporary(buffer0);
				buffer0 = buffer1;
				buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

				// Render the horizontal pass
				Graphics.Blit(buffer0, buffer1, material, 1);

				RenderTexture.ReleaseTemporary(buffer0);
				buffer0 = buffer1;
			}

			Graphics.Blit(buffer0, outTexture);
			RenderTexture.ReleaseTemporary(buffer0);
		}
		else
		{
			Graphics.Blit(inTexture, outTexture);
		}
	}
}