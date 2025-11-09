using Events;
using GameplayAbilities.Runtime.Abilities;
using UnityEngine;

namespace Skill_tree_related.Skill_tree_UI
{
    public class CombatStatsSKillIcon : SkillIcon
    {
        [SerializeField] private Perk skillScriptableObject;
        [SerializeField] private CrossObjectEventWithDataSO broadcastSkill;
        
        public void BroadcastSkill()
        {
            this.broadcastSkill.TriggerEvent(this, this.skillScriptableObject);
        }

        override public void SetSkillIcon()
        {
            spriteIcon.sprite = skillScriptableObject.sprite;
        }
    }
}
