using UnityEngine;

[CreateAssetMenu(menuName = "PrefabSet", fileName = "Prefab Set")]
public class PrefabContainer : ScriptableObject
{
    [SerializeField] private GameObject[] _prefabs;
    public GameObject GetPrefab<T>() where T : MonoBehaviour
    {
        for (int i = 0; i < _prefabs.Length; i++)
        {
            if (_prefabs[i].TryGetComponent<T>(out _))
            {
                return _prefabs[i];
            }
        }
        
        Debug.LogError($"Prefab of type {typeof(T)} does not exist in SpawnManager prefabs.");
        return null;
    }
}
