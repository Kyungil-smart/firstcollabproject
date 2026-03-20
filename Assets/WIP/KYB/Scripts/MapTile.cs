using System;
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

    /// <summary>
    /// 주변 바닥 배치 상태(2진수 문자열)를 분석해 알맞은 방향의 벽 타일을 칠해주는 메서드 
    /// </summary>
    /// <param name="position">벽을 칠할 좌표</param>
    /// <param name="binaryType">주변 바닥 유무를 나태내는 2진수 문자열</param>
    public void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        var typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        
        // 미완성입니다 나중에 벽 스프라이트가 얼마나 추가될지에 따라서 달라집니다
    }

    /// <summary>
    /// 대각선 및 모서리 주변 상황(8방향)을 분석해서 알맞은 타일을 칠해주는 메서드
    /// </summary>
    public void PaintSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        
        // 미완성입니다 나중에 벽 스프라이트가 얼마나 추가될지에 따라서 달라집니다
    }


    /// <summary>
    /// 맵 생성할 때 기존에 그려져 있던 타일맵의 모든 타일들을 초기화해주는 메서드
    /// </summary>
    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }
}   
