using System;
using System.Collections.Generic;
using SaintsField;
using ModularItemsAndInventory.Runtime.Items;
using ModularItemsAndInventory.Runtime.Inventory;
using UnityEngine;
using WeaponsSystem.Runtime.WeaponComponents;
// using System.Diagnostics;

namespace Shop.Runtime
{
    /// <summary>
    /// A runtime shop inventory that stores which items the shop sells.
    /// Attach this script to an empty GameObject in your scene or prefab.
    /// </summary>
    public class BlacksmithInventory : MonoBehaviour
    {
        // [System.Serializable]
        public bool TryGetItem(ItemKey key, out WeaponComponent foundItem)
        {
            foundItem = weaponsForSale.Find(w => w.WeaponKey.Equals(key));
            return foundItem != null;
        }
        [Header("Shop Items")]
        [SerializeField] private List<WeaponComponent> weaponsForSale = new List<WeaponComponent>();

        public IReadOnlyList<WeaponComponent> WeaponsForSale => weaponsForSale;

        public void Use(List<WeaponComponent> weapons)
        {
            weaponsForSale.Clear();
            weaponsForSale.AddRange(weapons);
        }
        public bool Add(WeaponComponent weapon)
        {
            if (weapon == null)
            {
                Debug.LogWarning("Cannot add null weapon.");
                return false;
            }
            if (weaponsForSale.Exists(w => w.WeaponKey.Equals(weapon.WeaponKey)))
            {
                Debug.LogWarning($"Weapon {weapon.WeaponKey} already in shop inventory.");
                return false;
            }
            weaponsForSale.Add(weapon);
            return true;
        }

        public bool Remove(ItemKey key)
        {
            var weapon = weaponsForSale.Find(w => w.WeaponKey.Equals(key));
            if (weapon == null)
            {
                Debug.LogWarning($"Weapon {weapon.Id} not found in shop inventory.");
                return false;
            }
            weaponsForSale.Remove(weapon);
            return true;
        }

        public int GetPrice(ItemKey key)
        {
            var weapon = weaponsForSale.Find(w => w.WeaponKey.Equals(key));
            return weapon != null ? weapon.price : -1;
        }

        public bool TryPurchase(ItemKey key, out int price)
        {
            var weapon = weaponsForSale.Find(w => w.WeaponKey.Equals(key));
            if (weapon == null)
            {
                price = 0;
                return false;
            }

            // Since stock is always 1, removing it means it's sold out.
            weaponsForSale.Remove(weapon);
            price = weapon.price;
            return true;
        }
        public bool HasWeapon(ItemKey key)
        {
            return weaponsForSale.Exists(w => w.WeaponKey.Equals(key));
        }
    }
}

