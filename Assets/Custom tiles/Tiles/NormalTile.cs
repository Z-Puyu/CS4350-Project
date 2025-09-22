using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NormalTile : CustomTile
{
    // public override void MovePlayerToThisTile(PlayerExploration playerExploration, Vector3 pos)
    // {
    //     playerExploration.MovePlayer(pos);
    // }
    
    #if UNITY_EDITOR
    [MenuItem("Assets/Create/2D/Custom tiles/Normal tiles")]
       public static void CreateNormalTile()
       {
           string path = EditorUtility.SaveFilePanelInProject("Save normal Tile", "New normal Tile", "Asset", "Save normal Tile", "Assets");
           if (path == "")
               return;                           
           AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<NormalTile>(), path);
        }
#endif
}
