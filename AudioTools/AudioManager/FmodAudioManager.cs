using FMODUnity;

namespace SNShien.Common.AudioTools
{
    public class FmodAudioManager : IAudioManager
    {
        private readonly IAudioCollection audioCollection;

        public FmodAudioManager(IAudioCollection audioCollection)
        {
            this.audioCollection = audioCollection;
        }

        public void PlayOneShot(string audioKey)
        {
            RuntimeManager.PlayOneShot(audioCollection.GetEventReference(audioKey));
        }
    }
}