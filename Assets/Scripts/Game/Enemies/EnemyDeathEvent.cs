using UnityEngine;

namespace Game.Enemies {
    public readonly struct EnemyDeathEvent {
        public EnemyData DeadEnemy { get; }
        public GameObject Killer { get; }

        public EnemyDeathEvent(EnemyData deadEnemy, GameObject killer) {
            this.DeadEnemy = deadEnemy;
            this.Killer = killer;
        }
    }
}
