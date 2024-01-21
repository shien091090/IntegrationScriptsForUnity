using UnityEngine;

namespace SNShien.Common.AdapterTools
{
    public class Collider2DAdapterComponent : MonoBehaviour
    {
        [SerializeField] private ColliderHandleType handleType;
        private ICollider2DHandler handler;

        public void InitHandler(ICollider2DHandler handler)
        {
            this.handler = handler;
        }

        public void OnCollisionEnter2D(Collision2D col)
        {
            if (handleType == ColliderHandleType.Collision)
                handler?.CollisionEnter2D(new Collision2DAdapter(col));
        }

        public void OnCollisionExit2D(Collision2D col)
        {
            if (handleType == ColliderHandleType.Collision)
                handler?.CollisionExit2D(new Collision2DAdapter(col));
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
            if (handleType == ColliderHandleType.Trigger)
                handler?.ColliderTriggerEnter2D(new Collider2DAdapter(col));
        }

        public void OnTriggerExit2D(Collider2D col)
        {
            if (handleType == ColliderHandleType.Trigger)
                handler?.ColliderTriggerExit2D(new Collider2DAdapter(col));
        }

        public void OnTriggerStay2D(Collider2D col)
        {
            if (handleType == ColliderHandleType.Trigger)
                handler?.ColliderTriggerStay2D(new Collider2DAdapter(col));
        }
    }
}