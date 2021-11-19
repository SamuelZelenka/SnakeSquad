using System;
using System.Collections.Generic;

public class ObjectPool<T> where T : new()
{
    public int capacity = 50;

    protected delegate T CreationHandler();
    protected CreationHandler onCreate;

    protected Queue<T> pool = new Queue<T>();

    public int GetPoolSize() => pool.Count;

    public ObjectPool()
    {
        onCreate = () => new T();
    }

    public ObjectPool(Func<T> createFunction) => onCreate = createFunction.Invoke;

    public virtual T Acquire()
    {
        T poolObject;

        if (pool.Count > 0)
        {
            poolObject = pool.Dequeue();
        }
        else
        {
            poolObject = onCreate.Invoke();
        }
        return poolObject;
    }

    public virtual void Release(T returnObject)
    {
        if (GetPoolSize() > capacity)
        {
            returnObject = default;
            return;
        }
        pool.Enqueue(returnObject);
    }
}
