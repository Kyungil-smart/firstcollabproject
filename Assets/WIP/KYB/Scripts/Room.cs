using System;
using Monster;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public enum RoomType
{
    StartRoom,
    NormalRoom,
    BossRoom
}

public class Room : MonoBehaviour
{  
    public Monster.MonsterSpawner spawner;
    public List<Transform> spawnPoints;

    private void Start()
    {
        StartCoroutine(SpawnPointRoutine());
    }

    private IEnumerator SpawnPointRoutine()
    {
        while (true)
        {
            List<Vector2Int> offScreenPoint = new List<Vector2Int>();

            foreach (var point in spawnPoints)
            {
                Vector3 viewPos = Camera.main.WorldToViewportPoint(point.position);

                if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
                {
                    Vector2Int girdPos = new Vector2Int(Mathf.RoundToInt(point.position.x), Mathf.RoundToInt(point.position.y));
                    offScreenPoint.Add(girdPos);
                }
            }

            if (spawner != null && spawnPoints.Count > 0)
            {
                spawner.UpdateSpawnableTiles(offScreenPoint);
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }

    [Header("Door Prefabs")] 
    [SerializeField] private GameObject upDoor;
    [SerializeField] private GameObject downDoor;
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;

    public void PlaceDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up) upDoor.SetActive(true); 
        else if (direction == Vector2Int.down) downDoor.SetActive(true);
        else if (direction == Vector2Int.left) leftDoor.SetActive(true);
        else if (direction == Vector2Int.right) rightDoor.SetActive(true);
    }
    
    
}
