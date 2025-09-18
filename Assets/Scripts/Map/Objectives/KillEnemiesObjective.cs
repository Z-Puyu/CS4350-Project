namespace Map.Objectives
{
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