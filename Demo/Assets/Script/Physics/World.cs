using System;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// World
    /// </summary>
    public partial class World
    {

        #region 公共方法

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            octree = new Octree(new Vector3(1000, 1000, 1000), 20, 20);

            //boundsTree = new BoundsOctree<Shape>(15, Vector3.zero, 1, 1.25f);
        }

        //public BoundsOctree<Shape> boundsTree;

        /// <summary>
        /// id索引
        /// </summary>
        /// <returns></returns>
        public static ulong RequestId()
        {
            return m_idCounter++;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            // 1. 碰撞检测
            CollisionDetection();

            // 2. 进行模拟
            SimulateStep();

            // 3. 进行清理
            ContactsClear();
        }

        /// <summary>
        /// 创建body
        /// </summary>
        /// <returns></returns>
        public RigidBody RigidBodyCreate(RigidBodyCreateData data)
        {
            // 创建刚体
            RigidBody body = new RigidBody(this, data);
            body.IsActive = true;

            m_bodiesDict.Add(body.RigidBodyId, body);
            return body;
        }

        /// <summary>
        /// 移除Body
        /// </summary>
        public void RigidBodyRemove(RigidBody body)
        {
            // 清理刚体对应的Shape
            foreach (var shape in body.Shapes)
            {
                ShapeDetach(shape);
                shape.RigidBodyDetach();
                octree.ShapeRemove(shape);
                //boundsTree.Remove(shape);
            }

            // 清理连接

            // 移除刚体
            m_bodiesDict.Remove(body.RigidBodyId);
        }

        /// <summary>
        /// 射线检测
        /// </summary>
        /// <returns></returns>
        public RayCastResult RayCast()
        {
            // 实现射线检测逻辑
            // 返回 RayCastResult
            return new RayCastResult();
        }

        /// <summary>
        /// Shape
        /// </summary>
        /// <param name="shape"></param>
        public void ShapeAttach(Shape shape)
        {
            if (!m_shapesDict.ContainsKey(shape.m_shapeId))
            {
                shape.BBoxUpdate();
                m_shapesDict.Add(shape.m_shapeId, shape);
                octree.ShapeAdd(shape);
                //boundsTree.Add(shape, shape.WorldBoundingBox);
                EventOnShapeAttach?.Invoke(shape);
            }
        }
        public void ShapeDetach(Shape shape)
        {
            if (m_shapesDict.Remove(shape.m_shapeId))
            {
                octree.ShapeRemove(shape);
                //boundsTree.Remove(shape);
                EventOnShapeDetach.Invoke(shape);
            }
        }

        public void ShapeUpdate(Shape shape)
        {
            octree.ShapeUpdate(shape);
            //boundsTree.Remove(shape);
            //boundsTree.Add(shape, shape.WorldBoundingBox);
        }

        /// <summary>
        /// 获取Shape
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Shape ShapeGetById(ulong id)
        {
            return m_shapesDict[id];
        }
        #endregion

        #region 碰撞事件

        /// <summary>
        /// 碰撞事件
        /// </summary>
        public Action<CollisionResult> EventOnCollisionEnter;
        public Action<CollisionResult> EventOnCollisionStay;
        public Action<CollisionResult> EventOnCollisionExit;

        #endregion

        #region 私有方法

        /// <summary>
        /// 进行模拟
        /// </summary>
        private void SimulateStep()
        {
            //1.整合受力
            IntegrateForces();
            //2.解决碰撞和约束
            Resolve();
            //3.积分求解，更新状态
            RigidBodyTransformUpdate();
        }

        /// <summary>
        /// 碰撞检测
        /// </summary>
        private void CollisionDetection()
        {
            // 1.空间划分等方法进行粗筛选
            // 2.包围盒检测
            // 3.Shape对检测
            // 4.记录碰撞对并触发碰撞事件
            OctreeNodeCheck(octree.m_rootNode);
            //OctreeNodeCheck();
        }

        /// <summary>
        /// 清空碰撞
        /// </summary>
        private void ContactsClear()
        {
            m_collisionDataDict.Clear();
            foreach (var body in m_bodiesDict.Values)
            {
                body.Force = Vector3.zero;
            }
        }

        #endregion

        #region Shape事件

        public Action<Shape> EventOnShapeAttach;
        public Action<Shape> EventOnShapeDetach;

        #endregion

        #region 字段&属性

        /// <summary>
        /// 刚体列表
        /// </summary>
        private readonly Dictionary<ulong, RigidBody> m_bodiesDict = new();
        /// <summary>
        /// Shape列表
        /// </summary>
        private readonly Dictionary<ulong, Shape> m_shapesDict = new();

        /// <summary>
        /// 碰撞信息
        /// </summary>
        private Dictionary<(ulong, ulong), CollisionResult> m_collisionDataDict = new();

        /// <summary>
        /// 重力
        /// </summary>
        private Vector3 m_gravity = new(0, -9.81f * 0.1f, 0);
        public Vector3 Gravity
        {
            get => m_gravity;
            set => m_gravity = value;
        }

        /// <summary>
        /// 更新步长
        /// </summary>
        public const float StepDeltaTime = 1.0f / 100.0f;

        public Octree octree;

        /// <summary>
        /// idCounter
        /// </summary>
        private static ulong m_idCounter;

        #endregion
    }
}
