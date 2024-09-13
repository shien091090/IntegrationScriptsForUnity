using System.Collections.Generic;
using UnityEngine;

namespace SNShien.Common.AdapterTools
{
    public interface ICollision2DAdapter
    {
        int Layer { get; }
        List<Vector2> ContactPoints { get; }
        T GetComponent<T>();
        bool CheckPhysicsOverlapCircle(Vector3 point, float radius, string layerMask);
    }
}