using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

namespace SNShien.Common.AudioTools
{
    [CreateAssetMenu]
    public class FmodAudioCollectionScriptableObject : ScriptableObject, IAudioCollection
    {
        [SerializeField] private List<FmodAudioCollection> audioEventRefList;

        public EventReference GetEventReference(string audioKey)
        {
            FmodAudioCollection audioCollection = audioEventRefList.FirstOrDefault(x => x.GetAudioKey == audioKey);
            return audioCollection?.GetEventRef ?? default;
        }
    }
}