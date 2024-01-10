using System;
using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// 碰撞
    /// </summary>
    public struct CollisionResult
    {
        public bool collising;

        public RigidBody m_body1;
        public RigidBody m_body2;


        public float Bias;
        /// <summary>
        /// 摩擦
        /// </summary>
        private float Friction;
        /// <summary>
        /// 弹性系数
        /// </summary>
        private float Restitution;

        /// <summary>
        /// 法线和切线
        /// </summary>
        private Vector3 Normal;
        private Vector3 Tangent1;
        private Vector3 Tangent2;

        /// <summary>
        /// 相对位置
        /// </summary>
        private Vector3 RelativePos1;
        private Vector3 RelativePos2;

        /// <summary>
        /// 相对速度
        /// </summary>
        private Vector3 RelativeVelocity;


        private Vector3 M_n1;
        private Vector3 M_t1;
        private Vector3 M_tt1;

        private Vector3 M_n2;
        private Vector3 M_t2;
        private Vector3 M_tt2;

        /// <summary>
        /// 累积冲量
        /// </summary>
        public float AccumulatedTangentImpulse1;
        public float AccumulatedTangentImpulse2;
        public float AccumulatedNormalImpulse;

        public float MassNormal;

        public float MassTangent1;
        public float MassTangent2;
        public float MaxTangentImpulse;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <param name="restitution"></param>
        /// <param name="friction"></param>
        public void Init(RigidBody body1, RigidBody body2, Vector3 normal, Vector3 pointA, Vector3 pointB, float restitution, float friction)
        {
            collising = true;
            m_body1 = body1;
            m_body2 = body2;

            // 碰撞点的相对位置
            RelativePos1 = pointA - body1.Position;
            RelativePos2 = pointB - body2.Position;

            Normal = normal;

            // 相对速度
            RelativeVelocity = body2.Velocity - body1.Velocity;


            // 计算切线
            Vector3 dv = body2.Velocity + Vector3.Cross(body2.AngularVelocity, RelativePos2);
            dv -= body1.Velocity + Vector3.Cross(body1.AngularVelocity, RelativePos1);
            float relNormalVel = Vector3.Dot(dv, Normal);

            // Fake restitution
            if (relNormalVel < -1.0f)
            {
                Bias = Math.Max(-restitution * relNormalVel, Bias);
            }

            // 切线方向
            Tangent1 = dv - Normal * relNormalVel;
            float num = Tangent1.sqrMagnitude;
            if (num > 1e-12f)
            {
                num = 1.0f / MathF.Sqrt(num);
                Tangent1 *= num;
            }
            else
            {
                Tangent1 = MathHelper.CreateOrthonormal(Normal);
            }

            Tangent2 = Vector3.Cross(Tangent1, Normal);

            Restitution = restitution;
            Friction = friction;
        }

        /// <summary>
        /// 碰撞处理
        /// </summary>
        public void CollisionHandle()
        {
            if (!collising)
            {
                return;
            }

            // 迭代前相关数据计算 
            IteratePrepare(m_body1, m_body2);

            // 迭代
            // 这里只做了一次，因为只记录了最近的一个碰撞点
            Iterate(m_body1, m_body2);
        }


        /// <summary>
        /// 计算冲量大小
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        private void IteratePrepare(RigidBody b1, RigidBody b2)
        {
            // 接触点速度
            MathHelper.Transform(RelativePos1, b1.Orientation, out RelativePos1);
            var vp1 = b1.Velocity + RelativePos1;
            MathHelper.Transform(RelativePos2, b2.Orientation, out RelativePos2);
            var vp2 = b2.Velocity + RelativePos2;
            // 刚体在法线上的转动惯量
            var tt = Vector3.Cross(RelativePos1, Normal);
            MathHelper.Transform(tt, b1.InverseInertiaWorld, out M_n1);

            tt = Vector3.Cross(RelativePos2, Normal);
            MathHelper.Transform(tt, b2.InverseInertiaWorld, out M_n2);

            // 计算在切线方向上的转动惯量
            tt = Vector3.Cross(RelativePos1, Tangent1);
            MathHelper.Transform(tt, b1.InverseInertiaWorld, out M_t1);

            tt = Vector3.Cross(RelativePos2, Tangent1);
            MathHelper.Transform(tt, b2.InverseInertiaWorld, out M_t2);

            tt = Vector3.Cross(RelativePos1, Tangent2);
            MathHelper.Transform(tt, b1.InverseInertiaWorld, out M_tt1);

            tt = Vector3.Cross(RelativePos2, Tangent2);
            MathHelper.Transform(tt, b2.InverseInertiaWorld, out M_tt2);


            float kTangent1 = 0.0f;
            float kTangent2 = 0.0f;
            float kNormal = 0.0f;

            kTangent1 += b1.InverseMass;
            kTangent2 += b1.InverseMass;
            kNormal += b1.InverseMass;

            var rbntrb = Vector3.Cross(M_t2, RelativePos2);
            kTangent1 += Vector3.Dot(rbntrb, Tangent1);

            rbntrb = Vector3.Cross(M_tt2, RelativePos2);
            kTangent2 += Vector3.Dot(rbntrb, Tangent2);

            rbntrb = Vector3.Cross(M_n2, RelativePos2);
            kNormal += Vector3.Dot(rbntrb, Normal);

            MassTangent1 = 1.0f / kTangent1;
            MassTangent2 = 1.0f / kTangent2;
            MassNormal = 1.0f / kNormal;
        }

        public void Iterate(RigidBody b1, RigidBody b2)
        {
            Vector3 dv = b2.Velocity + Vector3.Cross(b2.AngularVelocity, RelativePos2);
            dv -= b1.Velocity + Vector3.Cross(b1.AngularVelocity, RelativePos1);

            float vn = Vector3.Dot(Normal, dv);
            float vt1 = Vector3.Dot(Tangent1, dv);
            float vt2 = Vector3.Dot(Tangent2, dv);

            float normalImpulse = Bias - vn;
            normalImpulse *= MassNormal;

            float oldNormalImpulse = AccumulatedNormalImpulse;
            AccumulatedNormalImpulse = MathF.Max(oldNormalImpulse + normalImpulse, 0.0f);
            normalImpulse = AccumulatedNormalImpulse - oldNormalImpulse;

            float maxTangentImpulse = Friction * AccumulatedNormalImpulse;
            float tangentImpulse1 = MassTangent1 * -vt1;
            float tangentImpulse2 = MassTangent2 * -vt2;

            float oldTangentImpulse = AccumulatedTangentImpulse1;
            AccumulatedTangentImpulse1 = oldTangentImpulse + tangentImpulse1;
            AccumulatedTangentImpulse1 = Math.Clamp(AccumulatedTangentImpulse1, -maxTangentImpulse, maxTangentImpulse);
            tangentImpulse1 = AccumulatedTangentImpulse1 - oldTangentImpulse;

            float oldTangentImpulse2 = AccumulatedTangentImpulse2;
            AccumulatedTangentImpulse2 = oldTangentImpulse2 + tangentImpulse2;
            AccumulatedTangentImpulse2 = Math.Clamp(AccumulatedTangentImpulse2, -maxTangentImpulse, maxTangentImpulse);
            tangentImpulse2 = AccumulatedTangentImpulse2 - oldTangentImpulse2;

            // 应用冲量
            Vector3 impulse = normalImpulse * Normal + tangentImpulse1 * Tangent1 + tangentImpulse2 * Tangent2;

            b1.Velocity -= b1.InverseMass * impulse;
            b1.AngularVelocity -= normalImpulse * M_n1 + tangentImpulse1 * M_t1 + tangentImpulse2 * M_tt1;

            b2.Velocity += b2.InverseMass * impulse;
            b2.AngularVelocity += normalImpulse * M_n2 + tangentImpulse1 * M_t2 + tangentImpulse2 * M_tt2;
        }

    }
}
