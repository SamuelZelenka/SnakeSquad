using UnityEngine;

public class GameObjectPool<T> : ObjectPool<T> where T : IPoolable, new()
{    
    public override T Acquire()
    {
        T acquired = base.Acquire();
        acquired.SetActive(true);
        return acquired;
    }

    public override void Release(T releaseObject)
    {
        releaseObject.SetActive(false);
        if (GetPoolSize() > capacity)
        {
            Object.Destroy(releaseObject.gameObject);
            return;
        }
        pool.Enqueue(releaseObject);
    }

    public GameObjectPool(T prefab, Transform parent)
    {
        onCreate = () =>
        {
            IPoolable newObject = Object.Instantiate(prefab.gameObject, parent).GetComponent<IPoolable>();
            return (T)newObject;
        };
    }
}
