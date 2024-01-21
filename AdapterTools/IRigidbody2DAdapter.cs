using UnityEngine;

namespace SNShien.Common.AdapterTools
{
    public interface IRigidbody2DAdapter
    {
        Vector3 position { get; set; }
        Vector2 velocity { get; set; }
        void AddForce(Vector2 forceVector, ForceMode2D forceMode = ForceMode2D.Force);
    }
}