using System;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace QuestAndObjective.Runtime {
    [Serializable]
    public abstract class Objective {
        protected internal abstract string Name { get; }

        public abstract void Initialise(IQuestProgressProvider provider);
        
        public abstract bool IsCompleted(IQuestProgressProvider provider);

        public abstract bool Advance(IQuestProgressProvider provider);
    }
}
