using Player_related.Player_things_to_note_ui_manager;
using SaintsField;
using UnityEngine;

    
[CreateAssetMenu(fileName = "PlayerMessage", menuName = "Player message")]
public class PlayerMessage : ScriptableObject
{
    [ResizableTextArea] public string text;
    public Color textColorOnSpawn;

    public void SpawnText(PlayerThingsToNoteUIManager playerThingsToNoteUIManager)
    {
        playerThingsToNoteUIManager.SpawnText(text, textColorOnSpawn);
    }
}
