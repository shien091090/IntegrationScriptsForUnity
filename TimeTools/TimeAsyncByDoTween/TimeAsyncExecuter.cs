using System;
using DG.Tweening;
using UnityEngine;

namespace SNShien.Common.TimeTools 
{
    public class TimeAsyncExecuter
    {
        public void DelayedCall(float delayTimes, Action callback)
        {
            if (Application.isPlaying)
                DOVirtual.DelayedCall(delayTimes, () =>
                {
                    callback?.Invoke();
                });
            else
            {
                callback?.Invoke();
            }
        }
    }
}