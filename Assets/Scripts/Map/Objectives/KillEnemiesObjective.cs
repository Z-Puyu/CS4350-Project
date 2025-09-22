using UnityEngine;
using Map.Objectives.Objective_UI;

namespace Map.Objectives

{
	[CreateAssetMenu(fileName = "Kill enemies objective", menuName = "Objectives/Kill enemies objective")]
    public class KillEnemiesObjective : Objective
    {
        //private EnemySO enemySO;
		public int enemyToKillAmount;
		private int currentKillCounter;

		public void AddProgress(ObjectiveManager objectiveManager) {
			currentKillCounter += 1;
			if (currentKillCounter == enemyToKillAmount)
			{
				objectiveManager.HandleDeletion(this);
			}
 		}

	    public override bool IsComplete()
	    {
		    return currentKillCounter >= enemyToKillAmount;
	    }
    }
}