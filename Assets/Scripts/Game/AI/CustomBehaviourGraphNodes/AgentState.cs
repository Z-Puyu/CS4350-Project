using Unity.Behavior;

namespace Game.AI.CustomBehaviourGraphNodes {
	[BlackboardEnum]
	public enum AgentState {
		Idle,
		Alert,
		Cautious,
		Aggressive
	}
}
