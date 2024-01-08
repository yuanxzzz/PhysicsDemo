using System;
using System.Collections.Generic;
using Assets.Script.Physics;
using Unity.VisualScripting;
using UnityEngine;

namespace PhysicsDemo
{
    public partial class World
    {
        /// <summary>
        /// 检测八叉树节点
        /// </summary>
        /// <param name="node"></param>
        private void OctreeNodeCheck(OctreeNode node)
        //private void OctreeNodeCheck()
        {
            //foreach (var shapeA in m_shapesDict.Values)
            //{
            //    List<Shape> collidingWith = new List<Shape>();
            //    boundsTree.GetColliding(collidingWith, shapeA.WorldBoundingBox);
            //    foreach (var shapeB in collidingWith)
            //    {
            //        //若是自己则直接跳过
            //        if (shapeA.m_shapeId == shapeB.m_shapeId)
            //        {
            //            continue;
            //        }

            //        // 检测包围盒
            //        if (shapeA.WorldBoundingBox.Intersects(shapeB.WorldBoundingBox))
            //        {
            //            // 细检测
            //            Detect(shapeA, shapeB);
            //        }
            //    }
            //}

            // 只用检测叶子节点
            if (node.m_childrenNode == null /*&& node.m_shapes.Count > 1*/)
            {
                foreach (var shapeA in node.m_shapes)
                {
                    foreach (var shapeB in node.m_shapes)
                    {
                        if (shapeA.m_shapeId == 305 || shapeA.m_shapeId == 383)
                        {
                            int a = 0;
                        }

                        // 若是自己则直接跳过
                        if (shapeA.m_shapeId == shapeB.m_shapeId)
                        {
                            continue;
                        }

                        // 检测包围盒
                        if (shapeA.WorldBoundingBox.Intersects(shapeB.WorldBoundingBox))
                        {
                            // 细检测
                            Detect(shapeA, shapeB);
                        }
                    }
                }
            }

            // 递归检查子节点
            if (node.m_childrenNode != null)
            {
                foreach (var childNode in node.m_childrenNode)
                {
                    OctreeNodeCheck(childNode);
                }
            }
        }

        /// <summary>
        /// 碰撞检测
        /// </summary>
        /// <param name="sA"></param>
        /// <param name="sB"></param>
        private void Detect(Shape sA, Shape sB)
        {
            if (sB.m_shapeId < sA.m_shapeId)
            {
                (sA, sB) = (sB, sA);
            }

            // 去除重复检测
            if (m_collisionDataDict.ContainsKey((sA.m_shapeId, sB.m_shapeId)))
            {
                return;
            }

            // 非活跃物体不用检测
            if (!sA.RigidBody.IsActive && !sB.RigidBody.IsActive) return;
            // 静态物体间没碰撞
            if (sA.RigidBody.IsStatic && sB.RigidBody.IsStatic) return;

            bool colliding = CollisionHelper.Detect(sA, sB, out var result);
            if (colliding)
            {
                // 触发事件
                EventOnCollisionEnter?.Invoke(result);
            }
            // 记录碰撞检测结果
            m_collisionDataDict.Add((sA.m_shapeId, sB.m_shapeId), result);
        }
    }
}


