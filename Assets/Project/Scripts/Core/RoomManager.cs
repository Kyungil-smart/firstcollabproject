using System;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomManager : MonoBehaviour
{
    // 싱글톤
    public static RoomManager Instance { get; private set; }

    [Header("방 데이터 세팅")] public int startStageId = 70001;

    [Header("방 프리팹 참조")]
    [SerializeField] private GameObject startRoomPrefab;
    [SerializeField] private GameObject bossRoomPrefab;
    [SerializeField] private List<GameObject> normalRoomPrefabs;

    [Header("NavMesh Surface")]
    [SerializeField] private NavMeshSurface navMeshSurfaces;
    
    [Header("방 크기에 맞춘 오프셋")]
    [SerializeField] private float roomOffset;
    
    private Dictionary<Vector2Int, Room> _roomDic =  new Dictionary<Vector2Int, Room>();
    private Queue<int> _roomIdQueue = new Queue<int>();

    // 싱글톤
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    /// <summary>
    /// 실제 맵을 찍어주는 메서드
    /// </summary>
    /// <param name="floorPositions"></param>
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
            // 오프셋에 맞게 방 배치
            Vector2 worldPosition = new Vector2(position.x *  roomOffset, position.y * roomOffset);
            GameObject spawnedRoom;

            // x= 0, y= 0이면 startRoom 배치
            if (position == Vector2Int.zero)
            {
                spawnedRoom = Instantiate(startRoomPrefab, worldPosition, Quaternion.identity);
            }
            else
            {
                // 인덱스 순환
                int index = roomIndex % normalRoomPrefabs.Count;
                spawnedRoom = Instantiate(normalRoomPrefabs[index], worldPosition, Quaternion.identity);
                roomIndex++;
                normalRoomCount++;

                float distance = Vector2.Distance(Vector2Int.zero, position);
                
                // 최댓값 구하는 알고리즘
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestPosition = position;
                }
            }

            Room room = spawnedRoom.GetComponent<Room>();
            _roomDic.Add(position,room);
        }

        // 가장 먼 곳에 있는 방을 "파괴"하고 보스방을 생성
        if (farthestPosition != Vector2.zero)
        {
            DestroyImmediate(_roomDic[farthestPosition].gameObject);
            
            Vector2 bossWorldPos = new Vector2(farthestPosition.x * roomOffset, farthestPosition.y * roomOffset);
            GameObject bossRoom = Instantiate(bossRoomPrefab, bossWorldPos, Quaternion.identity);
            
            _roomDic[farthestPosition] = bossRoom.GetComponent<Room>();
        }
        
        TrySpawnDoors();
        InitStageQueue(normalRoomCount);
        
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
    }

    /// <summary>
    /// Normal Room에만 Id가 들어가게 for문 돌려주는 메서드
    /// </summary>
    /// <param name="roomQueue">normalRoomCount</param>
    private void InitStageQueue(int roomQueue)
    {
        _roomIdQueue.Clear();

        for (int i = 0; i < roomQueue; i++)
        {
            _roomIdQueue.Enqueue(startStageId + i);
        }
    }

    /// <summary>
    /// 현재 위치 + 방향벡터 이용해서 넘겨주는 메서드
    /// </summary>
    private void TrySpawnDoors()
    {
        foreach (var roomKey in _roomDic)
        {
            Vector2Int currentPosition = roomKey.Key;
            Room currentRoom = roomKey.Value;
            RoomDirection roomDir = RoomDirection.None;

            if (_roomDic.ContainsKey(currentPosition + Vector2Int.up))
                roomDir |= RoomDirection.Up;
                
            if (_roomDic.ContainsKey(currentPosition + Vector2Int.down))
                roomDir |= RoomDirection.Down;
            
            if (_roomDic.ContainsKey(currentPosition + Vector2Int.left))
                roomDir |= RoomDirection.Left;
            
            if (_roomDic.ContainsKey(currentPosition + Vector2Int.right))
                roomDir |= RoomDirection.Right;
            
            currentRoom.SetRoomConnection(roomDir);
        }
    }

    /// <summary>
    /// 전에 깔려 있던 그리드맵 없애주는 메서드
    /// </summary>
    private void ClearGrid()
    {
        foreach (var room in _roomDic.Values)
        {
            DestroyImmediate(room.gameObject); 
        }
        
        _roomDic.Clear();
    }
    
    
    /// <summary>
    /// 피셔-예이츠 셔플 알고리즘
    /// </summary>
    private void ShuffleRoom<T>(List<T> roomList)
    {
        for (int i = 0; i < roomList.Count - 1; i++)
        {
            var randomRoomIndex = Random.Range(i, roomList.Count);
            Swap(roomList, i, randomRoomIndex);
        }
    }

    /// <summary>
    /// 피셔-예이츠 셔플에 필요한 스왑 메서드
    /// </summary>
    private void Swap<T>(List<T> roomList, int i, int randomRoomIndex)
    {
        // 튜플 처리
        // (n, m) = (m, n);
        // (roomList[i], roomList[randomRoomIndex]) = (roomList[randomRoomIndex], roomList[i]);
        
        var temp = roomList[i];
        roomList[i] = roomList[randomRoomIndex];
        roomList[randomRoomIndex] = temp;
    }

    /// <summary>
    /// SO 데이터에 맞게 Queue로 방 ID 관리해주는 메서드
    /// </summary>
    /// <returns></returns>
    public int GetNextStageId()
    {
        if (_roomIdQueue.Count > 0)
        {
            return _roomIdQueue.Dequeue();
        }

        return startStageId;
    }
}
