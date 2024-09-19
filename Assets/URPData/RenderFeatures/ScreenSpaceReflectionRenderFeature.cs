using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenSpaceReflectionRenderFeature : ScriptableRendererFeature
{
    [Serializable]
    public class SSRSettings
    {
        [Range(0f,1f)] public float minimumSmoothness = 0.5f;

        [Range(0f, 1f)] public float dithering = 0f;

        [Range(0.1f, 1f)] public float objectThickness = 0.1f;

        [Range(1, 8)] public int stride = 8;

        [Range(16, 128)] public int maxRaySteps = 128;

        [Range(0f, 0.001f)] public float blurRadius = 0.0008f;

    }
    class ScreenSpaceReflectionRenderPass : ScriptableRenderPass
    {
        private Shader _shader;
        private Material _material;
        private RenderTextureDescriptor _ssrRTDescriptor;
        private RTHandle _ssrRTHandle, _ssr1RTHandle;
        private ProfilingSampler _profilingSampler = new ProfilingSampler("SSR");
        private SSRSettings _ssrSettings;

        private static readonly int cameraParamsID = Shader.PropertyToID("_CameraProjectionParams"),
            cameraViewTopLeftCornerID = Shader.PropertyToID("_CameraViewTopLeftCorner"),
            cameraViewXExtentID = Shader.PropertyToID("_CameraViewXExtent"),
            cameraViewYExtentID = Shader.PropertyToID("_CameraViewYExtent"),
            sourceSizeID = Shader.PropertyToID("_SourceSize"),
            minSmoothnessID = Shader.PropertyToID("_MinSmoothness"),
            ditheringID = Shader.PropertyToID("_Dithering"),
            objectThicknessID = Shader.PropertyToID("_ObjectThickness"),
            maxRayStepsID = Shader.PropertyToID("_MaxRaySteps"),
            strideID = Shader.PropertyToID("_Stride"),
            blurRadiusID = Shader.PropertyToID("_BlurRadius");

            
        public ScreenSpaceReflectionRenderPass(Shader shader, SSRSettings ssrSettings)
        {
            _shader = shader;
            _ssrSettings = ssrSettings;
            _ssrRTDescriptor =
                new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.RGB111110Float, 0);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            _ssrRTDescriptor.width = cameraTextureDescriptor.width;
            _ssrRTDescriptor.height = cameraTextureDescriptor.height;
            RenderingUtils.ReAllocateIfNeeded(ref _ssrRTHandle, _ssrRTDescriptor, FilterMode.Bilinear,
                TextureWrapMode.Mirror);
            RenderingUtils.ReAllocateIfNeeded(ref _ssr1RTHandle, _ssrRTDescriptor, FilterMode.Bilinear,
                TextureWrapMode.Mirror);

        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var camData = renderingData.cameraData;

            Matrix4x4 view = camData.GetViewMatrix();
            Matrix4x4 proj = camData.GetProjectionMatrix();

            // 将camera view space 的平移置为0，用来计算world space下相对于相机的vector
            Matrix4x4 cameraViewSpaceMatrix = view;
            cameraViewSpaceMatrix.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));
            Matrix4x4 vp = proj * cameraViewSpaceMatrix;

            // 计算viewProj逆矩阵，即从裁剪空间变换到世界空间
            Matrix4x4 inverseVP = vp.inverse;

            // 计算世界空间下，近平面四个角的坐标
            Vector4 topLeftCorner = inverseVP.MultiplyPoint(new Vector4(-1.0f, 1.0f, -1.0f, 1.0f));
            Vector4 topRightCorner = inverseVP.MultiplyPoint(new Vector4(1.0f, 1.0f, -1.0f, 1.0f));
            Vector4 bottomLeftCorner = inverseVP.MultiplyPoint(new Vector4(-1.0f, -1.0f, -1.0f, 1.0f));

            // 计算相机近平面上方向向量
            Vector4 cameraXExtent = topRightCorner - topLeftCorner;
            Vector4 cameraYExtent = bottomLeftCorner - topLeftCorner;

            float near = camData.camera.nearClipPlane;
            if (_material is null)
            {
                _material = new Material(_shader);
            }
            _material.SetVector(cameraViewTopLeftCornerID, topLeftCorner);
            _material.SetVector(cameraViewXExtentID, cameraXExtent);
            _material.SetVector(cameraViewYExtentID, cameraYExtent);
            _material.SetVector(cameraParamsID, new Vector4(1.0f / near, 0, 0, 0));
            _material.SetVector(sourceSizeID, new Vector4(_ssrRTDescriptor.width, _ssrRTDescriptor.height, 1.0f / _ssrRTDescriptor.width, 1.0f / _ssrRTDescriptor.height));
            _material.SetFloat(minSmoothnessID, _ssrSettings.minimumSmoothness);
            _material.SetFloat(ditheringID, _ssrSettings.dithering);
            _material.SetFloat(objectThicknessID, _ssrSettings.objectThickness);
            _material.SetFloat(maxRayStepsID, _ssrSettings.maxRaySteps);
            _material.SetFloat(strideID, _ssrSettings.stride);
            
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cameraTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
            
            var cmd = CommandBufferPool.Get();
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                if (cameraTargetHandle != null && _ssrRTHandle != null && _material != null)
                {
                    Blitter.BlitCameraTexture(cmd, cameraTargetHandle, _ssrRTHandle, _material, 0);
                    
                    // Horizontal Blur
                    cmd.SetGlobalFloat(blurRadiusID, _ssrSettings.blurRadius);
                    Blitter.BlitCameraTexture(cmd, _ssrRTHandle, _ssr1RTHandle, _material, 1);
                    
                    // Vertical Blur
                    cmd.SetGlobalFloat(blurRadiusID, _ssrSettings.blurRadius);
                    Blitter.BlitCameraTexture(cmd, _ssr1RTHandle, _ssrRTHandle, _material, 2);
                    
                    Blitter.BlitCameraTexture(cmd, _ssrRTHandle, cameraTargetHandle, _material, 3);
                }
            }
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            
        }

        public void Dispose()
        {
            _ssrRTHandle?.Release();
            _ssr1RTHandle?.Release();
        }
    }

    ScreenSpaceReflectionRenderPass m_ScriptablePass;
    
    private Material _material;
    private Shader _shader;
    private const string _shaderName = "Hidden/SSR";
    [SerializeField]
    private RenderPassEvent _renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

    [SerializeField] private SSRSettings _ssrSettings = new SSRSettings();

    /// <inheritdoc/>
    public override void Create()
    {
        _shader ??= Shader.Find(_shaderName);
        m_ScriptablePass = new ScreenSpaceReflectionRenderPass(_shader, _ssrSettings)
        {
            renderPassEvent = _renderPassEvent
        };
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (m_ScriptablePass != null)
        {
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        m_ScriptablePass?.Dispose();
        m_ScriptablePass = null;
    }
}


