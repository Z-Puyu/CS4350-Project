using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace Weapon_system_page {
    public class WeaponSystemUIManager : MonoBehaviour
    {
        public ComponentInfoPanel componentPanelPrefab;
        private List<GameObject> allSpawnedPanels;
        private int currentIndex = 0;
        public SaintsDictionary<int, GameObject> mappedIndexToWeaponDetailPrefab;

        public void SpawnComponentInfo()
        {

        }

        public void SetWeaponDetail(int index)
        {
            this.mappedIndexToWeaponDetailPrefab[this.currentIndex].SetActive(false);
            this.currentIndex = index;
            this.mappedIndexToWeaponDetailPrefab[this.currentIndex].SetActive(true);
        }
    
    

    }
}
