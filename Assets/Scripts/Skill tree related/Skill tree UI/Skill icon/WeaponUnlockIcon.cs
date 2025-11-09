using Events;
using GameplayAbilities.Runtime.Abilities;
using UnityEngine;

namespace Skill_tree_related.Skill_tree_UI
{
    public class WeaponUnlockIcon : SkillIcon
    {
        [SerializeField] private WeaponUnlockPerk skillScriptableObject;
        [SerializeField] private CrossObjectEventWithDataSO broadcastWeaponUnlockSkillIcon;
        
        public void BroadcastSkill()
        {
            this.broadcastWeaponUnlockSkillIcon.TriggerEvent(this, this.skillScriptableObject);
        }
        
        override public void SetSkillIcon()
        {
            spriteIcon.sprite = skillScriptableObject.sprite;
        }
    }
}