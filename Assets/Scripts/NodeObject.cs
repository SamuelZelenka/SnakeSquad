using UnityEngine;

public abstract class NodeObject : MonoBehaviour, ISpawnable
{
    public abstract NodeObject OnCollision<T>(T other) where T : NodeObject;
    public abstract GameObject GetGameObject();
}
