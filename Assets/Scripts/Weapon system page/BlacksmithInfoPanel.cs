using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Weapon_system_page {
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
            this.buffPanel.SetActive(false);
            this.layoutElement = this.GetComponent<LayoutElement>();
            this.ChangePreferredHeight();
        }

        public void PopulateComponentPanelInfo()
        {

        }

        public void ToggleBuffPanel()
        {
            this.animator = this.buffDescription.GetComponent<Animator>();
            if(this.animator != null)
            {
                bool isOpen = this.animator.GetBool("Openinfo");

                this.animator.SetBool("Openinfo", !isOpen);

                // Start smooth height animation
                float duration = 0.4f; // Match your Animator transition duration
                this.StartCoroutine(this.AnimateMinHeight(
                    this.layoutElement.minHeight,
                    !isOpen ? this.minHeightWithActiveBuffPanel : this.minHeightWithClosedBuffPanel,
                    duration));
            }
            this.buffPanel.SetActive(!this.buffPanel.activeSelf);
            this.ChangePreferredHeight();
        }

        private IEnumerator AnimateMinHeight(float startHeight, float targetHeight, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                this.layoutElement.minHeight = Mathf.Lerp(startHeight, targetHeight, t);
                yield return null;
            }
            this.layoutElement.minHeight = targetHeight; // Ensure final value is exact
        }

        private void ChangePreferredHeight()
        {
            if (this.buffPanel.activeSelf)
            {
                this.layoutElement.minHeight = this.minHeightWithActiveBuffPanel;
            }
            else
            {
                this.layoutElement.minHeight = this.minHeightWithClosedBuffPanel;
            }

        }
    }
}