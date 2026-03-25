using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomWalk : MonoBehaviour
{
    [SerializeField] private RoomManager visualizer;
    
    [SerializeField] 
    protected Vector2Int startPosition = Vector2Int.zero;
    
    public int targetRoomCount;


    public void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk();
        visualizer.VisualizeMap(floorPositions);
    }

    private HashSet<Vector2Int> RunRandomWalk()
    {
        return ProceduralGeneration.RandomWalk(startPosition, targetRoomCount);
    }
}