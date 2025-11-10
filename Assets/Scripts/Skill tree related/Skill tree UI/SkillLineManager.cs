using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill_tree_related.Skill_tree_UI
{
    public class SkillLineManager : MonoBehaviour
    {
        [SerializeField] private List<SkillIcon> skillIconsManaged = new List<SkillIcon>();
        [SerializeField] private List<SkillLineSlider> skillLinesManaged = new List<SkillLineSlider>();
        private int level;

        void Start()
        {
            GetComponentsInChildren<SkillIcon>(skillIconsManaged);
            GetComponentsInChildren<SkillLineSlider>(skillLinesManaged);
        }

        public void InitialiseLine()
        {
            if (skillIconsManaged == null || skillIconsManaged.Count == 0) return;

            if (level == 0)
            {
                skillIconsManaged[0].SetCanUnlock();
            }

            for (int i = 0; i < skillIconsManaged.Count; i++)
            {
                bool isUnlocked = i < level;
                skillIconsManaged[i].InitialiseIcon(isUnlocked);

                if (i < skillLinesManaged.Count)
                {
                    skillLinesManaged[i].InitialiseLine(isUnlocked);
                }
            }
        }

        public void LevelUp(SkillIcon skillIcon)
        {
            for (int i = 0; i < skillIconsManaged.Count; i++)
            {
                if (skillIconsManaged[i] == skillIcon)
                {
                    if (i + 1 < skillIconsManaged.Count)
                        skillIconsManaged[i + 1].SetCanUnlock();

                    skillIcon.UnlockSkill();
                    level++;

                    if (i < skillLinesManaged.Count)
                        skillLinesManaged[i].FillLine(() => skillIconsManaged[i].InitialiseIcon(true));

                    return;
                }
            }
        }
        
    }
}