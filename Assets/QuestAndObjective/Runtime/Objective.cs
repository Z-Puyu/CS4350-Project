using System;

namespace QuestAndObjective.Runtime {
    [Serializable]
    public abstract class Objective {
        public abstract bool IsCompleted { get; }

        public abstract void Update<S>(S @event) where S : struct, IObjectiveStateUpdateEvent;
    }
    
    public abstract class Objective<E> : Objective where E : struct, IObjectiveStateUpdateEvent {
        public abstract void Update(E @event);
        
        public sealed override void Update<S>(S @event) {
            if (!this.IsCompleted && @event is E e) {
                this.Update(e);
            }
        }
    }
}
