#if CUSTOM_USING_FMOD
using System;
using System.Collections.Generic;
using FMOD.Studio;

namespace SNShien.Common.AudioTools
{
    public class FmodAudioCallbackSetting
    {
        private Dictionary<EVENT_CALLBACK_TYPE, Action> callbackDict;

        public FmodAudioCallbackSetting()
        {
            callbackDict = new Dictionary<EVENT_CALLBACK_TYPE, Action>();
        }

        public void TryCallback(EVENT_CALLBACK_TYPE type)
        {
            if (callbackDict.TryGetValue(type, out Action callback))
                callback?.Invoke();
        }

        public void Register(EVENT_CALLBACK_TYPE callbackType, Action action)
        {
            callbackDict[callbackType] = action;
        }
    }
}
#endif