using Farming_related;
using UnityEngine;

namespace Inventory_related.Inventory_UI_Manager_V2
{
    public class InventoryV2UIManagerSoilDetector : MonoBehaviour
    {
        [SerializeField] private SoilPlantInteraction currentSoil;
        public void SetCurrentSoil(Component component, object isOverSoil)
        {
            SoilPlantInteraction soilPlantInteraction = (SoilPlantInteraction)component;
            bool isOverSoilBool = (bool)((object[])isOverSoil)[0];
            if (isOverSoilBool)
            {
                currentSoil = soilPlantInteraction;
            }
            else
            {
                currentSoil = null;
            }  
        }

        public SoilPlantInteraction GetCurrentSoil()
        {
            return currentSoil;
        }
        
    }
}
