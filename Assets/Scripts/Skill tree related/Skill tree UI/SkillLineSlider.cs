using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Skill_tree_related.Skill_tree_UI
{
    public class SkillLineSlider : MonoBehaviour
    {
        public Slider slider;

        private void Start()
        {
            slider.value = 0;
        }

        public void InitialiseLine(bool isFilled)
        {
            if (isFilled)
            {
                slider.value = 1;
            }
            else
            {
                slider.value = 0;
            }
        }

        public void FillLine(Action callback)
        {
            StartCoroutine(FillLineAcrossTme(callback));
        }

        IEnumerator FillLineAcrossTme(Action callback)
        {
            while (true)
            {
                if (slider.value >= 1)
                {
                    callback();
                    break;
                }
                slider.value += 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
        }
        
    }
   
}