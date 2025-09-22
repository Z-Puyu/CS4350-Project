using System.Collections.Generic;
using UnityEngine;

public class ScrollListPopulatorWeapons : MonoBehaviour
{
    public GameObject itemPrefab;        // prefab to instantiate
    public Transform contentParent;       // the “Content” transform under ScrollView
    public List<string> itemList;   // the list of items you have
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PopulateList();
    }

    void PopulateList()
    {
        foreach (var data in itemList)
        {
            GameObject go = Instantiate(itemPrefab, contentParent);
            go.GetComponentInChildren<UnityEngine.UI.Text>().text = data;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
