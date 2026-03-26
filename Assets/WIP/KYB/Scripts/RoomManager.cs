using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("방 프리팹 참조")]
    [SerializeField] private GameObject startRoomPrefab;
    [SerializeField] private GameObject bossRoomPrefab;
    [SerializeField] private List<GameObject> normalRoomPrefabs;
    
    private Dictionary<Vector2Int, Room> _roomDic =  new Dictionary<Vector2Int, Room>();

    [SerializeField] private float roomOffset;
    
    
    public void VisualizeMap(HashSet<Vector2Int> floorPositions)
    {
        ClearGrid(); 
        ShuffleRoom(normalRoomPrefabs);

        int roomIndex = 0;
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
            Destroy(_roomDic[farthestPosition].gameObject);
            
            Vector2 bossWorldPos = new Vector2(farthestPosition.x * roomOffset, farthestPosition.y * roomOffset);
            GameObject bossRoom = Instantiate(bossRoomPrefab, bossWorldPos, Quaternion.identity);
            
            _roomDic[farthestPosition] = bossRoom.GetComponent<Room>();
        }
        
        TrySpawnDoors();
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
            Destroy(room.gameObject);
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
}
