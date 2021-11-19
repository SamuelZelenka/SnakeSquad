using UnityEngine;

[CreateAssetMenu(menuName = "PrefabSet", fileName = "Prefab Set")]
public class PrefabContainer : ScriptableObject
{
    [SerializeField] private GameObject[] _prefabs;
    public T GetPrefab<T>() where T : MonoBehaviour
    {
        for (int i = 0; i < _prefabs.Length; i++)
        {
            if (_prefabs[i].TryGetComponent(out T prefab))
            {
                return prefab;
            }
        }
        
        Debug.LogError($"Prefab of type {typeof(T)} does not exist in SpawnManager prefabs.");
        return null;
    }
}
