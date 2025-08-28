using UnityEngine;
using System.Collections.Generic;
using SaintsField;

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
        mappedIndexToWeaponDetailPrefab[currentIndex].SetActive(false);
        currentIndex = index;
        mappedIndexToWeaponDetailPrefab[currentIndex].SetActive(true);
    }
    
    

}
