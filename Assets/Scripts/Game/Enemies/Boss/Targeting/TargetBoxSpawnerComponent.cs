using UnityEngine;

namespace Game.Enemies
{
    public class TargetBoxSpawnerComponent : MonoBehaviour
    {
        public TargetBox targetBoxPrefab;
        public TargetingType targetingType;
        
        public void SpawnTargetBox()
        {
            targetingType.SpawnTarget(targetBoxPrefab);
        }
    }
}
