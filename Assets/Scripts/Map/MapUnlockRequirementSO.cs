using UnityEngine;
using SaintsField;

namespace Map
{
    [CreateAssetMenu(fileName = "Map Unlock Requirement SO", menuName = "Map related/Map Unlock Requirement SO", order = 1)]
    public sealed class MapUnlockRequirementSO : ScriptableObject

    {
        public Sprite key;
        public SaintsDictionary<int, int> enemiesToKill;
    }
}