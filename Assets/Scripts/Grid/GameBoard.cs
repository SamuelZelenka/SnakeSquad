using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private PrefabContainer prefabContainer;

    [SerializeField, Range(0, 100)] private int _fruitSpawnChance = 2;
    [SerializeField, Range(0, 100)] private int _wallSpawnChance = 15;
    [SerializeField, Range(1, 5)] private int _revealRange = 2;
    [SerializeField] private string _worldSeed = "";
    [SerializeField] private float _safeSpawnRange;
    [SerializeField] private AnimationCurve _fadeCurve;

    private static GameBoard _instance;
    
    private readonly Dictionary<Vector2Int, NodeObject> _nodeObjects = new Dictionary<Vector2Int, NodeObject>();
    private readonly Dictionary<Vector2Int, HexNode> _visibleNodes = new Dictionary<Vector2Int, HexNode>();
    
    private GameObjectPool<HexNode> _nodePool;

    public static float SafeSpawnRange => _instance._safeSpawnRange;
    public static float WallSpawnChance => _instance._wallSpawnChance;
    public static float RevealRange => _instance._revealRange;

    private static int WorldSeed => (int)(_instance._worldSeed.GetHashCode() * 1/WallSpawnChance); 

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        if (_worldSeed == "")
        {
            _worldSeed = Random.Range(int.MinValue, int.MaxValue).ToString();
        }

        print(WorldSeed);
        HexNode hexNodePrefab = prefabContainer.GetPrefab<HexNode>().GetComponent<HexNode>();
        _nodePool = new GameObjectPool<HexNode>(hexNodePrefab, transform);

        Snake.onMoveTick += RevealHexagonsAroundSquad;
        GameSession.onGameReset += RevealHexagonsInit;
        RevealHexagonsAroundSquad(Vector2Int.zero);
    }

    public static void SetNodeObjectAt(Vector2Int coordinate, NodeObject nodeObject)
    {
        if (!_instance._nodeObjects.ContainsKey(coordinate))
        {
            _instance._nodeObjects.Add(coordinate, nodeObject);
        }
    }

    public static void RemoveNodeObjectAt(Vector2Int coordinate)
    {
        _instance._nodeObjects.Remove(coordinate);
    }

    public static void ReleaseNode(HexNode node)
    {
        _instance._nodePool.Release(node);
        _instance._visibleNodes.Remove(node.coordinate);
    }
    
    public static NodeObject GetObjectAt(Vector2Int coordinate)
    {
        return _instance._nodeObjects.ContainsKey(coordinate) ?  _instance._nodeObjects[coordinate] : null;
    }
    public static HexNode GetHexNode(Vector2Int coordinate)
    {
        return _instance._visibleNodes.ContainsKey(coordinate) ?  _instance._visibleNodes[coordinate] : null;
    }
    public static int RandomNumberByCoordinate(Vector2Int coordinate, int min, int max)
    {
        Random.InitState(coordinate.x * coordinate.y + WorldSeed);
        return Random.Range(min, max);
    }

    public static float GetFadeValue(float percentage)
    {
        return _instance._fadeCurve.Evaluate(percentage);
    }
    private void AcquireHexagons(Vector2Int[] nodeCoordinates)
    {
        foreach (Vector2Int nodeCoordinate in nodeCoordinates)
        {
            if (Application.isPlaying && !_visibleNodes.ContainsKey(nodeCoordinate))
            {
                AcquireHexagon(nodeCoordinate);
            }  
        }
        void AcquireHexagon(Vector2Int coordinate)
        {
            HexNode newNode = _nodePool.Acquire();
            newNode.UpdateNode(coordinate, SpawnNewObject(coordinate));
            _visibleNodes.Add(coordinate, newNode);
        }
    }

    private void RevealHexagonsAroundSquad(Vector2Int moveToCoordinate)
    {
        HashSet<Vector2Int> neighbours = new HashSet<Vector2Int>();

        GetHexagonsInRange(moveToCoordinate, _revealRange);

        AcquireHexagons(neighbours.ToArray());
        
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
    private void RevealHexagonsInit()
    {
        RevealHexagonsAroundSquad(new Vector2Int(0, 0));
    }
    
    public static T SpawnAt<T>(Vector2Int coordinate) where T : MonoBehaviour
    {
        T spawnPrefab = _instance.prefabContainer.GetPrefab<T>();
        T spawnedObject = Instantiate(spawnPrefab, HexGrid.GetWorldPos(coordinate), Quaternion.identity);
        return spawnedObject.GetComponent<T>();
    }

    private NodeObject SpawnNewObject(Vector2Int coordinate)
    {
        bool hasSpawned = Random.Range(1, 101) < _fruitSpawnChance && !HexNode.IsCoordinateWall(coordinate);
        
        NodeObject nodeObject = null;
        if (coordinate != Vector2Int.zero)
        {
            if (_instance._nodeObjects.ContainsKey(coordinate))
            {
                nodeObject = _instance._nodeObjects[coordinate];
            }
            else
            {
                nodeObject = hasSpawned ? SpawnAt<PickUpSegmentObject>(coordinate) : null;
                if (hasSpawned)
                {
                    nodeObject.SetCoordinate(coordinate);
                    _nodeObjects.Add(coordinate, nodeObject);
                }
            }
        }
        return nodeObject;
    }
}
