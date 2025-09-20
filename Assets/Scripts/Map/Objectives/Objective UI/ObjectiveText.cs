using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Map.Objectives.Objective_UI
{
    public class ObjectiveText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI objectiveText;
        [SerializeField] private GameObject backgroundImage;
        
        public void SetText(string text, bool isCompleted)
        {
            objectiveText.text = text;
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