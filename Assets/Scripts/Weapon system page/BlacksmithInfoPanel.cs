using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BlacksmithInfoPanel : MonoBehaviour
{
    public TextMeshProUGUI componentName;
    public TextMeshProUGUI buffDescription;
    public GameObject buffPanel;
    public int minHeightWithClosedBuffPanel;
    public int minHeightWithActiveBuffPanel;
    private LayoutElement layoutElement;
    private Animator animator;

    void Start()
    {
        buffPanel.SetActive(false);
        layoutElement = GetComponent<LayoutElement>();
        ChangePreferredHeight();
    }

    public void PopulateComponentPanelInfo()
    {

    }

    public void ToggleBuffPanel()
    {
        animator = buffDescription.GetComponent<Animator>();
        if(animator != null)
        {
            bool isOpen = animator.GetBool("Openinfo");

            animator.SetBool("Openinfo", !isOpen);

            // Start smooth height animation
            float duration = 0.4f; // Match your Animator transition duration
            StartCoroutine(AnimateMinHeight(
                layoutElement.minHeight,
                !isOpen ? minHeightWithActiveBuffPanel : minHeightWithClosedBuffPanel,
                duration));
        }
        buffPanel.SetActive(!buffPanel.activeSelf);
        ChangePreferredHeight();
    }

    private IEnumerator AnimateMinHeight(float startHeight, float targetHeight, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            layoutElement.minHeight = Mathf.Lerp(startHeight, targetHeight, t);
            yield return null;
        }
        layoutElement.minHeight = targetHeight; // Ensure final value is exact
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