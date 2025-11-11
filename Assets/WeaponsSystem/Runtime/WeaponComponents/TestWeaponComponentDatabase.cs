using UnityEngine;

namespace WeaponsSystem.Runtime.WeaponComponents {
    public class TestWeaponComponentDatabase : MonoBehaviour {
        void Start() {
            Debug.Log("Testing ComponentDatabase...");

            // Force load
            var allComponents = ComponentDatabase.All;

            foreach (var comp in allComponents) {
                Debug.Log($"Loaded Component: {comp.ItemName} | Price: {comp.price} | Rarity: {comp.rarity}");
                foreach(var mat in comp.craftingMaterials)
                {
                    Debug.Log($"Material cost: {mat.material} | Amount: {mat.amount}");
                }
            }

            // Example: Try to get one directly by name
            if (ComponentDatabase.TryGet("FireComponent", out var fireComp)) {
                Debug.Log($"Successfully retrieved FireComponent! Name: {fireComp.ItemName}");
            } else {
                Debug.LogWarning("Could not find FireComponent!");
            }
        }
    }
}
