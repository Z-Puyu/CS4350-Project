using System;
using System.Collections;
using UnityEngine;

public class LevelUpEffect : MonoBehaviour
{
    [SerializeField] private int timeToDisplay;
    
    void OnEnable()
    {
        StartCoroutine(Display());
    }

    IEnumerator Display()
    {
        gameObject.SetActive(true);
        int time = timeToDisplay;
        while (time > 0)
        {
            yield return new WaitForSeconds(1.0f);
            time -= 1;
        }
        gameObject.SetActive(false);
    }
}
