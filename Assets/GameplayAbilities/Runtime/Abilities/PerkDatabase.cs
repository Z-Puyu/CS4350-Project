using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.GeneralUtils;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Abilities {
    public class PerkDatabase : Singleton<PerkDatabase> {
        [field: SerializeField, ResourceFolder] 
        public string PerkDataFolder { get; private set; }
        
        private Dictionary<Perk, List<Perk>> Perks { get; } = new Dictionary<Perk, List<Perk>>();
        
        protected override void Awake() {
            base.Awake();
            foreach (Perk data in Resources.LoadAll<Perk>(this.PerkDataFolder)) {
                if (!this.Perks.ContainsKey(data)) {
                    this.Perks.Add(data, new List<Perk>());
                }

                foreach (Perk prerequisite in data.Prerequisites) {
                    if (this.Perks.TryGetValue(prerequisite, out List<Perk> children)) {
                        children.Add(data);
                    } else {
                        this.Perks.Add(prerequisite, new List<Perk> { data });
                    }
                }
            }
            
            Debug.Log($"Loaded {this.Perks.Count} perks from {this.PerkDataFolder}", this);
        }

        public static IEnumerable<Perk> GetChildren(Perk p) {
            return Singleton<PerkDatabase>.Instance.Perks.TryGetValue(p, out List<Perk> children)
                    ? children
                    : Enumerable.Empty<Perk>();
        }
    }
}
