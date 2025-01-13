using System;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    [RequireComponent(typeof(ComputableTriggerUI))]
    public class TriggerUICrossDetector : MonoBehaviour
    {
        private ComputableTriggerUI computableTriggerUI;

        private void SetEventRegister(bool isListen)
        {
            computableTriggerUI.OnChangeTriggeredState -= OnChangeTriggeredState;
            
            if (isListen)
            {
                computableTriggerUI.OnChangeTriggeredState += OnChangeTriggeredState;
            }
        }

        private void Awake()
        {
            computableTriggerUI = GetComponent<ComputableTriggerUI>();
        }

        private void OnEnable()
        {
            SetEventRegister(true);
        }

        private void OnChangeTriggeredState(bool isTriggered)
        {
        }
    }
}