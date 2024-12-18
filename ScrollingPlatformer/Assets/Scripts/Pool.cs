using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IPool
{
    void ReturnToPool(PoolObject poolObject);
}

public class Pool<T> : MonoBehaviour, IPool where T: PoolObject
{
    [SerializeField] private int initSize = 4;
    [SerializeField] private T objectToPool;

    private Stack<T> stack;
    private Scene poolScene;

    private void Start()
    {
        int capacity = initSize < 4 ? 4 : initSize;
        stack = new Stack<T>(capacity);
        
        SetupPoolScene();
        SetObjectsNumber(initSize);
    }

    private void SetupPoolScene()
    {
        Scene scene =
            SceneManager.GetSceneByName(objectToPool.gameObject.name + " Pool");
        if (scene.IsValid())
        {
            GameObject[] objects = scene.GetRootGameObjects();
            for (int i = 0; i < objects.Length; i++)
            {
                GameObject obj = objects[i];
                obj.SetActive(false);
                T pooledObject = obj
                    .GetComponent<T>();
                if (pooledObject != null)
                {
                    stack.Push(pooledObject);
                }
            }
        }
        else
        {
            scene = SceneManager.CreateScene(objectToPool.gameObject.name +
                                             " Pool");
        }
        
        poolScene = scene;
    }

    public void SetObjectsNumber(int number)
    {
        // 将池中物体数量设置到指定值
        int difference = number - stack.Count;
        T instance = null;
        
        if (difference >= 0)
        {
            for (int i = 0; i < difference; i++)
            {
                instance = Instantiate(objectToPool);
                instance.Pool = this;
                GameObject obj = instance.gameObject;
                obj.SetActive(false);
                SceneManager.MoveGameObjectToScene(obj, poolScene);
                stack.Push(instance);
            }
        }
        else
        {
            for (int i = 0; i < -difference; i++)
            {
                instance = stack.Pop();
                Destroy(instance.gameObject);
            }
        }
    }

    public T GetObject()
    {
        T instance;
        if (stack.Count == 0)
        {
            instance = Instantiate(objectToPool);
            instance.Pool = this;
            SceneManager.MoveGameObjectToScene(instance.gameObject, poolScene);
        }
        else
        {
            instance = stack.Pop();
            instance.Pool = this;
            instance.gameObject.SetActive(true);
        }
        return instance;
    }
    
    public void ReturnToPool(PoolObject poolObject)
    {
        stack.Push(poolObject as T);
        poolObject.gameObject.SetActive(false);
    }
}
