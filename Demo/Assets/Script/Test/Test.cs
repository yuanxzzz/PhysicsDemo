﻿using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PhysicsDemo
{
    public class Test : MonoBehaviour
    {
        private void Start()
        {
            Random.InitState(123);

            m_world = new World();
            m_world.Initialize();
            m_world.EventOnShapeAttach += OnShapeAttach;
            m_world.EventOnShapeDetach += OnShapeAttach;
            // 测试：碰撞则将两个物体设为红色
            m_world.EventOnCollisionEnter += data =>
            {
                var rigdA = data.m_body1;
                var rigdB = data.m_body2;
                Color color = Color.red;
                foreach (var shape in rigdA.Shapes)
                {
                    if (m_gameObjectControllerDict.TryGetValue(shape.m_shapeId, out var ctor))
                    {
                        ctor.gameObject.GetComponent<MeshRenderer>().material.color = color;
                    }
                }

                foreach (var shape in rigdB.Shapes)
                {
                    if (m_gameObjectControllerDict.TryGetValue(shape.m_shapeId, out var ctor))
                    {
                        ctor.gameObject.GetComponent<MeshRenderer>().material.color = color;
                    }
                }
            };

            // 随机300个物体
            for (int i = 0; i < 300; i++)
            {
                var position = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f),
                    Random.Range(-10f, 10f));
                RigidBody rig = m_world.RigidBodyCreate();
                rig.Position = position;
                rig.ShapeAdd(new SphereShape(0.5f));
                rig.AffectedByGravity = false;
            }
        }

        private void Update()
        {
            // 累加经过的时间
            m_elapsedTime += Time.deltaTime;

            // 判断是否需要进行固定帧率的物理更新
            while (m_elapsedTime >= World.StepDeltaTime)
            {
                //Update4Input();

                // 进行物理更新
                m_world.Update();

                Update4View();

                // 减去已经模拟的时间
                m_elapsedTime -= World.StepDeltaTime;
            }
        }

        /// <summary>
        /// 显示层更新
        /// </summary>
        private void Update4View()
        {
            foreach (var pair in m_gameObjectControllerDict)
            {
                var shapeId = pair.Key;
                var shape = m_world.ShapeGetById(shapeId);
                if (shape == null)
                {
                    m_gameObjectControllerDict.Remove(shape.m_shapeId);
                    return;
                }
                var controller = pair.Value;
                controller.TargetTransformSet(shape.RigidBody.Position, shape.RigidBody.Orientation.rotation);
            }
        }

        /// <summary>
        /// 添加和移除物体
        /// </summary>
        /// <param name="shape"></param>
        private void OnShapeAttach(Shape shape)
        {
            GameObject go;
            switch (shape)
            {
                case SphereShape:
                    go = Instantiate(m_spherePrefab);
                    go.name = shape.m_shapeId.ToString();
                    break;
                default:
                    return;
            }
            var ctor = go.AddComponent<GameObjectController>();
            if (m_gameObjectControllerDict.TryAdd(shape.m_shapeId, ctor))
            {
                ctor.Initialize(shape.RigidBody.Position, shape.RigidBody.Orientation.rotation);
            }
        }
        private void OnShapeDetach(Shape shape)
        {
            if (m_gameObjectControllerDict.TryGetValue(shape.m_shapeId, out var ctrl))
            {
                Destroy(ctrl);
                m_gameObjectControllerDict.Remove(shape.m_shapeId);
            }
        }

        /// <summary>
        /// 画出八叉树边界
        /// </summary>
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                m_world.octree.m_rootNode.Draw();
            }
        }

        /// <summary>
        /// controller字典
        /// </summary>
        private Dictionary<ulong, GameObjectController> m_gameObjectControllerDict = new();

        /// <summary>
        /// 计时器
        /// </summary>
        private float m_elapsedTime = 0f;

        World m_world;


        [SerializeField]
        private GameObject m_spherePrefab;
        [SerializeField]
        private GameObject m_cubePrefab;
        [SerializeField]
        private GameObject m_planePrefab;

        private float fixedTimeStep;
    }
}
