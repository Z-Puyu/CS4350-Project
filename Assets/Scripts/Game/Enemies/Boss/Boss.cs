using System;
using Events;
using UnityEngine;

namespace Game.Enemies
{
    public class Boss : Enemy
    {
        public CrossObjectEventWithDataSO broadcastMessageOnSpawn;
        
        private void OnEnable()
        {
            broadcastMessageOnSpawn.TriggerEvent(this, Data.messageForPlayerOnSpawn);
        }
    }
}
