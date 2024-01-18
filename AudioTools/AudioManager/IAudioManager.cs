using FMODUnity;

namespace SNShien.Common.AudioTools
{
    public interface IAudioManager
    {
        void InitAudioCollection();
        void SetParam(string audioParamKey, float paramValue);
        void Play(string audioKey, int trackIndex = 0);
        void Play(EventReference eventReference, int trackIndex = 0);
        void Stop(int trackIndex = 0, bool stopImmediately = false);
        void PrintAudioKeys();
        void PlayOneShot(string audioKey);
        void PlayOneShot(EventReference eventReference);
    }
}