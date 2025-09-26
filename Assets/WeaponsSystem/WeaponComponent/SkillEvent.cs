using UnityEngine;
using UnityEngine.Events;

namespace WeaponsSystem.WeaponComponent {
    [CreateAssetMenu(menuName = "Events/Skill Event")]
    public class SkillEvent : ScriptableObject {
        public UnityEvent<string> onSkillActivatable;

        public void Raise(string skillId) {
            Debug.Log($"Skill Event Activating skill {skillId}", this);
            this.onSkillActivatable?.Invoke(skillId);
        }
    }
}
