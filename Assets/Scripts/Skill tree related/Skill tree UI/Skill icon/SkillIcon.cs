using Events;
using GameplayAbilities.Runtime.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace Skill_tree_related.Skill_tree_UI {
    public abstract class SkillIcon : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] protected Image spriteIcon;
        [SerializeField] private Sprite lockedIconBg;
        [SerializeField] private Sprite unlockedIconBg;

        //Is this skill icon unlocked?
        [SerializeField] private bool isUnlocked = false;
        
        //Can we unlock this skill?
        [SerializeField] private bool canUnlock = false;

        void Start()
        {
            image = GetComponent<Image>();
            spriteIcon = GetComponentInChildren<Image>();
            SetSkillIcon();
        }

        public void UnlockSkill()
        {
            isUnlocked = true;
            canUnlock = false;
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
        
        public bool CheckCanUnlock()
        {
            return canUnlock;
        }

        public void SetCanUnlock()
        {
            canUnlock = true;
        }

        public abstract void SetSkillIcon();
    }
}
