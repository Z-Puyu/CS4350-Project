using Events;
using Skill_tree_related.Skills;
using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    [SerializeField] private Skill skillScriptableObject;
    [SerializeField] private CrossObjectEventWithDataSO broadcastSkill;
    
    [SerializeField] private Image image;
    [SerializeField] private Sprite lockedIconBg;
    [SerializeField] private Sprite unlockedIconBg;

    void Start()
    {
        image = GetComponent<Image>();
    }

    public void InitialiseIcon(bool isFilled)
    {
        if (isFilled)
        {
            image.sprite = unlockedIconBg;
        }
        else
        {
            image.sprite = lockedIconBg;
        }
    }

    public void BroadcastSkill()
    {
        broadcastSkill.TriggerEvent(this, skillScriptableObject);
    }
}
