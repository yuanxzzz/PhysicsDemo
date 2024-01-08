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
            GJKEPA.Detect(sA, sB, sA.RigidBody.Orientation,
                sB.RigidBody.Orientation, sA.Position, sB.Position, out var pointA, out var pointB, out var normal, out var separation);
            result = new CollisionResult();
            bool colliding = separation < 0;
            if (separation < 0)
            {
                result.m_body1 = sA.RigidBody;
                result.m_body2 = sB.RigidBody;
                result.Init(sA.RigidBody, sB.RigidBody, normal, pointA, pointB,
                    Mathf.Max(sA.RigidBody.Restitution, sB.RigidBody.Restitution),
                    Mathf.Max(sA.RigidBody.Friction, sB.RigidBody.Friction));
            }
            else
            {
                result.collising = false;
            }
            return colliding;
        }
    }
}
