using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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
        Vector2 mousePos = transform.InverseTransformPoint(Mouse.current.position.ReadValue());
        hoverOverScrollView.transform.localPosition = new Vector2(mousePos.x + 100, mousePos.y - 150);
        hoverOverScrollView.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        hoverOverScrollView.SetActive(false);
    }
}
