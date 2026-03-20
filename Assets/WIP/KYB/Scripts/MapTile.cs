using UnityEngine;
using UnityEngine.Tilemaps;

public class Tilemap : MonoBehaviour
{
    [Header("Grid Tilemap")] [SerializeField] private Tilemap floorTilemap, wallTilemap; // 장애물 필요하면 추가 obstacleTile < 아마 오브젝트 처리할 거 같긴한데.. 

    [Header("기본 타일 스프라이트")] 
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    // 일단 벽 타일만 해놓고 필요할 때 변수 늘리기 ex) wallSideRight, wallSideLeft

    // [Header("모서리용 타일 스프라이트")]
    
    
}
