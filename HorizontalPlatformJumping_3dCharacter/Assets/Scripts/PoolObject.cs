using UnityEngine;

public class PoolObject: MonoBehaviour 
{
	private IPool _pool;

	public IPool Pool
	{
		get => _pool;
		set => _pool = value;
	}

	public void Release()
	{
		_pool.ReturnToPool(this);
	}
}
