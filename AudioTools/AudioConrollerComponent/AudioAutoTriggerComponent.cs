using System.Reflection;
using FMODUnity;
using SNShien.Common.ProcessTools;
using UnityEngine;
using Zenject;

namespace SNShien.Common.AudioTools
{
    public class AudioAutoTriggerComponent : MonoBehaviour
    {
        [Inject] private IAudioManager audioManager;
        [Inject] private IAudioTriggerEventSetting audioEventSetting;

        public void SetupRegisterAudioEvent(IEventRegister eventRegister)
        {
            if (audioEventSetting == null)
                return;

            foreach (AudioEventCollection audioEventCollection in audioEventSetting.GetAudioEventCollections)
            {
                MethodInfo methodInfo = null;
                switch (audioEventCollection.GetActionType)
                {
                    case AudioTriggerEventActionType.PlayOneShot:
                        methodInfo = typeof(AudioAutoTriggerComponent).GetMethod("RegisterTriggerPlayOneShotEvent");
                        break;

                    case AudioTriggerEventActionType.Play:
                        methodInfo = typeof(AudioAutoTriggerComponent).GetMethod("RegisterTriggerPlayEvent");
                        break;

                    case AudioTriggerEventActionType.Stop:
                        methodInfo = typeof(AudioAutoTriggerComponent).GetMethod("RegisterTriggerStopEvent");
                        break;
                }

                if (methodInfo == null)
                    continue;

                MethodInfo method = methodInfo.MakeGenericMethod(audioEventCollection.GetTriggerEventTypeName());
                method.Invoke(this, new object[] { eventRegister });
            }
        }

        public void RegisterTriggerPlayEvent<T>(IEventRegister eventRegister) where T : IArchitectureEvent
        {
            eventRegister.Unregister<T>(Play);
            eventRegister.Register<T>(Play);
        }

        public void RegisterTriggerStopEvent<T>(IEventRegister eventRegister) where T : IArchitectureEvent
        {
            eventRegister.Unregister<T>(Stop);
            eventRegister.Register<T>(Stop);
        }

        private bool CheckIsPreSetParam(IAudioTriggerEvent audioTriggerEvent)
        {
            return string.IsNullOrEmpty(audioTriggerEvent.PreSetParamName) == false;
        }

        private void Stop<T>(T eventInfo)
        {
            if (CheckIsPreSetParam((IAudioTriggerEvent)eventInfo))
                PreSetParam((IAudioTriggerEvent)eventInfo);

            int trackIndex = audioEventSetting.GetAudioEventTrackIndex(typeof(T).Name);
            audioManager.Stop(trackIndex);
        }

        private void PreSetParam(IAudioTriggerEvent audioTriggerEvent)
        {
            audioManager.SetParam(audioTriggerEvent.PreSetParamName, audioTriggerEvent.PreSetParamValue);
        }

        private void Play<T>(T eventInfo)
        {
            if (CheckIsPreSetParam((IAudioTriggerEvent)eventInfo))
                PreSetParam((IAudioTriggerEvent)eventInfo);

            EventReference audioEventReference = audioEventSetting.GetAudioEventReference(typeof(T).Name);
            int trackIndex = audioEventSetting.GetAudioEventTrackIndex(typeof(T).Name);
            audioManager.Play(audioEventReference, trackIndex);
        }

        public void RegisterTriggerPlayOneShotEvent<T>(IEventRegister eventRegister) where T : IArchitectureEvent
        {
            eventRegister.Unregister<T>(PlayOneShot);
            eventRegister.Register<T>(PlayOneShot);
        }

        private void PlayOneShot<T>(T eventInfo)
        {
            if (CheckIsPreSetParam((IAudioTriggerEvent)eventInfo))
                PreSetParam((IAudioTriggerEvent)eventInfo);

            EventReference audioEventReference = audioEventSetting.GetAudioEventReference(typeof(T).Name);
            audioManager.PlayOneShot(audioEventReference);
        }
    }
}