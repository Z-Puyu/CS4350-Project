using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class GirdManager : MonoBehaviour
{
    //public PlayerExploration playerExploration;
    private Tilemap tilemap;
    private CustomTile selectedTile;

    void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
    }

    // void OnClick()
    // {
    //     Vector2 mousePos = Mouse.current.position.ReadValue();
    //     Vector3Int cellPos = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(mousePos));
    //     selectedTile = (CustomTile)tilemap.GetTile(cellPos);
    //     if (selectedTile != null)
    //     {
    //         selectedTile.MovePlayerToThisTile(playerExploration, tilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.25f, 0));   
    //     }
    //     // if (!(selectedTile is LockedTile))
    //     // {
    //     //     playerExploration.MovePlayer(tilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.25f, 0));
    //     // }
    //     // Debug.Log(selectedTile);
    // }


}
