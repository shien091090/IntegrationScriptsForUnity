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
        private readonly IAudioCollection audioCollection;
        private Dictionary<int, EventInstance> eventInstanceTrackDict;
        private FMOD.Studio.System studioSystem = RuntimeManager.StudioSystem;

        public FmodAudioManager(IAudioCollection audioCollection, IAssetManager assetManager)
        {
            this.audioCollection = audioCollection;
            LoadAudioTextAsset(assetManager);
        }

        private void LoadAudioTextAsset(IAssetManager assetManager)
        {
            List<string> loadBankNames = audioCollection.GetLoadBankNameList;

            if (loadBankNames == null)
                return;

            foreach (string bankName in loadBankNames)
            {
                TextAsset audioTextAsset = assetManager.GetAsset<TextAsset>(bankName);
                if (audioTextAsset == null)
                    continue;

                RuntimeManager.LoadBank(audioTextAsset);
            }
        }

        public void PlayOneShot(string audioKey)
        {
            RuntimeManager.PlayOneShot(audioCollection.GetEventReference(audioKey));
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
            EventReference eventReference = audioCollection.GetEventReference(audioKey);
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
    }
}