using UnityEngine;

public abstract class NodeObject : MonoBehaviour, ISpawnable
{
    public abstract NodeObject OnCollision(Squad collidingSquad);
    public abstract GameObject GetGameObject();
}
