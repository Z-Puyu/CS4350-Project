using Events;
using UnityEngine;

namespace Game.SFX_trigger
{
    public class SFXTrigger : MonoBehaviour
    {
        [SerializeField] private CrossObjectEventWithDataSO broadcastSFX;

        public void BroadcastSFX(AudioClip audioClip)
        {
            broadcastSFX.TriggerEvent(this, audioClip);
        }
        
    }
}