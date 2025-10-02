using UnityEngine;
using SaintsField;

namespace Map.Map_manager
{
    public class MapManager : MonoBehaviour
    {
        public SaintsDictionary<MapUnlockRequirementSO, CompositeCollider2D> mapRequirementToRegion;

        public void RequirementCompleted(Component component, object requirement)
        {
            MapUnlockRequirementSO mapRequirement = (MapUnlockRequirementSO)((object[])requirement)[0];
            CompositeCollider2D collider = mapRequirementToRegion[mapRequirement];
            collider.isTrigger = true;
        }
    }
}
