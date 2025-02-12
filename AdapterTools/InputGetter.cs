using UnityEngine;

namespace SNShien.Common.AdapterTools
{
    public class InputGetter : IInputGetter
    {
        public bool IsClickDown()
        {
#if UNITY_ANDROID || UNITY_IOS
            return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#endif
            return Input.GetMouseButtonDown(0);
        }
    }
}