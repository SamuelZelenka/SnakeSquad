using System.Collections.Generic;
using UnityEngine;

public enum HexDirection { E, SE, SW, W, NW, NE }

public static class HexGrid
{
    public static Vector3 GetWorldPos(Vector2Int gridCoordinate)
    {
        const float GRID_OFFSET = 0.5f;
        float xOffset = gridCoordinate.y % 2 == 0 ? 0 : GRID_OFFSET;
        float xPos = gridCoordinate.x + xOffset;
        float yPos = gridCoordinate.y;
        
        return new Vector3(xPos, yPos, 0);
    }

    public static Vector2Int[] GetNeighboursAt(Vector2Int gridCoordinate)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        for (int direction = 0; direction < (int)HexDirection.NE + 1; direction++)
        {
            neighbours.Add(GetCoordinateInDirection(gridCoordinate, (HexDirection)direction));
        }
        return neighbours.ToArray();
    }

    public static Vector2Int GetCoordinateInDirection(Vector2Int origin, HexDirection direction) => GetDirectionVector(origin, direction) + origin;

    public static Vector2Int GetDirectionVector(Vector2Int origin, HexDirection direction)
    {
        switch (direction)
        {
            case HexDirection.E:
                return new Vector2Int(1, 0);

            case HexDirection.SE:
                if (origin.y % 2 == 0)
                {
                    return new Vector2Int(0, -1);
                }
                return new Vector2Int(1, -1);

            case HexDirection.SW:
                if (origin.y % 2 == 0)
                {
                    return new Vector2Int(-1, -1);
                }
                return new Vector2Int(0, -1);

            case HexDirection.W:
                return new Vector2Int(-1, 0);

            case HexDirection.NW:
                if (origin.y % 2 == 0)
                {
                    return new Vector2Int(-1, 1);
                }
                return new Vector2Int(0, 1);

            case HexDirection.NE:
                if (origin.y % 2 == 0)
                {
                    return new Vector2Int(0, 1);
                }
                return new Vector2Int(1, 1);
            default:
                return Vector2Int.zero;
        }
    }
}
