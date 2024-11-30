using UnityEngine;

// Player的人物循环中移动逻辑从这里面获取到, 经由ChaControlState过滤
public class CharacterState: MonoBehaviour
{
	public string[] tags;

	public ChaControlState controlState;
	public ChaProperty property;
	public ChaResource resource;
	
	// Player主循环:
	// 获取输入
	// 评估后, 获得输入的移动值
	// 收集各个移动值之后, 在传给physicsController进行移动
	// 人物的移动速度应该是在Player脚本中进行配置, 也确实是这样的
	
	// 架构:
	// Player主循环从各个组件那里获取方法
	
	// 评估输入
	// 输入都应该接收, 评估的是移动值
	// public 
	
	// 评估动画
}

[System.Serializable]
public struct ChaControlState
{
	public bool canMove;
	public bool canRotate;
	public bool canAttack;

	public ChaControlState(bool canMove = true, bool canRotate = true, bool
		canAttack = true)
	{
		this.canMove = canMove;
		this.canRotate = canRotate;
		this.canAttack = canAttack;
	}

	public void Origin()
	{
		this.canMove = true;
		this.canRotate = true;
		this.canAttack = true;
	}

	public static ChaControlState origin = new ChaControlState();

	public static ChaControlState stun = new ChaControlState(false, false,
		false);

	public static ChaControlState operator &(ChaControlState a,
		ChaControlState b)
	{
		return new ChaControlState(
			a.canMove && b.canMove,
			a.canRotate && b.canRotate,
			a.canAttack && b.canAttack
		);
	}
}

// 当前人物的数值, 例如血量的最大值
// 装备, buff也会有该数据
[System.Serializable]
public struct ChaProperty
{
	public int hp;
	public int attack;

	public ChaProperty(int hp, int attack)
	{
		this.hp = hp;
		this.attack = attack;
	}

	public static ChaProperty zero = new ChaProperty(0, 0);

	public static ChaProperty operator +(ChaProperty a, ChaProperty b)
	{
		return new ChaProperty(a.hp + b.hp, b.attack + a.attack);
	}
}

// 当前人物的资源, 例如当前的血量, 感觉还是应该是struct
[System.Serializable]
public struct ChaResource
{
	public int hp;

	public ChaResource(int hp)
	{
		this.hp = hp;
	}

	public void ClampResource(ChaProperty property)
	{
		hp = Mathf.Clamp(hp, 0, property.hp);
	}

	public static ChaResource operator +(ChaResource a, ChaResource b)
	{
		return new ChaResource(a.hp + b.hp);
	}
}