using System;

namespace QuestAndObjective.Runtime {
    [Serializable]
    public abstract class Objective {
        public abstract bool IsCompleted { get; }

        public abstract bool Advance<S>(S @event) where S : struct, IObjectiveStateUpdateEvent;
    }
    
    public abstract class Objective<E> : Objective where E : struct, IObjectiveStateUpdateEvent {
        protected abstract bool Advance(E @event);
        
        public sealed override bool Advance<S>(S @event) {
            if (!this.IsCompleted && @event is E e) {
                return this.Advance(e);
            }
            
            return false;
        }
    }
}
