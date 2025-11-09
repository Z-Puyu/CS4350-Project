using GameplayAbilities.Runtime.Abilities;
using Player_related.Player_exp;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    [SerializeField] private PlayerExp PlayerExp;
    [SerializeField] private AbilitySystem abilitySystem;
    
    public void UnlockSkill()
    {
        PlayerExp.MinusPoint(abilitySystem);
    }
    
}
