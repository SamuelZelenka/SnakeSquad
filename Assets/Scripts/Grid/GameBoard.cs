using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private PrefabContainer prefabContainer;

    [SerializeField, Range(0, 100)] private float _spawnRate = 2;
    [SerializeField, Range(1, 5)] private int _revealRange = 2;

    private static GameBoard _instance;
    
    public Dictionary<Vector2Int, NodeObject> nodeObjects = new Dictionary<Vector2Int, NodeObject>();
    public Dictionary<Vector2Int, HexNode> visibleNodes = new Dictionary<Vector2Int, HexNode>();

    private GameObjectPool<HexNode> _nodePool;

    private void Awake()
    {
        HexNode hexNodePrefab = prefabContainer.GetPrefab<HexNode>().GetComponent<HexNode>();
        _nodePool = new GameObjectPool<HexNode>(hexNodePrefab, transform);

        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Squad.onMoveTick += RevealHexagonsAroundSquad;
    }

    public static void SetNodeObjectAt(Vector2Int coordinate, NodeObject nodeObject)
    {
        if (!_instance.nodeObjects.ContainsKey(coordinate))
        {
            _instance.nodeObjects.Add(coordinate, nodeObject);
        }
    }

    public static void RemoveNodeObjectAt(Vector2Int coordinate)
    {
        _instance.nodeObjects.Remove(coordinate);
    }

    public static void ReleaseNode(HexNode node)
    {
        _instance._nodePool.Release(node);
        _instance.visibleNodes.Remove(node.coordinate);
    }

    public static NodeObject GetNodeObject(Vector2Int coordinate)
    {
        return _instance.nodeObjects.ContainsKey(coordinate) ? _instance.nodeObjects[coordinate] : null;
    }

    private void DrawHexagons(Vector2Int[] nodeCoordinates)
    {
        foreach (Vector2Int nodeCoordinate in nodeCoordinates)
        {
            if (Application.isPlaying && !visibleNodes.ContainsKey(nodeCoordinate))
            {
                DrawHexagon(nodeCoordinate);
                if (!nodeObjects.ContainsKey(nodeCoordinate) && SpawnNewObject())
                {
                    NodeObject generatedObject = SpawnAt<NewMember>(nodeCoordinate);
                    
                    generatedObject.SetCoordinate(nodeCoordinate);
                    
                    nodeObjects[nodeCoordinate] = generatedObject;
                    SetNodeObjectAt(nodeCoordinate, generatedObject);
                }
            }  
        }
        void DrawHexagon(Vector2Int coordinate)
        {
            HexNode newNode = _nodePool.Acquire();
            newNode.coordinate = coordinate;
            newNode.transform.position = HexGrid.GetWorldPos(coordinate);
            visibleNodes.Add(coordinate, newNode);
        }
    }

    private void RevealHexagonsAroundSquad(Vector2Int moveToCoordinate, Squad squad)
    {
        HashSet<Vector2Int> neighbours = new HashSet<Vector2Int>();

        GetHexagonsInRange(moveToCoordinate, _revealRange);

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
    
    public static T SpawnAt<T>(Vector2Int coordinate) where T : MonoBehaviour
    {
        GameObject spawnPrefab = _instance.prefabContainer.GetPrefab<T>() as GameObject;
        GameObject spawnedObject = Instantiate(spawnPrefab, HexGrid.GetWorldPos(coordinate), Quaternion.identity);
        return spawnedObject.GetComponent<T>();
    }

    //Future problem Make better way to spawn stuff
    private bool SpawnNewObject()
    {
        return Random.Range(0, 100) < _spawnRate;
    }
}
