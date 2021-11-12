using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    public List<GameObject> prefabs = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private static GameObject GetPrefab<T>() where T : MonoBehaviour
    {
        foreach (GameObject prefab in instance.prefabs)
        {
            if (prefab.TryGetComponent<T>(out _))
            {
                return prefab;
            }
        }
        
        Debug.LogError($"Prefab of type {typeof(T)} does not exist in SpawnManager prefabs.");
        return null;
    }
    public static T SpawnAt<T>(Vector2Int coordinate) where T : MonoBehaviour
    {
        GameObject spawnPrefab = GetPrefab<T>();
        GameObject spawnedObject = Instantiate(spawnPrefab, HexGrid.GetWorldPos(coordinate), quaternion.identity);
        return spawnedObject.GetComponent<T>();
    }
}
