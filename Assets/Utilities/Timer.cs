using UnityEngine;
using System;
namespace Utilities {
    public class Timer {
        public event Action OnTimerFinished;
        
        private float startTime;
        private readonly float duration;
        private float targetTime;

        private bool isActive;
        
        public Timer(float duration) {
            this.duration = duration;
        }

        public void Start() {
            this.targetTime = Time.time + this.duration;
            this.startTime = Time.time;
            this.isActive = true;
        }
        
        public void Stop() {
            this.isActive = false;
        }
        
        public void Tick() {
            if (!this.isActive) return;
            if (Time.time <= this.targetTime) {
                return;
            }
            
            this.isActive = false;
            this.OnTimerFinished?.Invoke();
        }
    }
}
