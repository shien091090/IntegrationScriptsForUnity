using FMODUnity;

namespace SNShien.Common.AudioTools
{
    public interface IAudioCollection
    {
        EventReference GetEventReference(string audioKey);
    }
}