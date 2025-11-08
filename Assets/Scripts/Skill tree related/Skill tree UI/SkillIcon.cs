using Events;
using GameplayAbilities.Runtime.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace Skill_tree_related.Skill_tree_UI {
    public class SkillIcon : MonoBehaviour
    {
        [SerializeField] private Perk skillScriptableObject;
        [SerializeField] private CrossObjectEventWithDataSO broadcastSkill;
    
        [SerializeField] private Image image;
        [SerializeField] private Sprite lockedIconBg;
        [SerializeField] private Sprite unlockedIconBg;

        private bool isUnlocked;

        void Start()
        {
            isUnlocked = false;
            this.image = this.GetComponent<Image>();
        }

        public void UnlockSkill()
        {
            isUnlocked = true;
        }

        public void InitialiseIcon(bool isFilled)
        {
            if (isFilled)
            {
                this.image.sprite = this.unlockedIconBg;
            }
            else
            {
                this.image.sprite = this.lockedIconBg;
            }
        }

        public bool CheckIsUnlocked()
        {
            return isUnlocked;
        }

        public void BroadcastSkill()
        {
            this.broadcastSkill.TriggerEvent(this, this.skillScriptableObject);
        }
    }
}
