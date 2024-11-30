using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProperties", menuName = "ScriptableObject/PlayerPropertiesPreset")]
public class PlayerPropertiesPreset : ScriptableObject
{
	[Header("Gound")] public float acceleration;
	public float walkSpeed;
	public float deceleration;

	[Header("Air")]public float jumpHeight = 4;
	public float jumpSpeed;
}