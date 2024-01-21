using UnityEngine;

namespace SNShien.Common.AdapterTools
{
    public class DeltaTimeGetter : IDeltaTimeGetter
    {
        public float deltaTime => Time.deltaTime;
    }
}