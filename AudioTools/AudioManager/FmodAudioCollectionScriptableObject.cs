using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

namespace SNShien.Common.AudioTools
{
    [CreateAssetMenu(fileName = "FmodAudioCollectionSetting", menuName = "SNShien/Create FmodAudioCollectionSetting")]
    public class FmodAudioCollectionScriptableObject : ScriptableObject, IAudioCollection
    {
        [SerializeField] private List<string> loadBankNameList;
        [SerializeField] private List<FmodAudioCollection> audioEventRefList;

        public List<string> GetLoadBankNameList => loadBankNameList;

        public EventReference GetEventReference(string audioKey)
        {
            FmodAudioCollection audioCollection = audioEventRefList.FirstOrDefault(x => x.GetAudioKey == audioKey);
            return audioCollection?.GetEventRef ?? default;
        }
    }
}