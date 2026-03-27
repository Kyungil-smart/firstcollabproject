using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomWalk : MonoBehaviour
{
    [SerializeField] private RoomManager visualizer;
    [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;
    
    // 만들어져야 할 방의 개수
    public int targetRoomCount;

    private void Start()
    {
        RunProceduralGeneration();
    }
    
    /// <summary>
    /// 방 생성 메서드
    /// </summary>
    private void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk();
        visualizer.VisualizeMap(floorPositions);
    }

    private HashSet<Vector2Int> RunRandomWalk()
    {
        return ProceduralGeneration.RandomWalk(startPosition, targetRoomCount);
    }
}