using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private float roomOffset = 1f;
    
    private Dictionary<Vector2Int, Room> _roomDic =  new Dictionary<Vector2Int, Room>();
    // private List<GameObject> _spawnedRooms = new List<GameObject>();

    public void VisualizeMap(HashSet<Vector2Int> floorPositions)
    {
        ClearMap();
        int roomIndex = 0;
        
        foreach (var position in floorPositions)
        { 
            Vector2 worldPosition = new Vector2(position.x *  roomOffset, position.y * roomOffset);
            
            GameObject newRoom = Instantiate(roomPrefab, worldPosition, Quaternion.identity);
            Room room = newRoom.GetComponent<Room>();
            room.SetRoomIndex(roomIndex++);
            
            _roomDic.Add(position,room);
            
            TrySpawnDoors();
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
                        currentRoom.PlaceDoor(Direction2D.CardinalDirectionList[2]);
                        neighborRoom.PlaceDoor(Direction2D.CardinalDirectionList[0]);
                    }
                    else
                    {
                        currentRoom.PlaceDoor(Direction2D.CardinalDirectionList[3]);
                        neighborRoom.PlaceDoor(Direction2D.CardinalDirectionList[1]);
                    }
                }
            }
        }
    }

    private void ClearMap()
    {
        foreach (var room in _roomDic.Values)
        {
            Destroy(room.gameObject);
        }
        
        _roomDic.Clear();
    }
}
