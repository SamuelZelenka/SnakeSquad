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
    public static GameObject SpawnAt<T>(Vector2Int coordinate) where T : MonoBehaviour
    {
        return Instantiate(GetPrefab<T>(), HexGrid.GetWorldPos(coordinate), quaternion.identity);
    }
}
