using UnityEngine;
using TMPro;

namespace Map.Objectives.Objective_UI
{
    public class ObjectiveText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI objectiveText;
        [SerializeField] private GameObject backgroundImage;
        
        public void SetText(string text, int currentCount, int requiredCOunt, bool isCompleted)
        {
            objectiveText.text = text + " (" + currentCount + "/" + requiredCOunt + ")";
            if (isCompleted)
            {
                objectiveText.color = Color.red;
                objectiveText.fontStyle = FontStyles.Strikethrough;
                backgroundImage.SetActive(true);
            }
            else
            {
                objectiveText.color = Color.black;
                backgroundImage.SetActive(false);
            }
        }
    }
}