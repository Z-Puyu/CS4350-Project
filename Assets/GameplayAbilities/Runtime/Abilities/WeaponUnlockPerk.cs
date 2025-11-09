using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities
{
    [CreateAssetMenu(fileName = "New Weapon Perk", menuName = "Weapon Perk")]
    public class WeaponUnlockPerk : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public int skillPointsToUnlock{ get; private set; }
        public string weaponName;
        public int weaponIndex;
        public Sprite sprite;
    }
}