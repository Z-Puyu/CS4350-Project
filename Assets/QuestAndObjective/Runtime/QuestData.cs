using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace QuestAndObjective.Runtime {
    [CreateAssetMenu(fileName = "New Quest Data", menuName = "Quest and Objective/Quest Data")]
    public sealed class QuestData : ScriptableObject {
        [field: SerializeField] internal string Id { get; private set; }
        [field: SerializeField] private string Name { get; set; }
        [field: SerializeField, TextArea] private string Description { get; set; }
        [field: SerializeField] internal List<QuestStage> Stages { get; private set; }
    }
}
