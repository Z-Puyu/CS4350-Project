using UnityEngine;

public abstract class ItemSlot : ISerializationCallbackReceiver
{
    [NonSerialized] protected InventoryItemData itemData;
    [SerializeField] protected int _itemID = -1;
    [SerializeField] protected int stackSize;
    public void OnBeforeSerialize()
    {

    }
    
    public void OnAfterDeserialize()
    {
        
    }
}
