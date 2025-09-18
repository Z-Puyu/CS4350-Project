using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LockedTile : CustomTile
{
    // public bool canMove = false;

    // public override void MovePlayerToThisTile(PlayerExploration playerExploration, Vector3 pos)
    // {
    //     if (canMove)
    //     {
    //         playerExploration.MovePlayer(pos);
    //     }
    // }
    
    #if UNITY_EDITOR
    [MenuItem("Assets/Create/2D/Custom tiles/Locked tiles")]
       public static void CreateLockedTile()
       {
           string path = EditorUtility.SaveFilePanelInProject("Save locked Tile", "New locked Tile", "Asset", "Save locked Tile", "Assets");
           if (path == "")
               return;                           
           AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<LockedTile>(), path);
        }
#endif
}
