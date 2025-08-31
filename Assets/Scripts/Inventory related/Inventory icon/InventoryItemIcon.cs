using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryItemIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public TextMeshProUGUI bonusText;
    public GameObject hoverOverScrollView;

    void Start()
    {
        hoverOverScrollView.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        hoverOverScrollView.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        hoverOverScrollView.SetActive(false);
    }
}
