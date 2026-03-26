using System;
using Monster;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Room : MonoBehaviour
{  
    [Header("Room Info")]
    public RoomType roomType;
    
    public MonsterSpawner spawner;
    public List<Transform> spawnPoints;

    private void Start()
    {
        Debug.Log($"[Room Info] 위치: {transform.position}, 타입: {roomType}");
        
        StartCoroutine(SpawnPointRoutine());
    }

    /// <summary>
    /// 스폰 포인트에 대한 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnPointRoutine()
    {
        while (true)
        {
            List<Vector2Int> offScreenPoint = new List<Vector2Int>();

            foreach (var point in spawnPoints)
            {
                Vector3 viewPos = Camera.main.WorldToViewportPoint(point.position);
                
                // WorldToViewportPoint를 써서 카메라 뷰 안에 스폰 포인트가 있는지 확인
                if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
                {
                    Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(point.position.x), Mathf.RoundToInt(point.position.y));
                    offScreenPoint.Add(gridPos);
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

    
    public void ClearRoom()
    {
        
    }
    
    /// <summary>
    /// 방에 들어왔을 때, 스포너를 모두 받아오는 메서드
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ool isVisit = false;
        
        if (other.CompareTag("Player"))
        {
            if (roomType == RoomType.NormalRoom)
            {
                MonsterManager.Instance.monsterSpawner = this.spawner;
            }
            
            MonsterManager.Instance.StartStage();
        }
        
        // 아직 미완성입니다 계속 보완해 가겠습니다
    }
    
}
