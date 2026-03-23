using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGeneration
{
    private static List<GameObject> _spawnedRooms = new List<GameObject>();
    
    /// <summary>
    /// Random Walk 알고리즘
    /// </summary>
    /// <param name="startPosition">시작할 좌표</param>
    /// <param name="targetRoomCount">만들어질 방의 개수</param>
    /// <returns></returns>
    public static HashSet<Vector2Int> RandomWalk(Vector2Int startPosition, int targetRoomCount)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        path.Add(startPosition);
        Vector2Int previousPosition = startPosition;

        while (path.Count < targetRoomCount)
        {
            Vector2Int newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }

        return path;
    }
}

public static class Direction2D
{
    public static List<Vector2Int> CardinalDirectionList = new List<Vector2Int>
    {
        new Vector2Int(0, 1), // Up
        new Vector2Int(1, 0), // Right
        new Vector2Int(0, -1), // Down
        new Vector2Int(-1, 0) // Left
    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return CardinalDirectionList[Random.Range(0, CardinalDirectionList.Count)];
    }
}
