#if CUSTOM_USING_FMOD
using FMOD.Studio;
using FMODUnity;

namespace SNShien.Common.AudioTools
{
    public interface IAudioManager
    {
        void InitCollectionFromProject();
        void InitCollectionFromSetting(IAudioCollection collectionSetting);
        void InitCollectionFromBundle(IAudioCollection collectionSetting, FmodAudioInitType initType);
        void SetParam(string audioParamKey, float paramValue);
        void Play(string audioKey, int trackIndex = 0);
        void Play(EventReference eventReference, int trackIndex = 0);
        void Stop(int trackIndex = 0, bool stopImmediately = false);
        FmodAudioCallbackSetting PlayWithCallback(string audioKey, int trackIndex = 0);
        void PlayOneShot(string audioKey);
        void PlayOneShot(EventReference eventReference);
    }
}
#endif