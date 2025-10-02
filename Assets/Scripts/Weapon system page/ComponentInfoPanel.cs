using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Weapon_system_page {
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
            this.layoutElement = this.GetComponent<LayoutElement>();
            this.ChangePreferredHeight();
        }

        public void PopulateComponentPanelInfo()
        {

        }

        public void ToggleBuffPanel()
        {
            this.buffPanel.SetActive(!this.buffPanel.activeSelf);
            this.ChangePreferredHeight();
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
