using System.Collections.Generic;
using UnityEngine;

namespace PhysicsDemo
{
    public struct RigidBodyCreateData
    {
        public bool IsStatic;
    }

    /// <summary>
    /// RigidBody
    /// </summary>
    public class RigidBody
    {
        public RigidBody(World world, RigidBodyCreateData data)
        {
            World = world;
            RigidBodyId = World.RequestId();

            IsStatic = data.IsStatic;

            DefaultMassInertiaSet();
        }

        #region 公共方法

        public void TransformUpdate(Vector3 pos, Matrix4x4 ori)
        {
            Position = pos;
            Orientation = ori;

            // 更新惯性矩阵
            InertiaUpdate();

            // 设为Active
            IsActive = true;

            // 更新对应Shape
            foreach (var shape in Shapes)
            {
                shape.ShapeUpdate();
                World.ShapeUpdate(shape);
            }
        }

        /// <summary>
        /// 施加力
        /// </summary>
        /// <param name="force"></param>
        public void ForceAdd(in Vector3 force)
        {
            Force += force;
        }

        public void ForceAdd(in Vector3 force, in Vector3 position)
        {
            var torque = Vector3.Cross(position - Position, force);

            Force += force;
            Torque += torque;
        }

        /// <summary>
        /// 添加Shape
        /// </summary>
        /// <param name="shape"></param>
        public void ShapeAdd(Shape shape)
        {
            shape.RigidBodyAttach(this);
            Shapes.Add(shape);
            World.ShapeAttach(shape);
            MassInertiaUpdate();
        }

        /// <summary>
        /// 移除Shape
        /// </summary>
        /// <param name="shape"></param>
        public void ShapeRemove(Shape shape)
        {
            shape.RigidBodyDetach();
            Shapes.Remove(shape);
            World.ShapeDetach(shape);
            MassInertiaUpdate();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 设置默认的质量和惯性
        /// </summary>
        private void DefaultMassInertiaSet()
        {
            InverseInertia = Matrix4x4.identity;
            if (IsStatic)
            {
                InverseMass = 0.0f;
            }
            else
            {
                InverseMass = 1.0f;
            }
            InertiaUpdate();
        }

        /// <summary>
        /// 更新质量和惯性
        /// </summary>
        public void MassInertiaUpdate()
        {
            if (Shapes.Count == 0)
            {
                InverseInertia = Matrix4x4.identity;
                InverseMass = 1.0f;
                return;
            }

            Matrix4x4 inertia = Matrix4x4.zero;
            float mass = 0.0f;

            for (int i = 0; i < Shapes.Count; i++)
            {
                MathHelper.Add(inertia, Shapes[i].Inertia, out inertia);
                mass += Shapes[i].Mass;
            }

            InverseInertia = Matrix4x4.Inverse(inertia);
            InverseMass = 1.0f / mass;

            InertiaUpdate();
        }

        /// <summary>
        /// 更新惯性矩阵的逆
        /// </summary>
        private void InertiaUpdate()
        {
            if (IsStatic)
            {
                InverseInertiaWorld = Matrix4x4.zero;
            }
            else
            {
                MathHelper.MultiplyTransposed(Orientation * InverseInertia, Orientation, out var inverseInertiaWorld);
                InverseInertiaWorld = inverseInertiaWorld;
            }
        }
        #endregion

        #region 字段&属性

        /// <summary>
        /// id
        /// </summary>
        public readonly ulong RigidBodyId;

        /// <summary>
        /// 刚体对应shape
        /// </summary>
        public List<Shape> Shapes { get; } = new(1);

        /// <summary>
        /// 是否Active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 是否为静态
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// 受力
        /// </summary>
        public Vector3 Force { get; set; }

        /// <summary>
        /// 力矩
        /// </summary>
        public Vector3 Torque { get; set; }


        #region Data

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 Position { get; set; } = Vector3.zero;

        /// <summary>
        /// 朝向
        /// </summary>
        public Matrix4x4 Orientation { get; set; } = Matrix4x4.identity;

        /// <summary>
        /// 速度
        /// </summary>
        public Vector3 Velocity { get; set; } = Vector3.zero;
        public Vector3 AngularVelocity { get; set; } = Vector3.zero;

        /// <summary>
        /// 加速度
        /// </summary>
        public Vector3 LinearAcceleration { get; set; }
        public Vector3 AngularAcceleration { get; set; }

        /// <summary>
        /// 惯性矩阵
        /// </summary>
        public Matrix4x4 InverseInertia { get; set; }
        public Matrix4x4 InverseInertiaWorld { get; set; }

        /// <summary>
        /// 摩擦
        /// </summary>
        public float Friction { get; set; }
        /// <summary>
        /// 弹性系数
        /// </summary>
        public float Restitution { get; set; }

        /// <summary>
        /// 阻尼
        /// </summary>
        public float LinearDampingMultiplier = 0.995f;
        public float AngularDampingMultiplier = 0.995f;

        /// <summary>
        /// 质量
        /// </summary>
        public float Mass => 1.0f / InverseMass;
        public float InverseMass { get; set; } = 1;

        #endregion

        /// <summary>
        /// 所属World
        /// </summary>
        public World World { get; }

        /// <summary>
        /// 是否受重力影响
        /// </summary>
        public bool AffectedByGravity { get; set; } = true;
        #endregion
    }
}
