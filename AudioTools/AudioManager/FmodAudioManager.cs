using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using SNShien.Common.AssetTools;
using SNShien.Common.DataTools;
using SNShien.Common.TesterTools;
using UnityEngine;
using Zenject;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SNShien.Common.AudioTools
{
    public class FmodAudioManager : IAudioManager
    {
        private const string DEBUGGER_KEY = "FmodAudioManager";
        private static EVENT_CALLBACK audioCallbackEvent;

        [InjectOptional] private IAssetManager assetManager;

        private FMOD.Studio.System studioSystem = RuntimeManager.StudioSystem;
        private Dictionary<int, EventInstance> eventInstanceTrackDict;
        private Dictionary<string, EventReference> audioCollectionDict;
        private FmodAudioCallbackSetting callbackSetting;

        private readonly JsonParser jsonParser;
        private readonly Debugger debugger;

        public List<string> GetAudioKeyList => new List<string>(audioCollectionDict.Keys);

        public FmodAudioManager()
        {
            audioCollectionDict = new Dictionary<string, EventReference>();
            jsonParser = new JsonParser();
            debugger = new Debugger(DEBUGGER_KEY);
            audioCallbackEvent = OnAudioCallback;
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
            EventInstance eventInstance = GetEventInstance(eventReference, trackIndex);
            eventInstanceTrackDict[trackIndex] = eventInstance;
            callbackSetting = null;
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

            callbackSetting = null;
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

        public void InitCollectionFromBundle(IAudioCollection collectionSetting, FmodAudioInitType initType)
        {
            List<string> bankAssetNameList = collectionSetting.GetBankAssetNameList;
            if (bankAssetNameList == null || bankAssetNameList.Count == 0)
            {
                debugger.ShowLog("bankAssetNameList is null or empty", true);
                return;
            }

            if (assetManager == null)
            {
                debugger.ShowLog("assetManager is null", true);
                return;
            }

            foreach (string bankAssetName in bankAssetNameList)
            {
                TextAsset textAsset = assetManager.GetAsset<TextAsset>(bankAssetName);
                RuntimeManager.LoadBank(textAsset);
            }

            debugger.ShowLog($"LoadBank success, bankAssetNameList:\n{string.Join("\n", bankAssetNameList)}", true);

            switch (initType)
            {
                case FmodAudioInitType.FromSetting:
                    InitCollectionFromSetting(collectionSetting);
                    break;

                case FmodAudioInitType.FromProject:
                    InitCollectionFromProject();
                    break;
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

        public FmodAudioCallbackSetting PlayWithCallback(string audioKey, int trackIndex = 0)
        {
            if (TryGetAudioEventReference(audioKey, out EventReference eventReference) == false)
                return null;

            callbackSetting = new FmodAudioCallbackSetting();

            EventInstance eventInstance = GetEventInstance(eventReference, trackIndex);
            eventInstanceTrackDict[trackIndex] = eventInstance;

            eventInstance.setCallback(audioCallbackEvent, EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
            eventInstance.start();

            return callbackSetting;
        }

        private bool TryGetAudioEventReference(string audioKey, out EventReference eventReference)
        {
            string convertAudioKey = ConvertDictionaryKey(audioKey);
            return audioCollectionDict.TryGetValue(convertAudioKey, out eventReference);
        }

        private EventInstance GetEventInstance(EventReference eventReference, int trackIndex = 0)
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
            return eventInstance;
        }

        private string ConvertDictionaryKey(string audioKey)
        {
            return audioKey.Replace("_", string.Empty).ToLower();
        }

        private RESULT OnAudioCallback(EVENT_CALLBACK_TYPE type, IntPtr _event, IntPtr parameters)
        {
            if (Application.isPlaying == false)
            {
                debugger.ShowLog($"is not playing and return", true);
                return RESULT.OK;
            }

            if (type == EVENT_CALLBACK_TYPE.SOUND_STOPPED ||
                type == EVENT_CALLBACK_TYPE.DESTROYED)
            {
                debugger.ShowLog("AudioCallback Release", true);
                callbackSetting = null;
                audioCallbackEvent = null;
                return RESULT.OK;
            }

            callbackSetting?.TryCallback(type);
            return RESULT.OK;
        }
    }
}