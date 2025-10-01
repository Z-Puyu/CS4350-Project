using System.Collections.Generic;
using UnityEngine;

namespace Weapon_system_page {
    public class ScrollListPopulatorWeapons : MonoBehaviour
    {
        public GameObject itemPrefab;        // prefab to instantiate
        public Transform contentParent;       // the �Content� transform under ScrollView
        public List<string> itemList;   // the list of items you have
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            this.PopulateList();
        }

        void PopulateList()
        {
            foreach (var data in this.itemList)
            {
                GameObject go = Object.Instantiate(this.itemPrefab, this.contentParent);
                go.GetComponentInChildren<UnityEngine.UI.Text>().text = data;
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
