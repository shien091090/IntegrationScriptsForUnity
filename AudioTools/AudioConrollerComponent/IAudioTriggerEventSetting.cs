using System.Collections.Generic;
using FMODUnity;

namespace SNShien.Common.AudioTools
{
    public interface IAudioTriggerEventSetting
    {
        EventReference GetAudioEventReference(string triggerTypeName);
        List<AudioEventCollection> GetAudioEventCollections { get; }
    }
}