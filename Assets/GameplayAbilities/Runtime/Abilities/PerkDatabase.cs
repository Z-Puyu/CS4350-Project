using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.GeneralUtils;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    public class PerkDatabase : Singleton<PerkDatabase> {
        [field: SerializeField, ResourceFolder] 
        private string PerkDataFolder { get; set; }
        
        [field: SerializeField, ResourceFolder] 
        private string AbilityDataFolder { get; set; }

        private List<string> ObtainableAbilities { get; } = new List<string>();
        private Dictionary<string, Ability> Abilities { get; } = new Dictionary<string, Ability>();
        private Dictionary<Perk, List<Perk>> Perks { get; } = new Dictionary<Perk, List<Perk>>();
        
        private void LoadAbilities() {
            foreach (Ability data in Resources.LoadAll<Ability>(this.AbilityDataFolder)) {
                if (!this.Abilities.TryAdd(data.Id, data)) {
                    Debug.LogError($"Duplicate ability ID {data.Id}!");
                } else if (data.IsObtainable) {
                    this.ObtainableAbilities.Add(data.Id);
                }
            }
            
            this.ObtainableAbilities.Sort();
        }

        private void LoadPerks() {
            Perk[] perks = Resources.LoadAll<Perk>(this.PerkDataFolder);
            foreach (Perk data in perks) {
                this.Perks.Add(data, new List<Perk>());
            }

            foreach (Perk data in perks) {
                foreach (Perk prerequisite in data.Prerequisites) {
                    this.Perks[prerequisite].Add(data);
                }
            }
        }

        protected override void Awake() {
            base.Awake();
            this.LoadPerks();
            this.LoadAbilities();
            
            Debug.Log($"Loaded {this.Perks.Count} perks from {this.PerkDataFolder}");
            Debug.Log($"Loaded {this.Abilities.Count} abilities from {this.AbilityDataFolder}");
        }

        public static IEnumerable<Perk> GetChildren(Perk p) {
            return Singleton<PerkDatabase>.Instance.Perks.TryGetValue(p, out List<Perk> children)
                    ? children
                    : Enumerable.Empty<Perk>();
        }
        
        public static Ability GetAbility(string id) {
            return Singleton<PerkDatabase>.Instance.Abilities.GetValueOrDefault(id);
        }

        public static IEnumerable<string> GetAllAbilityIds() {
            return Resources.LoadAll<Ability>(Singleton<PerkDatabase>.Instance.AbilityDataFolder)
                            .Select(a => a.Id);
        }
        
        public static IEnumerable<string> GetAllObtainableAbilityIds() {
            return Resources.LoadAll<Ability>(Singleton<PerkDatabase>.Instance.AbilityDataFolder)
                            .Where(a => a.IsObtainable)
                            .Select(a => a.Id);
        }
    }
}
