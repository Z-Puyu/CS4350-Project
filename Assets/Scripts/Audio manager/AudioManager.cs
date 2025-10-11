using UnityEngine;

namespace Audio_manager
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource bgmAudioSource;
        [SerializeField] private AudioSource templateSoundFXAudioSource;
        
        public void PlayBgmAudio(Component component, object aC)
        {
            AudioClip clip = (AudioClip)((object[])aC)[0];
            bgmAudioSource.clip = clip;
            bgmAudioSource.Play();
        } 
        
        public void PlaySoundFXAudio(Component component, object aC)
        {
            AudioSource newTempAudioSource = Instantiate(templateSoundFXAudioSource);
            AudioClip clip = (AudioClip)((object[])aC)[0];
            newTempAudioSource.outputAudioMixerGroup = templateSoundFXAudioSource.outputAudioMixerGroup;
            newTempAudioSource.clip = clip;
            newTempAudioSource.Play();
            Destroy(newTempAudioSource, newTempAudioSource.clip.length);
        } 
    }
}
