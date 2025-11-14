using UnityEngine;
using SaintsField;
using UnityEngine.Tilemaps;

namespace Map.Map_manager
{
    public class MapManager : MonoBehaviour
    {
        public SaintsDictionary<MapUnlockRequirementSO, BoxCollider2D> mapRequirementToRegion;

        public void RequirementCompleted(Component component, object requirement)
        {
            MapUnlockRequirementSO mapRequirement = (MapUnlockRequirementSO)((object[])requirement)[0];
            BoxCollider2D collider = mapRequirementToRegion[mapRequirement];
            collider.gameObject.GetComponent<RegionBorder.RegionBorder>().UnlockRegion();
            collider.isTrigger = true;
        }
    }
}
