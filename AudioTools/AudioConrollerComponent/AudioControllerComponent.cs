using System.Reflection;
using FMODUnity;
using SNShien.Common.ArchitectureTools;
using UnityEngine;
using Zenject;

namespace SNShien.Common.AudioTools
{
    public class AudioControllerComponent : MonoBehaviour
    {
        [Inject] private IAudioManager audioManager;
        [Inject] private IAudioTriggerEventSetting audioEventSetting;

        public void SetupRegisterAudioEvent(IEventRegister eventRegister)
        {
            MethodInfo methodInfo = typeof(AudioControllerComponent).GetMethod("RegisterTriggerEvent");
            if (methodInfo == null || audioEventSetting == null)
                return;

            foreach (AudioEventCollection audioEventCollection in audioEventSetting.GetAudioEventCollections)
            {
                MethodInfo method = methodInfo.MakeGenericMethod(audioEventCollection.GetTriggerEventTypeName());
                method.Invoke(this, new object[] { eventRegister });
            }
        }

        public void RegisterTriggerEvent<T>(IEventRegister eventRegister) where T : IArchitectureEvent
        {
            eventRegister.Unregister<T>(PlayAudio);
            eventRegister.Register<T>(PlayAudio);
        }

        private void PlayAudio<T>(T eventInfo)
        {
            EventReference audioEventReference = audioEventSetting.GetAudioEventReference(typeof(T).Name);
            audioManager.PlayOneShot(audioEventReference);
        }
    }
}