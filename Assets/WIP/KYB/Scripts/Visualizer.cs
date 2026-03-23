using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private float roomOffset = 16f;
    
    private List<GameObject> _spawnedRooms = new List<GameObject>();

    public void VisualizeMap(HashSet<Vector2Int> floorPositions)
    {
        ClearMap();

        foreach (var position in floorPositions)
        { 
            Vector2 worldPosition = new Vector2(position.x *  roomOffset, position.y * roomOffset);
            
            GameObject newRoom = Instantiate(roomPrefab, worldPosition, Quaternion.identity);
            _spawnedRooms.Add(newRoom);
            newRoom.GetComponent<Room>().SetRoomIndex(_spawnedRooms.Count - 1);
        }
    }

    private void ClearMap()
    {
        foreach (var room in _spawnedRooms)
        {
            Destroy(room);
        }
        
        _spawnedRooms.Clear();
    }
}
