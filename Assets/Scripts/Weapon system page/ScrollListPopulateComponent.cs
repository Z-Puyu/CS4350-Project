using System.Collections.Generic;
using WeaponsSystem.Runtime.WeaponComponents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Weapon_system_page {
    public class ScrollListPopulateComponent : MonoBehaviour
    {
        public GameObject itemPrefab;        // prefab to instantiate
        public Transform contentParent;       // the �Content� transform under ScrollView
        void Start()
        {
            this.PopulateList();
        }

        void PopulateList()
        {
            var weaponComponents = ComponentDatabase.All;
            foreach (var data in weaponComponents)
            {
                GameObject go = Object.Instantiate(this.itemPrefab, this.contentParent);
                go.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>().SetText(data.ItemName);
                go.transform.Find("PriceText")?.GetComponent<TextMeshProUGUI>().SetText($"${data.price}");
                go.transform.Find("RarityText")?.GetComponent<TextMeshProUGUI>().SetText(data.rarity.ToString());
                var icon = go.transform.Find("Icon")?.GetComponent<Image>();
                if (icon != null && data.icon != null)
                    icon.sprite = data.icon;
                
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
