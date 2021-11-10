using System.Collections.Generic;
using System.Linq;
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

        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Squad.onMoveTick += RevealHexagonsAround;
    }

    public HexNode AcquireNode()
    {
      return _nodePool.Acquire();  
    }

    public static void ReleaseNode(HexNode node)
    {
        _instance._nodePool.Release(node);
        _instance.visibleNodes.Remove(node.coordinate);
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
            newNode.transform.position = HexGrid.GetWorldPos(position);
            visibleNodes.Add(position, newNode.GetComponent<HexNode>());
        }
    }

    private void RevealHexagonsAround(Vector2Int gridCoordinate)
    {
        HashSet<Vector2Int> neighbours = new HashSet<Vector2Int>();
        GetHexagonsInRange(gridCoordinate, 2);

        DrawHexagons(neighbours.ToArray());
        
        Vector2Int[] GetHexagonsInRange(Vector2Int source, int range)
        {
            List<Vector2Int> sourceNeighbours = new List<Vector2Int>();
            if (range > 0)
            {
                foreach (Vector2Int neighbour in HexGrid.GetNeighboursAt(source))
                {
                    sourceNeighbours.AddRange(GetHexagonsInRange(neighbour, range - 1));
                    foreach (Vector2Int position in sourceNeighbours)
                    {
                        neighbours.Add(position);
                    }
                }
            }
            return HexGrid.GetNeighboursAt(source);
        }
    }

    

    private NodeObject GenerateObject()
    {
        return Random.Range(0, 100) < 20 ? soldierPrefab : null;
    }
}
