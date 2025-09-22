using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DataStructuresForUnity.Runtime.GeneralUtils;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace LocalisationMappings.Runtime {
    public abstract class LocalisationTextFormatter : ScriptableObject {
        private protected static readonly Regex KeywordRegex = new Regex("@([A-Za-z][A-Za-z0-9]*(?:-[A-Za-z][A-Za-z0-9]*)*)"); 
    }
    
    public abstract class LocalisationTextFormatter<T> : LocalisationTextFormatter {
        [field: SerializeField, TextArea] private string Text { get; set; }
        [field: SerializeField, Table(hideAddButton: true, hideRemoveButton: true)]
        private List<KeywordMapping> Keywords { get; set; } = new List<KeywordMapping>();

        private Dictionary<string, Func<T, string>> KeywordReplacers { get; set; } =
            new Dictionary<string, Func<T, string>>(); 
        
        [Button]
        private void CollectKeywords() {
            HashSet<string> keywords = LocalisationTextFormatter.KeywordRegex
                                                                .Matches(this.Text)
                                                                .Select(match => match.Value)
                                                                .ToHashSet();
            this.Keywords.RemoveAll(keyword => !keywords.Contains(keyword.Keyword));
            keywords.ExceptWith(this.Keywords.Select(keyword => keyword.Keyword));
            foreach (string keyword in keywords) {
                this.Keywords.Add(KeywordMapping.Of(keyword, typeof(T)));
            }
        }

        private string Fetch(string member, T @object) {
            if (this.KeywordReplacers.TryGetValue(member, out Func<T, string> producer)) {
                return producer.Invoke(@object);
            }
            
            ExpressionTree.Getter<T, object> accessor = ExpressionTree.PropertyAccessor<T, object>(member);
            this.KeywordReplacers.Add(member, obj => accessor.Invoke(obj).ToString());
            return accessor.Invoke(@object).ToString();
        }

        public string Format(T @object) {
            return this.Keywords.Aggregate(this.Text, replace);

            string replace(string text, KeywordMapping mapping) {
                return text.Replace(mapping.Keyword, this.Fetch(mapping.Value, @object));
            }
        }
    }
}
