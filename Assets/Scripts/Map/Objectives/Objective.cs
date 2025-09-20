using UnityEngine;
using SaintsField;
using System.Collections.Generic;
using Map.Objectives.Objective_UI;

namespace Map.Objectives
{
    public abstract class Objective : ScriptableObject
    {
        public string title;
        public string description;

        public void AddProgress() {}

        public void SetText(ObjectiveText objectiveText)
        {
            objectiveText.SetText(title, IsComplete());
        }

        public abstract bool IsComplete();
    }
}