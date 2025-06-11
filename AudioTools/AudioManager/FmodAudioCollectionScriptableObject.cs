#if CUSTOM_USING_FMOD
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

namespace SNShien.Common.AudioTools
{
    [CreateAssetMenu(fileName = "FmodAudioCollectionSetting", menuName = "SNShien/Create FmodAudioCollectionSetting")]
    public class FmodAudioCollectionScriptableObject : ScriptableObject, IAudioCollection
    {
        [SerializeField] private List<string> bankAssetNameList;
        [SerializeField] private List<FmodAudioCollection> audioEventRefList;

        public List<string> GetBankAssetNameList => bankAssetNameList;
        public List<FmodAudioCollection> GetAudioEventRefList => audioEventRefList;

        public EventReference GetEventReference(string audioKey)
        {
            FmodAudioCollection audioCollection = GetAudioEventRefList.FirstOrDefault(x => x.GetAudioKey == audioKey);
            return audioCollection?.GetEventRef ?? default;
        }
    }
}
#endif