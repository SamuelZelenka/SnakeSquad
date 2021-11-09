using UnityEngine;

public class GameObjectPool<T> : ObjectPool<T> where T : IPoolable, new()
{
    private T _prefab;
    private Transform _parent;

    public override T Acquire()
    {
        T acquired = base.Acquire();
        acquired.SetActive(true);
        return acquired;
    }

    public override void Release(T releaseObject)
    {
        if (GetPoolSize() > capacity)
        {
            Object.Destroy(releaseObject.GameObject);
            return;
        }
        pool.Enqueue(releaseObject);
        releaseObject.SetActive(false);
    }
    public GameObjectPool(T prefab, Transform parent)
    {
        this._parent = parent;
        this._prefab = prefab;
        onCreate = () =>
        {
            IPoolable newObject = Object.Instantiate(prefab.GameObject, parent).GetComponent<IPoolable>();
            return (T)newObject;
        };
    }
}
