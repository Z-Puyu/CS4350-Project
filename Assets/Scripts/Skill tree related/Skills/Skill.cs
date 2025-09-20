using TMPro;
using UnityEngine;

namespace Skill_tree_related.Skills
{
    public abstract class Skill : ScriptableObject
    {
        [SerializeField] private string title;
        [SerializeField] private string description;

        public void SetInfo(TextMeshProUGUI titleText, TextMeshProUGUI descriptionText)
        {
            titleText.text = title;
            descriptionText.text = description;
        }
    }
}