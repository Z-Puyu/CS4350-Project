using System.Collections;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Runtime.Attributes {
    [CreateAssetMenu(fileName = "Attribute Table", menuName = "Gameplay Abilities/Attribute Table")]
    public class AttributeTable : ScriptableObject, IEnumerable<KeyValuePair<AttributeType, int>> {
        [field: SerializeField, SaintsDictionary("Attribute Type", "Initial Value")]
        private SaintsDictionary<AttributeType, int> Attributes { get; set; } =
            new SaintsDictionary<AttributeType, int>();

        public IEnumerator<KeyValuePair<AttributeType, int>> GetEnumerator() {
            return this.Attributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
