using System.Collections.Generic;
using DataStructuresForUnity.Runtime.Utilities;
using SaintsField;
using TMPro;
using UnityEngine;

namespace Common {
    public sealed class OnScreenDebugger : Singleton<OnScreenDebugger> {
        Queue<string> Messages { get; set; } = new Queue<string>();

        [field: SerializeField, PostFieldRichLabel("s")]
        private float UpdateInterval { get; set; } = 5f;
        
        [field: SerializeField] private TextMeshProUGUI TextBox { get; set; }
        private float NextUpdateTime { get; set; }

        protected override void Awake() {
            base.Awake();
            this.TextBox = this.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start() {
            this.NextUpdateTime = Time.time + this.UpdateInterval;
            OnScreenDebugger.Log("Debugger started");
        }

        private void UpdateText() {
            if (this.Messages.TryDequeue(out string _)) {
                this.TextBox.text = string.Join('\n', this.Messages);
            }
        }

        public static void Log(string message) {
            OnScreenDebugger instance = Singleton<OnScreenDebugger>.Instance;
            instance.Messages.Enqueue(message);
            if (instance.Messages.Count == 1) {
                instance.NextUpdateTime = Time.time + instance.UpdateInterval;
            }

            if (instance.TextBox != null)
            {
                instance.TextBox.text = string.Join('\n', instance.Messages);   
            }
        }

        private void LateUpdate() {
            if (this.Messages.Count == 0 || Time.time < this.NextUpdateTime) {
                return;
            }

            this.NextUpdateTime = Time.time + this.UpdateInterval;
            this.UpdateText();
        }
    }
}
