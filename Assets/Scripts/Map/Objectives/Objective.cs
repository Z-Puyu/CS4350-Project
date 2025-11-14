using UnityEngine;
using Map.Objectives.Objective_UI;

namespace Map.Objectives
{
    public abstract class Objective : ScriptableObject
    {
        public string title;
        public string description;

        public void AddProgress() {}

        public abstract void SetText(ObjectiveText objectiveText);

        public abstract bool IsComplete();
    }
}