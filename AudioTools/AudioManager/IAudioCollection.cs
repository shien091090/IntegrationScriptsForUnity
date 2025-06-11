#if CUSTOM_USING_FMOD
using System.Collections.Generic;
using FMODUnity;

namespace SNShien.Common.AudioTools
{
    public interface IAudioCollection
    {
        List<string> GetBankAssetNameList { get; }
        List<FmodAudioCollection> GetAudioEventRefList { get; }
        EventReference GetEventReference(string audioKey);
    }
}
#endif