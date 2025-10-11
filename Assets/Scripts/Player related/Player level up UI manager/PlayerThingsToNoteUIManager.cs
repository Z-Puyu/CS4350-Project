using Map;
using Player_related.Things_to_note_text;
using UnityEngine;

namespace Player_related.Player_things_to_note_ui_manager
{
    public class PlayerThingsToNoteUIManager : MonoBehaviour
    {
        public Transform backdrop;
        public ThingsToNoteText thingsToNoteText;
        [SerializeField] private int timeBeforeTextVanish;
        
        public void SpawnCombatLevelUpText(Component component, object change)
        {
            int levelChange = (int)((object[])change)[0];
            SpawnText("Combat level increased by " + levelChange.ToString());
        }
        
        public void SpawnFarmingLevelUpText(Component component, object change)
        {
            int levelChange = (int)((object[])change)[0];
            SpawnText("Farming level increased by " + levelChange.ToString());
        }
        
        public void SpawnClearRegionUnlockRequirementText(Component component, object mURSO)
        {
            MapUnlockRequirementSO mapUnlockRequirementSo = (MapUnlockRequirementSO)((object[])mURSO)[0];
            mapUnlockRequirementSo.SpawnTextWhenObjectiveIsCleared(this);
        }
        
        public void SpawnUnlockRegionText(Component component, object mURSO)
        {
            MapUnlockRequirementSO mapUnlockRequirementSo = (MapUnlockRequirementSO)((object[])mURSO)[0];
            mapUnlockRequirementSo.SpawnTextWhenObjectiveIsCleared(this);
        }

        public void SpawnText(string text)
        {
            GameObject levelUpText = Instantiate(thingsToNoteText.gameObject, backdrop);
            levelUpText.GetComponent<ThingsToNoteText>().SetText(text, timeBeforeTextVanish);
        }
    }   
}
