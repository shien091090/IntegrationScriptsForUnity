using UnityEngine;

namespace SNShien.Common.AdapterTools
{
    public class RigidBody2DAdapterComponent : MonoBehaviour, IRigidbody2DAdapter
    {
        public Vector3 position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Vector2 velocity
        {
            get => GetRigidbody.linearVelocity;
            set => GetRigidbody.linearVelocity = value;
        }

        private Rigidbody2D rigidbodyComponent;

        private Rigidbody2D GetRigidbody
        {
            get
            {
                if (rigidbodyComponent == null)
                    rigidbodyComponent = GetComponent<Rigidbody2D>();

                return rigidbodyComponent;
            }
        }

        public void AddForce(Vector2 forceVector, ForceMode2D forceMode = ForceMode2D.Force)
        {
            GetRigidbody.AddForce(forceVector, forceMode);
        }
    }
}