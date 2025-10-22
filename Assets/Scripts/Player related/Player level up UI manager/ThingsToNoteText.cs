using System.Collections;
using TMPro;
using UnityEngine;

namespace Player_related.Things_to_note_text
{
    public class ThingsToNoteText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        
        public void SetText(string text, int time)
        {
            this.text.text = text;
            StartCoroutine(StartCountdown(time));
        }

        IEnumerator StartCountdown(int time)
        {
            yield return new WaitForSeconds(time);
            StartCoroutine(FadeText(1.0f));
        }

        IEnumerator FadeText(float fadeTime)
        {
            float time = fadeTime;
            while (time > 0)
            {
                time -= 0.1f;
                text.color = new Color(1, 1, 1, time);
                yield return new WaitForSeconds(0.1f);
            }
            Destroy(this);
        }
        
    }
}