using System;
using Monster;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.AI;

public class Room : MonoBehaviour
{  
    [Header("Room Info")]
    public RoomType roomType;
    public List<Transform> spawnPoints;
    
    [Header("보스 프리팹")]
    [SerializeField] GameObject _bossPrefab;

    // 방문 확인용
    [SerializeField] private bool isVisited = false;
    public static event Action<Room> OnRoomEntered;
    // whlie문에서 new List, Camera.main 호출 방지 변수
    private Camera _mainCam;
    private List<Vector2Int> _offScreenPoint = new List<Vector2Int>();
    
    // 클리어 확인 용
    [SerializeField] private bool _isCleared = false;

    [Header("문 프리팹")] 
    [SerializeField] private GameObject upDoor;
    [SerializeField] private GameObject downDoor;
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;

    [Header("예비벽 프리팹")] 
    [SerializeField] private GameObject upReserveWall;
    [SerializeField] private GameObject downReserveWall;
    [SerializeField] private GameObject leftReserveWall;
    [SerializeField] private GameObject rightReserveWall;
    
    /*
    private void Start()
    {
        Debug.Log($"[Room Info] 위치: {transform.position}, 타입: {roomType}");
        
        StartCoroutine(SpawnPointRoutine());
    }
    */

    private void Awake()
    {
        _mainCam = Camera.main;
    }

    /// <summary>
    /// 스폰 포인트에 대한 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnPointRoutine()
    {
        if (_mainCam == null)
        {
            Debug.LogWarning("MainCam이 Null입니다.");
            yield break;
        }
        
        while (!_isCleared)
        {
            _offScreenPoint.Clear();

            foreach (var point in spawnPoints)
            {
                Vector3 viewPos = _mainCam.WorldToViewportPoint(point.position);
                
                // WorldToViewportPoint를 써서 카메라 뷰 안에 스폰 포인트가 있는지 확인
                if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
                {
                    Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(point.position.x), Mathf.RoundToInt(point.position.y));
                    _offScreenPoint.Add(gridPos);
                }
            }
            
            if (MonsterManager.Instance.monsterSpawner != null && spawnPoints.Count > 0)
            {
                MonsterManager.Instance.monsterSpawner.UpdateSpawnableTiles(_offScreenPoint);
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// 보스 스폰해주는 메서드
    /// </summary>
    private void SpawnBoss()
    {
        // MonsterManager.Instance.isStageCleared = false;

        Vector3 center = this.transform.position;

        if (NavMesh.SamplePosition(center, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            center = hit.position;

        GameObject bossObj = Instantiate(_bossPrefab, center, Quaternion.identity);

        var boss = bossObj.GetComponent<MonsterAction>();
        if (boss != null)
        {
            boss.Init();
        }
    }

    /*
    /// <summary>
    /// 현재 방과 이웃 방에 문 배치해주는 메서드
    /// </summary>
    /// <param name="direction"></param>
    public void PlaceDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up) upDoor.SetActive(true); 
        else if (direction == Vector2Int.down) downDoor.SetActive(true);
        else if (direction == Vector2Int.left) leftDoor.SetActive(true);
        else if (direction == Vector2Int.right) rightDoor.SetActive(true);
    }
    */

    /// <summary>
    /// 비트마스킹을 이용해 문과 벽 배치
    /// </summary>
    /// <param name="roomDic"></param>
    public void SetRoomConnection(RoomDirection roomDic)
    {
        bool upFlag = roomDic.HasFlag(RoomDirection.Up);
        bool downFlag = roomDic.HasFlag(RoomDirection.Down);
        bool leftFlag = roomDic.HasFlag(RoomDirection.Left);
        bool rightFlag = roomDic.HasFlag(RoomDirection.Right);
        
        upDoor.SetActive(upFlag);
        upReserveWall.SetActive(!upFlag);
        
        downDoor.SetActive(downFlag);
        downReserveWall.SetActive(!downFlag);
        
        leftDoor.SetActive(leftFlag);
        leftReserveWall.SetActive(!leftFlag);
            
        rightDoor.SetActive(rightFlag);
        rightReserveWall.SetActive(!rightFlag);
    }
    
    /// <summary>
    /// 클리어 됐을 때 문 열기
    /// </summary>
    public void ClearRoom()
    {
        if (_isCleared) return;
        _isCleared = true;

        if (upDoor.activeSelf && upDoor.TryGetComponent(out Door up)) up.Open();
        if (downDoor.activeSelf && downDoor.TryGetComponent(out Door down)) down.Open();
        if (leftDoor.activeSelf && leftDoor.TryGetComponent(out Door left)) left.Open();
        if (rightDoor.activeSelf && rightDoor.TryGetComponent(out Door right)) right.Open();
    }

    /// <summary>
    /// 방에 들어왔을 때 문 닫기
    /// </summary>
    private void CloseDoors()
    {
        if (upDoor.activeSelf && upDoor.TryGetComponent(out Door up)) up.Close();
        if (downDoor.activeSelf && downDoor.TryGetComponent(out Door down)) down.Close();
        if (leftDoor.activeSelf && leftDoor.TryGetComponent(out Door left)) left.Close();
        if (rightDoor.activeSelf && rightDoor.TryGetComponent(out Door right)) right.Close();
    }

    
    /// <summary>
    /// Normal Room & Boss Room 체크 해주는 메서드
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_isCleared) return;

            if (!isVisited)
            {
                isVisited = true;
                CloseDoors();

                if (roomType == RoomType.NormalRoom)
                {
                    Debug.Log("일반 방 진입!!");
                    StartCoroutine(SpawnPointRoutine());
                    
                    int currentStageId = RoomManager.Instance.GetNextStageId();
                    Debug.Log($"roomID: {currentStageId}");
                
                    GameManager.Instance.currentStage = currentStageId;

                    MonsterManager.Instance.currentRoom = this;
                    MonsterManager.Instance.StartStage();
                    OnRoomEntered?.Invoke(this);
                }
                else if (roomType == RoomType.BossRoom)
                {
                    GameManager.Instance.isBossRoom = true;
                    MonsterManager.Instance.currentRoom = this;
                    Debug.Log("보스 방 진입!!");
                    OnRoomEntered?.Invoke(this);
                    SpawnBoss();
                }
            }
        }
    }
}
