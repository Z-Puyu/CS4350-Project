using System.Collections.Generic;
using Game.Enemies;
using UnityEngine;
using Map.Objectives.Objective_UI;

namespace Map.Objectives

{
	[CreateAssetMenu(fileName = "Kill enemies objective", menuName = "Objectives/Kill enemies objective")]
    public class KillEnemiesObjective : Objective
    {
        public EnemyData enemyToKill;
		public int enemyToKillAmount;
		[SerializeField] private int currentKillCounter;

		void OnEnable()
		{
			currentKillCounter = 0;
		}
		
		public override void SetText(ObjectiveText objectiveText)
		{
			objectiveText.SetText(title, currentKillCounter, enemyToKillAmount, IsComplete());
		}

		public void AddProgress(ObjectiveManager objectiveManager, List<KillEnemiesObjective> allKillEnemiesObjectives, EnemyData enemyData) {
			if (enemyData == enemyToKill) 
			{
				currentKillCounter += 1;
				if (currentKillCounter >= enemyToKillAmount)
				{
					allKillEnemiesObjectives.Remove(this);
					objectiveManager.HandleDeletion(this);
				}
			}	
 		}

	    public override bool IsComplete()
	    {
		    return currentKillCounter >= enemyToKillAmount;
	    }
    }
}