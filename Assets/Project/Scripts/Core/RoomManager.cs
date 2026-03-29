using System;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [Header("방 데이터 세팅")] public int startStageId = 70001;

    [Header("방 프리팹 참조")]
    [SerializeField] private GameObject startRoomPrefab;
    [SerializeField] private GameObject bossRoomPrefab;
    [SerializeField] private List<GameObject> normalRoomPrefabs;
    
    // [Header("플레이어 프리팹 참조")]
    // [SerializeField] private GameObject playerPrefab;

    [Header("NavMesh Surface")]
    [SerializeField]
    private NavMeshSurface navMeshSurfaces;
    
    [Header("방 크기에 맞춘 오프셋")]
    [SerializeField] private float roomOffset;
    
    private Dictionary<Vector2Int, Room> _roomDic =  new Dictionary<Vector2Int, Room>();
    private Queue<int> _roomIdQueue = new Queue<int>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    public void VisualizeMap(HashSet<Vector2Int> floorPositions)
    {
        ClearGrid(); 
        ShuffleRoom(normalRoomPrefabs);

        int roomIndex = 0;
        int normalRoomCount = 0;
        float maxDistance = 0f;
        Vector2Int farthestPosition = Vector2Int.zero;
        
        foreach (var position in floorPositions)
        { 
            Vector2 worldPosition = new Vector2(position.x *  roomOffset, position.y * roomOffset);
            GameObject spawnedRoom;

            // 1. startRoom 배치
            if (position == Vector2Int.zero)
            {
                spawnedRoom = Instantiate(startRoomPrefab, worldPosition, Quaternion.identity);
            }
            else
            {
                int index = roomIndex % normalRoomPrefabs.Count;
                spawnedRoom = Instantiate(normalRoomPrefabs[index], worldPosition, Quaternion.identity);
                roomIndex++;
                normalRoomCount++;

                float distance = Vector2.Distance(Vector2Int.zero, position);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestPosition = position;
                }
            }

            Room room = spawnedRoom.GetComponent<Room>();
            _roomDic.Add(position,room);
        }

        if (farthestPosition != Vector2.zero)
        {
            DestroyImmediate(_roomDic[farthestPosition].gameObject);
            
            Vector2 bossWorldPos = new Vector2(farthestPosition.x * roomOffset, farthestPosition.y * roomOffset);
            GameObject bossRoom = Instantiate(bossRoomPrefab, bossWorldPos, Quaternion.identity);
            
            _roomDic[farthestPosition] = bossRoom.GetComponent<Room>();
        }
        
        TrySpawnDoors();
        InitStageQueue(normalRoomCount);
        
        //TODO: NavMesh로 맵 굽기
        
        // 방들의 Collider 좌표를 모두 동기화
        Physics2D.SyncTransforms();

        if (navMeshSurfaces != null)
        {
            navMeshSurfaces.RemoveData(); // 전에 베이크 됐던 네브메쉬 삭제
            // navMeshSurfaces.BuildNavMeshAsync(); // 비동기 방식으로 런타임에 Bake
            navMeshSurfaces.BuildNavMesh(); // 둘 중에 뭐가 낫지? 
            Debug.Log("NavMesh Bake Complete");
        }
        else
        {
            Debug.Log("NavMeshSurface 연결 되지 않았음");
        }

        /*
        if (playerPrefab != null)
        {
            Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }
        */
    }

    private void InitStageQueue(int roomQueue)
    {
        _roomIdQueue.Clear();

        for (int i = 0; i < roomQueue; i++)
        {
            _roomIdQueue.Enqueue(startStageId + i);
        }
    }

    private void TrySpawnDoors()
    {
        foreach (var roomKey in _roomDic)
        {
            Vector2Int currentPosition = roomKey.Key;
            Room currentRoom = roomKey.Value;

            Vector2Int[] checkDirections = { Vector2Int.up, Vector2Int.right };

            foreach (var direction in checkDirections)
            {
                Vector2Int targetPosition = currentPosition + direction;

                if (_roomDic.TryGetValue(targetPosition, out Room neighborRoom))
                {
                    if (direction == Vector2Int.up)
                    {
                        currentRoom.PlaceDoor(Vector2Int.up);
                        neighborRoom.PlaceDoor(Vector2Int.down);
                    }
                    else
                    {
                        currentRoom.PlaceDoor(Vector2Int.right);
                        neighborRoom.PlaceDoor(Vector2Int.left);
                    }
                }
            }
        }
    }

    private void ClearGrid()
    {
        foreach (var room in _roomDic.Values)
        {
            DestroyImmediate(room.gameObject);
        }
        
        _roomDic.Clear();
    }
    
    
    
    private void ShuffleRoom<T>(List<T> roomList)
    {
        for (int i = 0; i < roomList.Count - 1; i++)
        {
            var randomRoomIndex = Random.Range(i, roomList.Count);
            Swap(roomList, i, randomRoomIndex);
        }
    }

    private void Swap<T>(List<T> roomList, int i, int randomRoomIndex)
    {
        var temp = roomList[i]; 
        roomList[i] = roomList[randomRoomIndex];
        roomList[randomRoomIndex] = temp;
    }

    public int GetNextStageId()
    {
        if (_roomIdQueue.Count > 0)
        {
            return _roomIdQueue.Dequeue();
        }

        return startStageId;
    }
}
