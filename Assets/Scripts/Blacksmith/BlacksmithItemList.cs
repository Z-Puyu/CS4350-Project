using UnityEngine;

[CreateAssetMenu(fileName = "BlacksmithItemList", menuName = "Blacksmith/Blacksmith Item List")]
public class BlacksmithItemList : ScriptableObject
{
    [SerializeField] private List<WeaponComponent> _components;
}

[System.Serializable]
public struct BlacksmithInventoryItem
{
    public WeaponComponent ComponentData;
    public int Amount;
}
