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

        public void PrintAudioKeys()
        {
            string log = string.Join(", \n", audioCollectionDict.Keys);
            Debug.Log($"Audio Keys: {log}");
        }

        public void PlayOneShot(string audioKey)
        {
            if (audioCollectionDict.TryGetValue(audioKey, out EventReference eventReference))
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
            if (audioCollectionDict.TryGetValue(audioKey, out EventReference eventReference))
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

        public void InitAudioCollection()
        {
            audioCollectionDict = new Dictionary<string, EventReference>();

            RuntimeManager.StudioSystem.getBankList(out Bank[] banks);
            foreach (Bank bank in banks)
            {
                bank.getEventList(out EventDescription[] eventDescriptions);
                foreach (EventDescription eventDescription in eventDescriptions)
                {
                    eventDescription.getPath(out string path);
                    EventReference eventReference = EventReference.Find(path);
                    string[] split = path.Split('/');
                    string audioKey = split[split.Length - 1];
                    audioCollectionDict.Add(audioKey, eventReference);
                }
            }
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