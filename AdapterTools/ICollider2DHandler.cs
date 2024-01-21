namespace SNShien.Common.AdapterTools
{
    public interface ICollider2DHandler
    {
        public void ColliderTriggerEnter2D(ICollider2DAdapter col);
        public void ColliderTriggerExit2D(ICollider2DAdapter col);
        public void ColliderTriggerStay2D(ICollider2DAdapter col);
        void CollisionEnter2D(ICollision2DAdapter col);
        void CollisionExit2D(ICollision2DAdapter col);
    }
}