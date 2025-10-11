using Game.SFX_trigger;
using UnityEngine;

namespace Game.enemies
{
    public class EnemySFXTrigger : MonoBehaviour
    {
        [SerializeField] private SFXTrigger SfxTrigger;
        [SerializeField] private AudioClip attackSFX;
        [SerializeField] private AudioClip damagedSFX;
        [SerializeField] private AudioClip dieSFX;
    
        public void PlayDamagedSFX()
        {
            SfxTrigger.BroadcastSFX(damagedSFX);
        }
        
        public void PlayAttackSFX()
        {
            SfxTrigger.BroadcastSFX(attackSFX);
        }
        
        public void PlayDieSFX()
        {
            SfxTrigger.BroadcastSFX(dieSFX);
        }
    }   
}
