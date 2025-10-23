using System.Collections.Generic;
using System.Linq;
using DataStructuresForUnity.Runtime.Trie;
using DataStructuresForUnity.Runtime.Utilities;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Attributes {
    public sealed class AttributeDatabase : Singleton<AttributeDatabase> {
        [field: SerializeField, ResourceFolder] 
        private string AttributeDataFolder { get; set; }
        
        private TrieDictionary<string, char, AttributeType> AttributeTypes { get; } =
            new TrieDictionary<string, char, AttributeType>();

        protected override void Awake() {
            base.Awake();
            foreach (AttributeType type in Resources.LoadAll<AttributeType>(this.AttributeDataFolder)) {
                this.AttributeTypes.Add(type.Id, type);
            }
        }

        public static IEnumerable<AttributeType> FindAllSubtypes(string id) {
            return Singleton<AttributeDatabase>.Instance.AttributeTypes
                                               .PrefixSearchEntry(id)
                                               .Select(pair => pair.Value);
        }
    }
}
