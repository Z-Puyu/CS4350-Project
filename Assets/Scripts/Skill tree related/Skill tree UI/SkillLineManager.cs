using System.Collections.Generic;
using UnityEngine;

namespace Skill_tree_related.Skill_tree_UI
{
    public class SkillLineManager : MonoBehaviour
    {
        [SerializeField] private List<SkillIcon> skillIconsManaged;
        [SerializeField] private List<SkillLineSlider> skillLinesManaged;
        private int level;

        public void InitialiseLine()
        {
            for (int i = 0; i < skillIconsManaged.Count; i++)
            {
                if (i < level)
                {
                    skillIconsManaged[i].InitialiseIcon(true);
                    skillLinesManaged[i].InitialiseLine(true);   
                }
                else
                {
                    skillIconsManaged[i].InitialiseIcon(false);
                    skillLinesManaged[i].InitialiseLine(false);
                }
            }    
        }

        public void LevelUp(SkillIcon skillIcon)
        {
            for (int i = 0; i < skillIconsManaged.Count; i++)
            {
                if (skillIconsManaged[i] == skillIcon)
                {
                    skillIcon.UnlockSkill();
                    level++;
                    skillLinesManaged[i].FillLine(() => skillIconsManaged[i].InitialiseIcon(true));
                    return;
                }
            }
        }
        
    }
}