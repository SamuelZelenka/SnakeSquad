using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PrefabSet", fileName = "Prefab Set")]
public class PrefabContainer : ScriptableObject
{
    [SerializeField] private GameObject[] prefabs;
    public GameObject GetPrefab<T>() where T : MonoBehaviour
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            if (prefabs[i].TryGetComponent<T>(out _))
            {
                return prefabs[i];
            }
        }
        
        Debug.LogError($"Prefab of type {typeof(T)} does not exist in SpawnManager prefabs.");
        return null;
    }
}
