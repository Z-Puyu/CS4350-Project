using TMPro;
using UnityEngine;

namespace Player_related.Player_notebook_UI.Objectives
{
    public class UnclearObjectiveText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI counterText;

        public void UpdateCounterText(Component component, object counter)
        {
            int value = (int)((object[]) counter)[0];
            counterText.text = value.ToString();
        }
    }   
}
