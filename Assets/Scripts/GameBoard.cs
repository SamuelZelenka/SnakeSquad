using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameBoard : MonoBehaviour
{
    
    [SerializeField] private HexNode prefab;
    [SerializeField] private Soldier soldierPrefab;

    private static GameBoard _instance;
    public Dictionary<Vector2Int, HexNode> visibleNodes = new Dictionary<Vector2Int, HexNode>();
    
    private GameObjectPool<HexNode> _nodePool;

    private void Awake()
    {
        _nodePool = new GameObjectPool<HexNode>(prefab, transform);
        List<Vector2Int> hexPositions = new List<Vector2Int>();
        
        for (int x = -4; x < 4; x++)
        {
            for (int y = -4; y < 4; y++)
            {
                hexPositions.Add(new Vector2Int(x,y));
            }
        }

        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DrawHexagons(hexPositions.ToArray());
        Squad.onMoveTick += RevealHexagonsAround;
    }
    public HexNode AcquireNode() => _nodePool.Acquire();

    public static void ReleaseNode(HexNode node)
    {
        _instance._nodePool.Release(node);
        _instance.visibleNodes.Remove(node.coordinate);
    }
    public static void SetNodeObject(Vector2Int coordinate, NodeObject nodeObject)
    {
        _instance.visibleNodes[coordinate].value = nodeObject;
    }

    public static HexNode GetNode(Vector2Int coordinate)
    {
        return _instance.visibleNodes[coordinate];
    }

    private void DrawHexagons(Vector2Int[] nodePositions)
    {
        foreach (Vector2Int nodePosition in nodePositions)
        {
            if (!visibleNodes.ContainsKey(nodePosition))
            {
                DrawHexagon(nodePosition);
                NodeObject generatedObject = GenerateObject();
                if (generatedObject != null)
                {
                    visibleNodes[nodePosition].value = Instantiate(generatedObject, HexGrid.GetWorldPos(nodePosition), quaternion.identity);
                }
            }  
        }
        void DrawHexagon(Vector2Int position)
        {
            HexNode newNode = AcquireNode();
    
            newNode.coordinate = position;
            newNode.GetComponentInChildren<Text>().text = $"{position.x}, {position.y}";
            newNode.transform.position = HexGrid.GetWorldPos(position);
            visibleNodes.Add(position, newNode.GetComponent<HexNode>());
        }
    }

    private void RevealHexagonsAround(Vector2Int gridCoordinate)
    {
        Vector2Int[] neighbours = HexGrid.GetNeighboursAt(gridCoordinate);
        DrawHexagons(neighbours);
    }

    private NodeObject GenerateObject()
    {
        return Random.Range(0, 100) < 20 ? soldierPrefab : null;
    }


    
}
