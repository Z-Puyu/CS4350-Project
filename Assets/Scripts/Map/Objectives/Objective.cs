using UnityEngine;

namespace Map.Objectives
{
    public abstract class Objective : MonoBehaviour
    {
        public string title;
        public string description;

        public void AddProgress() {}
    }
}