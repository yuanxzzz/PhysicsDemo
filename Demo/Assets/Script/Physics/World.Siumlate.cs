using UnityEngine;

namespace PhysicsDemo
{
    public partial class World
    {
        /// <summary>
        /// 整合受力
        /// </summary>
        private void IntegrateForces()
        {
            foreach (var body in m_bodiesDict.Values)
            {
                if (!body.IsActive || body.IsStatic) continue;

                if (body.AffectedByGravity)
                {
                    body.Velocity += Gravity * StepDeltaTime;
                }

                // 速度
                body.Velocity += body.InverseMass * body.Force;
                // 角速度
                MathHelper.Transform(body.Torque, body.InverseInertiaWorld, out var t);
                body.AngularVelocity += t * StepDeltaTime;

                // 阻尼
                body.AngularVelocity *= body.AngularDampingMultiplier;
                body.Velocity *= body.LinearDampingMultiplier;
            }
        }

        /// <summary>
        /// 更新刚体Transform
        /// </summary>
        private void RigidBodyTransformUpdate()
        {
            foreach (var body in m_bodiesDict.Values)
            {
                if (body.IsStatic) continue;

                Vector3 lvel = body.Velocity;
                Vector3 avel = body.AngularVelocity;

                if (lvel == Vector3.zero && avel == Vector3.zero) { continue; }

                var newPos = body.Position + lvel * StepDeltaTime;

                // 计算旋转四元数
                Quaternion rotationQuaternion = Quaternion.AngleAxis(avel.magnitude * Mathf.Rad2Deg * StepDeltaTime, avel.normalized);

                // 将旋转四元数应用到当前旋转矩阵
                var newOri = Matrix4x4.TRS(Vector3.zero, rotationQuaternion, Vector3.one) * body.Orientation;

                // 更新位置和旋转
                body.TransformUpdate(newPos, newOri);
            }
        }

        /// <summary>
        /// 求解
        /// </summary>
        private void Resolve()
        {
            foreach (var collision in m_collisionDataDict.Values)
            {
                collision.CollisionHandle();
            }
        }

    }
}
