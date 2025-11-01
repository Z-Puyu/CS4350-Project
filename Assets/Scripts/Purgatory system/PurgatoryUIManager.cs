using System;
using Map.Wave_manager;
using UnityEngine;
using UnityEngine.UI;

namespace Purgatory_system {
public class PurgatoryUIManager : MonoBehaviour
{
    [SerializeField] private GameObject purgatoryCanvas;
    [SerializeField] private Slider defeatedGhostGauge;

    private void Start()
    {
        HideCanvas();
    }

    public void ResetGauge()
    {
        UpdateGauge(0);
    }

    public void UpdateGauge(float newValue)
    {
        defeatedGhostGauge.value = newValue;
    }

    public void ShowCanvas()
    {
        purgatoryCanvas.gameObject.SetActive(true);
    }

    public void HideCanvas()
    {
        purgatoryCanvas.gameObject.SetActive(false);
    }
    
} 
}
