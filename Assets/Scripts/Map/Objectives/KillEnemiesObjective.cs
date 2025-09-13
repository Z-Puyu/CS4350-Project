using UnityEngine;

namespace Map.Objectives
{
	[CreateAssetMenu(fileName = "Kill enemies objective", menuName = "Objectives/Kill enemies objective")]
    public class KillEnemiesObjective : Objective
    {
        //private EnemySO enemySO;
		public int enemyToKillAmount;
		private int currentKillCounter;

		public void AddProgress() {
			currentKillCounter += 1;
 		}
    }
}