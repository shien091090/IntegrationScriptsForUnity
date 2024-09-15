using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using SNShien.Common.AssetTools;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SNShien.Common.AudioTools
{
    public class FmodAudioManager : IAudioManager
    {
        private FMOD.Studio.System studioSystem = RuntimeManager.StudioSystem;
        private Dictionary<int, EventInstance> eventInstanceTrackDict;
        private Dictionary<string, EventReference> audioCollectionDict;

        public List<string> GetAudioKeyList => new List<string>(audioCollectionDict.Keys);

        public FmodAudioManager()
        {
            audioCollectionDict = new Dictionary<string, EventReference>();
        }

        public void PlayOneShot(string audioKey)
        {
            if (TryGetAudioEventReference(audioKey, out EventReference eventReference))
                RuntimeManager.PlayOneShot(eventReference);
        }

        private bool TryGetAudioEventReference(string audioKey, out EventReference eventReference)
        {
            audioKey = audioKey.Replace("_", string.Empty).ToLower();
            return audioCollectionDict.TryGetValue(audioKey, out eventReference);
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
                Debug.Log("[FmodAudioManager] [InitCollectionFromProject] banks is null or empty");
                return;
            }

            foreach (Bank bank in banks)
            {
                bank.getPath(out string bankPath);
                bank.getEventList(out EventDescription[] eventDescriptions);
                List<string> audioEventList = new List<string>();

                foreach (EventDescription eventDescription in eventDescriptions)
                {
                    eventDescription.getPath(out string path);
                    EventReference eventReference = RuntimeManager.PathToEventReference(path);
                    string[] split = path.Split('/');
                    string audioKey = split[split.Length - 1];
                    audioKey = audioKey.Replace("_", string.Empty).ToLower();
                    audioEventList.Add(audioKey);
                    audioCollectionDict.Add(audioKey, eventReference);
                }

                Debug.Log($"[FmodAudioManager] [InitCollectionFromProject] bank: {bankPath}, audioEventList: {string.Join(",\n", audioEventList)}");
            }
        }

        public void InitCollectionFromSetting(IAudioCollection collectionSetting)
        {
            List<string> logs = new List<string>();
            foreach (FmodAudioCollection collectionInfo in collectionSetting.GetAudioEventRefList)
            {
                audioCollectionDict[collectionInfo.GetAudioKey] = collectionInfo.GetEventRef;
                logs.Add($"audio key: {collectionInfo.GetAudioKey}, GUID: {collectionInfo.GetEventRef.Guid}");
            }

            Debug.Log($"[FmodAudioManager] [InitCollectionFromSetting] audioEventList:\n {string.Join(",\n", logs)}");
        }

        public void LoadAudioTextAsset(IAssetManager assetManager)
        {
            // List<string> loadBankNames = audioCollection.GetLoadBankNameList;
            //
            // if (loadBankNames == null)
            // return;
            //
            // foreach (string bankName in loadBankNames)
            // {
            //     TextAsset audioTextAsset = assetManager.GetAsset<TextAsset>(bankName);
            //     if (audioTextAsset == null)
            //         continue;
            //
            //     RuntimeManager.LoadBank(audioTextAsset);
            // }
        }
    }
}