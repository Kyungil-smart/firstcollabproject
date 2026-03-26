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
    public List<Transform> spawnPoints;

    // 방문 확인용
    [SerializeField]private bool isVisited = false;
    
    /*
    private void Start()
    {
        Debug.Log($"[Room Info] 위치: {transform.position}, 타입: {roomType}");
        
        StartCoroutine(SpawnPointRoutine());
    }
    */

    /// <summary>
    /// 스폰 포인트에 대한 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnPointRoutine()
    {
        while (!_isCleared)
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
            
            if (MonsterManager.Instance.monsterSpawner != null && spawnPoints.Count > 0)
            {
                MonsterManager.Instance.monsterSpawner.UpdateSpawnableTiles(offScreenPoint);
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

    private bool _isCleared = false;
    
    public void ClearRoom()
    {
        if (_isCleared) return;

        _isCleared = true;

        Door.OpenDoors();
    }
    
    /// <summary>
    /// 방에 들어왔을 때, 스포너를 모두 받아오는 메서드
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (roomType == RoomType.NormalRoom && !isVisited)
            {
                isVisited = true;
                
                StartCoroutine(SpawnPointRoutine());
                
                int currentStageId = RoomManager.Instance.GetNextStageId();
                Debug.Log($"roomID: {currentStageId}");
                
                GameManager.Instance.currentStage = currentStageId;
                
                MonsterManager.Instance.StartStage();
            }
        }
    }
    
}
