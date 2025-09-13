using UnityEngine;

namespace Map.Objectives
{
    public abstract class Objective : ScriptableObject
    {
        public string title;
        public string description;

        public void AddProgress() {}
    }
}