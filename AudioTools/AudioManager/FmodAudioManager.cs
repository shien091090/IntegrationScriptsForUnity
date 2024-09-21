using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using SNShien.Common.DataTools;
using SNShien.Common.TesterTools;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SNShien.Common.AudioTools
{
    public class FmodAudioManager : IAudioManager
    {
        private const string DEBUGGER_KEY = "FmodAudioManager";

        private FMOD.Studio.System studioSystem = RuntimeManager.StudioSystem;
        private Dictionary<int, EventInstance> eventInstanceTrackDict;
        private Dictionary<string, EventReference> audioCollectionDict;

        private readonly JsonParser jsonParser;
        private readonly Debugger debugger;

        public List<string> GetAudioKeyList => new List<string>(audioCollectionDict.Keys);

        public FmodAudioManager()
        {
            audioCollectionDict = new Dictionary<string, EventReference>();
            jsonParser = new JsonParser();
            debugger = new Debugger(DEBUGGER_KEY);
        }

        public void PlayOneShot(string audioKey)
        {
            if (TryGetAudioEventReference(audioKey, out EventReference eventReference))
                RuntimeManager.PlayOneShot(eventReference);
        }

        public void PlayOneShot(EventReference eventReference)
        {
            RuntimeManager.PlayOneShot(eventReference);
        }

        public void Play(EventReference eventReference, int trackIndex = 0)
        {
            if (eventInstanceTrackDict == null)
                eventInstanceTrackDict = new Dictionary<int, EventInstance>();

            EventInstance eventInstance;
            if (eventInstanceTrackDict.ContainsKey(trackIndex))
            {
                eventInstance = eventInstanceTrackDict[trackIndex];
                eventInstance.getPlaybackState(out PLAYBACK_STATE playbackState);
                if (playbackState == PLAYBACK_STATE.PLAYING)
                    eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
            }

            eventInstance = RuntimeManager.CreateInstance(eventReference);
            eventInstanceTrackDict[trackIndex] = eventInstance;

            eventInstance.start();
        }

        public void Play(string audioKey, int trackIndex = 0)
        {
            if (TryGetAudioEventReference(audioKey, out EventReference eventReference))
                Play(eventReference, trackIndex);
        }

        public void SetParam(string audioParamKey, float paramValue)
        {
            studioSystem.setParameterByName(audioParamKey, paramValue);
        }

        public void Stop(int trackIndex = 0, bool stopImmediately = false)
        {
            if (!eventInstanceTrackDict.ContainsKey(trackIndex))
                return;

            EventInstance eventInstance = eventInstanceTrackDict[trackIndex];
            eventInstance.stop(stopImmediately ?
                STOP_MODE.IMMEDIATE :
                STOP_MODE.ALLOWFADEOUT);
        }

        public void InitCollectionFromProject()
        {
            audioCollectionDict = new Dictionary<string, EventReference>();

            RuntimeManager.StudioSystem.getBankList(out Bank[] banks);
            if (banks == null || banks.Length == 0)
            {
                debugger.ShowLog("banks is null or empty", true);
                return;
            }

            foreach (Bank bank in banks)
            {
                bank.getPath(out string bankPath);
                bank.getEventList(out EventDescription[] eventDescriptions);
                List<string> audioEventLogs = new List<string>();

                foreach (EventDescription eventDescription in eventDescriptions)
                {
                    eventDescription.getPath(out string path);
                    EventReference eventReference = RuntimeManager.PathToEventReference(path);
                    string[] split = path.Split('/');
                    string audioKey = split[split.Length - 1];
                    audioKey = ConvertDictionaryKey(audioKey);
                    audioEventLogs.Add(audioKey);
                    audioCollectionDict.Add(audioKey, eventReference);
                }

                debugger.ShowLog($"bank: {bankPath}, audioEventList: {string.Join(",\n", audioEventLogs)}", true);
            }
        }

        public void InitCollectionFromSetting(IAudioCollection collectionSetting)
        {
            List<string> logs = new List<string>();
            foreach (FmodAudioCollection collectionInfo in collectionSetting.GetAudioEventRefList)
            {
                string audioKey = ConvertDictionaryKey(collectionInfo.GetAudioKey);
                audioCollectionDict[audioKey] = collectionInfo.GetEventRef;
                logs.Add($"audio key: {audioKey}, GUID: {collectionInfo.GetEventRef.Guid}");
            }

            debugger.ShowLog($"audioEventList:\n{string.Join(",\n", logs)}", true);
        }

        private bool TryGetAudioEventReference(string audioKey, out EventReference eventReference)
        {
            string convertAudioKey = ConvertDictionaryKey(audioKey);
            return audioCollectionDict.TryGetValue(convertAudioKey, out eventReference);
        }

        private string ConvertDictionaryKey(string audioKey)
        {
            return audioKey.Replace("_", string.Empty).ToLower();
        }
    }
}