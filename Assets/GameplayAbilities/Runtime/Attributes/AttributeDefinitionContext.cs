using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayAbilities.Runtime.Attributes {
    [CreateAssetMenu(
        fileName = "Attribute Definition Context", 
        menuName = "Gameplay Abilities/Attributes/Attribute Definition Context"
    )]
    public class AttributeDefinitionContext : ScriptableObject, IEnumerable<AttributeType> {
        [field: SerializeField] private List<AttributeType> DefinedAttributes { get; set; } = new List<AttributeType>();
        
        public IEnumerator<AttributeType> GetEnumerator() {
            return this.DefinedAttributes.GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
