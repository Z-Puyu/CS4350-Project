using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Skill_tree_related.Skill_tree_UI
{
    public class SkillLineSlider : MonoBehaviour
    {
        public Slider slider;

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
            if (slider.value >= 1)
            {
                callback();
            }
            else
            {
                slider.value += 0.05f;
                yield return new WaitForSeconds(0.05f);
                FillLineAcrossTme(callback);   
            }
        }
        
    }
   
}