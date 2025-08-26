using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComponentInfoPanel : MonoBehaviour
{
    public TextMeshProUGUI componentName;
    public TextMeshProUGUI buffDescription;
    public GameObject buffPanel;
    public int minHeightWithClosedBuffPanel;
    public int minHeightWithActiveBuffPanel;
    private LayoutElement layoutElement;

    void Start()
    {
        layoutElement = GetComponent<LayoutElement>();
        ChangePreferredHeight();
    }

    public void PopulateComponentPanelInfo()
    {

    }

    public void ToggleBuffPanel()
    {
        buffPanel.SetActive(!buffPanel.activeSelf);
        ChangePreferredHeight();
    }

    private void ChangePreferredHeight()
    {
        if (buffPanel.activeSelf)
        {
            layoutElement.minHeight = minHeightWithActiveBuffPanel;
        }
        else
        {
            layoutElement.minHeight = minHeightWithClosedBuffPanel;
        }
    }
}
