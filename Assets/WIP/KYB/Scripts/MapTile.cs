using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapTile : MonoBehaviour
{
    [Header("Grid Tilemap")] [SerializeField] private Tilemap floorTilemap, wallTilemap; // 장애물 필요하면 추가 obstacleTile < 아마 오브젝트 처리할 거 같긴한데.. 

    [Header("기본 타일 스프라이트")] 
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    // 일단 벽 타일만 해놓고 필요할 때 변수 늘리기 ex) wallSideRight, wallSideLeft

    // [Header("모서리용 타일 스프라이트")]

    
    /// <summary>
    /// 특정 타일맵의 여러 좌표에 동일한 타입을 반복해서 칠하는 메서드
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="tilemap"></param>
    /// <param name="tile"></param>
    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);   
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="floorPosition">바닥이 될 좌표들의 집합</param>
    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPosition)
    {
        PaintTiles(floorPosition, floorTilemap, floorTile);
    }

    /// <summary>
    /// 단일 좌표(Vector2Int)를 타일맵의 셀 좌표계(Vector3Int, WorldToCell)로 변환하여 타일을 배치하는 메서드
    /// </summary>
    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, floorTile);
    }

    public void PaintSingleTile(Vector2Int position, string binaryType)
    {
        
    }
}   
