using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

[AddComponentMenu("Dynamic Bone/Dynamic Bone Fast")]
public class DynamicBoneFast : MonoBehaviour
{
    #region Particle & HeadInfo

    public struct Particle
    {
        public int m_ParentIndex;
        public int m_ChildCount;
        public float m_Damping;
        public float m_Elasticity;
        public float m_Stiffness;
        public float m_Inert;

        public float m_Radius;

        public float3 m_EndOffset;
        public float3 m_InitLocalPosition;
        public quaternion m_InitLocalRotation;

        // [ta]
        public int index;
        public float3 tmpWorldPosition;
        public float3 tmpPrevWorldPosition;
        public float3 localPosition;
        public quaternion localRotation;
        public float3 parentScale;
        public int isRootParticle;

        //for output
        public float3 worldPosition;
        public quaternion worldRotation;
    }

    // [ta]
    public struct HeadInfo
    {
        int m_HeadIndex;

        public float m_UpdateRate;
        public Vector3 m_ObjectMove;
        public int m_particleCount;
        public int m_jobDataOffset;
        public int m_ParticleLoopCount;

        public float3 m_RootParentBoneWorldPos;
        public quaternion m_RootParentBoneWorldRot;

        public int HeadIndex
        {
            get => m_HeadIndex;
            set => m_HeadIndex = value;
        }
    }

    #endregion

    public const int MAX_TRANSFORM_LIMIT = 10;

    public Transform m_Root;
    public float m_UpdateRate = 60.0f;
    [Range(0, 1)]
    public float m_Damping = 0.1f;
    [Range(0, 1)]
    public float m_Elasticity = 0.1f;
    [Range(0, 1)]
    public float m_Stiffness = 0.1f;
    [Range(0, 1)]
    public float m_Inert = 0;

    public float m_Radius = 0;
    
    public List<DynamicBoneCollider> m_Colliders = null;


    public NativeArray<Particle> m_Particles;
    public Transform[] m_particleTransformArr;
    private int m_ParticleCount;
    public Transform m_rootParentTransform;
    public HeadInfo m_headInfo;
    bool m_IsInited;


    public HeadInfo ResetHeadIndexAndDataOffset(int headIndex)
    {
        m_headInfo.HeadIndex = headIndex;
        m_headInfo.m_jobDataOffset = headIndex * MAX_TRANSFORM_LIMIT;

        return m_headInfo;
    }

    public void ClearJobData()
    {
        if (m_Particles.IsCreated)
        {
            m_Particles.Dispose();
        }

        m_particleTransformArr = null;
        m_IsInited = false;
    }

    void Init()
    {
        if (m_Root == null)
            return;

        m_headInfo = new HeadInfo();
        m_headInfo.m_UpdateRate = m_UpdateRate;
        m_headInfo.m_ObjectMove = Vector3.zero;
        m_headInfo.m_particleCount = 0;

        m_Particles = new NativeArray<Particle>(MAX_TRANSFORM_LIMIT, Allocator.Persistent);
        m_particleTransformArr = new Transform[MAX_TRANSFORM_LIMIT];
        m_ParticleCount = 0;

        SetupParticles();

        m_IsInited = true;
    }

    void Update()
    {
        if (!m_IsInited && m_Root)
        {
            Init();
            DynamicBoneFastManager.Instance.OnEnter(this);
        }
    }

    void OnDisable()
    {
        m_Root = null;
        DynamicBoneFastManager.Instance.OnExit(this);
    }

    void SetupParticles()
    {
        AppendParticles(m_Root, -1);
        UpdateParameters();

        m_headInfo.m_particleCount = m_ParticleCount;
        m_rootParentTransform = m_Root.parent;

        for (int i = 0; i < m_ParticleCount; i++)
        {
            m_particleTransformArr[i].parent = null;
        }
    }

    void AppendParticles(Transform b, int parentIndex)
    {
        var p = new Particle();
        p.index = m_ParticleCount++;
        p.m_ParentIndex = parentIndex;

        if (b != null)
        {
            p.tmpWorldPosition = p.tmpPrevWorldPosition = b.position;
            p.m_InitLocalPosition = b.localPosition;
            p.m_InitLocalRotation = b.localRotation;

            // [ta]
            p.localPosition = b.localPosition;
            p.localRotation = b.localRotation;
            p.parentScale = b.parent.lossyScale;
            p.isRootParticle = parentIndex == -1 ? 1 : 0;
        }
        else     // end bone
        {
            Transform pb = m_particleTransformArr[parentIndex];
            p.m_EndOffset = pb.InverseTransformPoint(transform.position + pb.position);
            p.tmpWorldPosition = p.tmpPrevWorldPosition = pb.TransformPoint(p.m_EndOffset);
            p.m_InitLocalPosition = Vector3.zero;
            p.m_InitLocalRotation = Quaternion.identity;
        }

        if (parentIndex >= 0)
        {
            ++p.m_ChildCount;
        }

        m_Particles[p.index] = p;
        m_particleTransformArr[p.index] = b;

        int index = p.index;

        if (b != null)
        {
            for (int i = 0; i < b.childCount; ++i)
            {
                Transform child = b.GetChild(i);
                AppendParticles(child, index);
            }
        }
    }

    void UpdateParameters()
    {
        for (int i = 0; i < m_ParticleCount; ++i)
        {
            Particle p = m_Particles[i];
            p.m_Damping = m_Damping;
            p.m_Elasticity = m_Elasticity;
            p.m_Stiffness = m_Stiffness;
            p.m_Inert = m_Inert;
            p.m_Damping = Mathf.Clamp01(p.m_Damping);
            p.m_Elasticity = Mathf.Clamp01(p.m_Elasticity);
            p.m_Stiffness = Mathf.Clamp01(p.m_Stiffness);
            p.m_Inert = Mathf.Clamp01(p.m_Inert);
            p.m_Radius = m_Radius;

            m_Particles[i] = p;
        }
    }
}