using System;
using SaintsField;
using UnityEngine;

namespace QuestAndObjective.Runtime {
    [Serializable]
    public abstract class Objective {
        protected internal abstract string Name { get; }

        public abstract void Initialise();
        
        public abstract bool IsCompleted(QuestVariableContainer variables);

        public abstract bool Advance(QuestVariableContainer variables);
    }
}
