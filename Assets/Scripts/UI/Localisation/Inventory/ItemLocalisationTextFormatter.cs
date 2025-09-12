using LocalisationMappings.Runtime;
using ModularItemsAndInventory.Runtime.Items;
using UnityEngine;

namespace UI.Localisation.Inventory {
    [CreateAssetMenu(fileName = "New Item Formatter", menuName = "Localisation/Item Formatter")]
    public class ItemLocalisationTextFormatter : LocalisationTextFormatter<Item> { }
}
