using System;
using UnityEngine;

public class EffectController : MonoBehaviour
{
	[Header("Shadow Blade")]
	[SerializeField] private GhostSwordPool ghostSwordPool;
	[SerializeField] private Transform sword;
	[SerializeField] private float interval;
	[SerializeField] private float fadeTime;
	private float shadowTimer;

	[Space, Header("Bloom Blade")]
	[SerializeField] private StencilPostEffect bloomBladeEffect;
	[SerializeField] private float displayTime = 1f;
	private float displaySpeed;
	[SerializeField] private float disappearanceTime = 0.5f;
	private float disappearanceSpeed;
	private float bloomAlpha;
	private bool isBloom;

	private bool bloomEnable;
	
	private void Start()
	{
		shadowTimer = interval;
		bloomAlpha = 1f;
		displaySpeed = 1f / displayTime;
		disappearanceSpeed = 1f / disappearanceTime;

		if (bloomBladeEffect != null)
			bloomEnable = true;
		else
		{
			bloomBladeEffect = Camera.main.GetComponent<StencilPostEffect>();
			if (bloomBladeEffect != null)
				bloomEnable = true;
			else
				bloomEnable = false;
		}

		if (ghostSwordPool == null)
		{
			ghostSwordPool = GameObject.FindWithTag("GameController")
				.GetComponent<GhostSwordPool>();
		}
	}

	private void Update()
	{
		HandleShadowEffect();

		if (bloomEnable)
		{
			UpdateBloomAlpha();
			ApplyBloomEffect();
		}
	}

	private void HandleShadowEffect()
	{
		if (!isBloom)
		{
			if (shadowTimer <= 0)
			{
				GhostSword obj = ghostSwordPool.GetObject();
				obj.Initialize(sword.position, sword.rotation, fadeTime);
				shadowTimer += interval;
			}
			else
			{
				shadowTimer -= Time.deltaTime;
			}
		}
	}

	private void UpdateBloomAlpha()
	{
		if (!isBloom)
		{
			bloomAlpha -= disappearanceSpeed * Time.deltaTime;
		}
		else
		{
			bloomAlpha += displaySpeed * Time.deltaTime;
		}
		bloomAlpha = Mathf.Clamp(bloomAlpha, 0.2f, 1f);
	}

	private void ApplyBloomEffect()
	{
		bloomBladeEffect.alpha = bloomAlpha;
	}

	public void SetEffectState(ref Vector2 velocity)
	{
		isBloom = velocity.sqrMagnitude < 1f;
	}
}