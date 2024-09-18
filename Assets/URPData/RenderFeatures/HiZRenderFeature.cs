using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HiZRenderFeature : ScriptableRendererFeature
{
    class HiZRenderPass : ScriptableRenderPass
    {
        private HiZSettings _hiZSettings;
        private RenderTextureDescriptor[] _hiZBufferTempRTDescriptors;
        private RTHandle[] _hiZBufferTempRT;
        private RenderTextureDescriptor _hiZBufferDescriptor;
        private RTHandle _hiZBuffer;
        private Material _material;
        private RTHandle _cameraDepthRTHandle;
        private ProfilingSampler _profilingSampler = new ProfilingSampler("HiZ");

        private static readonly int
            maxHiZBufferMipLevelID = Shader.PropertyToID("_MaxHiZBufferMipLevel"),
            hiZBufferTextureID = Shader.PropertyToID("_HiZBuffer");
            
        public HiZRenderPass(HiZSettings hiZSettings)
        {
            _hiZSettings = hiZSettings;
            _hiZBufferTempRTDescriptors = new RenderTextureDescriptor[hiZSettings.mipCount];
            _hiZBufferTempRT = new RTHandle[hiZSettings.mipCount];

        }
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var renderer = renderingData.cameraData.renderer;
            var desc = renderingData.cameraData.cameraTargetDescriptor;

            var width = Math.Max((int)Math.Ceiling(Mathf.Log(desc.width, 2)), 1);
            var height = Math.Max((int)Math.Ceiling(Mathf.Log(desc.height, 2)), 1);
            width = 1 << width;
            height = 1 << height;

            _hiZBufferDescriptor =
                new RenderTextureDescriptor(width, height, RenderTextureFormat.RFloat, 0, _hiZSettings.mipCount);
            _hiZBufferDescriptor.msaaSamples = 1;
            _hiZBufferDescriptor.useMipMap = true;
            _hiZBufferDescriptor.sRGB = false;

            for (int i = 0; i < _hiZSettings.mipCount; i++)
            {
                _hiZBufferTempRTDescriptors[i] = new RenderTextureDescriptor(width, height, RenderTextureFormat.RFloat);
                _hiZBufferTempRTDescriptors[i].msaaSamples = 1;
                _hiZBufferTempRTDescriptors[i].useMipMap = false;
                _hiZBufferTempRTDescriptors[i].sRGB = false;
                width = Math.Max(width / 2, 1);
                height = Math.Max(height / 2, 1);
                RenderingUtils.ReAllocateIfNeeded(ref _hiZBufferTempRT[i], _hiZBufferTempRTDescriptors[i]);
            }

            RenderingUtils.ReAllocateIfNeeded(ref _hiZBuffer, _hiZBufferDescriptor);
            _material = new Material(Shader.Find("Hidden/HiZ"));
            
            ConfigureTarget(renderer.cameraColorTargetHandle);
            ConfigureClear(ClearFlag.None, Color.white);
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_material == null)
            {
                Debug.LogErrorFormat("{0}.Execute(): Missing material", GetType().Name);
                return;
            }

            _cameraDepthRTHandle = renderingData.cameraData.renderer.cameraDepthTargetHandle;
            
            var cmd = CommandBufferPool.Get();
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            using (new ProfilingScope(cmd, _profilingSampler))
            {
                Blitter.BlitCameraTexture(cmd, _cameraDepthRTHandle, _hiZBufferTempRT[0]);
                cmd.CopyTexture(_hiZBufferTempRT[0], 0, 0, _hiZBuffer, 0, 0);

                for (int i = 1; i < _hiZSettings.mipCount; i++)
                {
                    Blitter.BlitCameraTexture(cmd, _hiZBufferTempRT[i - 1], _hiZBufferTempRT[i], _material, 0);
                    cmd.CopyTexture(_hiZBufferTempRT[i], 0, 0, _hiZBuffer, 0, i);
                }
                
                cmd.SetGlobalFloat(maxHiZBufferMipLevelID, _hiZSettings.mipCount - 1);
                cmd.SetGlobalTexture(hiZBufferTextureID, _hiZBuffer);
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
            _hiZBuffer?.Release();
            foreach (var tempRT in _hiZBufferTempRT)
            {
                tempRT?.Release();
            }
        }
    }

    HiZRenderPass m_ScriptablePass;
    [SerializeField]
    private HiZSettings _hiZSettings;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new HiZRenderPass(_hiZSettings);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = _hiZSettings.renderPassEvent;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }

    protected override void Dispose(bool disposing)
    {
        m_ScriptablePass.Dispose();
    }
}

[Serializable]
public class HiZSettings
{
    //在1080P以下最佳设置为3
    //在1080P左右最佳设置为4
    //在4K左右最佳设置为5
    [Range(3, 6)] public int mipCount = 6;
    public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingGbuffer;
}


