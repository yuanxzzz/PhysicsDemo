using PhysicsDemo;
using UnityEngine;

namespace Assets.Script.Physics
{
    public static class CollisionHelper
    {
        /// <summary>
        /// 检测
        /// </summary>
        /// <param name="sA"></param>
        /// <param name="sB"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool Detect(Shape sA, Shape sB, out CollisionResult result)
        {
            return DetectSphere2Sphere((SphereShape)sA, (SphereShape)sB, out result);
        }

        /// <summary>
        /// 球形和球形检测
        /// </summary>
        /// <param name="sA"></param>
        /// <param name="posA"></param>
        /// <param name="oriA"></param>
        /// <param name="sB"></param>
        /// <param name="posB"></param>
        /// <param name="oriB"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool DetectSphere2Sphere(SphereShape sA, SphereShape sB, out CollisionResult result)
        {
            result = new CollisionResult();
            bool colliding = (sA.Position - sB.Position).magnitude < 1f;
            if (colliding)
            {
                result.m_body1 = sA.RigidBody;
                result.m_body2 = sB.RigidBody;
                // todo:构建collisionResult
                //result.init();
            }
            return colliding;
        }
    }
}
