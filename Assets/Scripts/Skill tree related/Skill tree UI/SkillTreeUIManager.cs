using UnityEngine;
using UnityEngine.UI;

public class SkillTreeUIManager : MonoBehaviour
{
    public Slider combatExpBar;
    public Slider farmingExpBar;
    [SerializeField]
    private int currentCombatExp;
    [SerializeField]
    private int currentFarmingExp;
    [SerializeField]
    private int maxCombatExp;
    [SerializeField]
    private int maxFarmingExp;

    void Start()
    {
        UpdateExp();
    }

    // void Update()
    // {
    //     UpdateExp();
    // }

    public void UpdateExp()
    {
        combatExpBar.value = (((float)currentCombatExp) / ((float)maxCombatExp)) * 0.5f;
        farmingExpBar.value = (((float)currentFarmingExp) / ((float)maxFarmingExp)) * 0.5f;
    }


}
