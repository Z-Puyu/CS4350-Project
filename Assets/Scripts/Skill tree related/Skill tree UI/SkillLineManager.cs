using System.Collections.Generic;
using UnityEngine;

namespace Skill_tree_related.Skill_tree_UI
{
    public class SkillLineManager : MonoBehaviour
    {
        [SerializeField] private List<SkillIcon> skillIconsManaged;
        [SerializeField] private List<SkillLineSlider> skillLinesManaged;

        public void InitialiseLine(int level)
        {
            for (int i = 0; i < skillIconsManaged.Count; i++)
            {
                if (i <= level - 1)
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

        public void LevelUp(int level)
        {
            skillLinesManaged[level - 1].FillLine(() => skillIconsManaged[level - 1].InitialiseIcon(true));
        }
        
    }
}