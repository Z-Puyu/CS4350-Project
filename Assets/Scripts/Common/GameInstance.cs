using DataStructuresForUnity.Runtime.GeneralUtils;
using Game.Player;
using SaintsField;
using UnityEngine;

namespace Common {
    public sealed class GameInstance : Singleton<GameInstance> {
        [field: SerializeField, Tag] private string PlayerTag { get; set; } = "Player";
        
        public static PlayerController Player { get; private set; }

        protected override void Awake() {
            base.Awake();
            GameInstance.Player = GameObject.FindGameObjectWithTag(this.PlayerTag).GetComponent<PlayerController>();
        }
    }
}
