namespace SNShien.Common.AudioTools 
{
    public interface IAudioManager
    {
        void PlayOneShot(string audioKey);
        void Play(string audioKey, int trackIndex = 0);
        void SetParam(string audioParamKey, float paramValue);
        void Stop(int trackIndex = 0, bool stopImmediately = false);
    }
}