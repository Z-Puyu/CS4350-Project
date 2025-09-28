using System.Collections.Generic;
using DataStructuresForUnity.Runtime.GeneralUtils;
using Game.Map;
using GameplayAbilities.Runtime.HealthSystem;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using WeaponsSystem;

namespace Game.Player {
    [DisallowMultipleComponent, RequireComponent(typeof(PlayerController))]
    public sealed class PlayerAfterlife : MonoBehaviour {
        private PlayerController Player { get; set; }
        [field: SerializeField, Required] private Health Health { get; set; }
        [field: SerializeField, Required] private Weaponry Weaponry { get; set; }
        private Vector3Int RespawnPoint { get; set; }

        private void Awake() {
            this.Player = this.GetComponent<PlayerController>();
        }

        private void Start() {
            this.Player.OnDeath += this.HandlePlayerDeath;
        }

        private void HandlePlayerDeath() {
            if (GameWorldManager.IsInPurgatory) {
                return;
            }
            
            this.RespawnPoint = GameWorldManager.Main.WorldToCell(this.transform.position);
            GameWorldManager.Purgatory.gameObject.SetActive(true);
            ((IMap)GameWorldManager.Purgatory).PlaceObjectAtOrigin(this.gameObject);
            List<int> weapons = new List<int>();
            for (int i = 0; i < this.Weaponry.Size; i += 1) {
                weapons.Add(i);
            }
            
            weapons.Shuffle();
            for (int i = 0; i < weapons.Count - 1; i += 1) {
                this.Weaponry.Lock(weapons[i]);
            }
            
            this.Weaponry.Switch(weapons[^1]);
            this.Health.Refill();
        }
        
        [Button]
        public void Resurrect() {
            GameWorldManager.Main.PlaceObject(this.gameObject, this.RespawnPoint);
            GameWorldManager.Purgatory.gameObject.SetActive(false);
            this.Weaponry.UnlockAll();
            this.Health.Refill();
        }
    }
}
