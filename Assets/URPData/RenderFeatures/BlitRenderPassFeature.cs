using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlitRenderPassFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class BlitSettings
    {
        public RenderPassEvent PassEvent = RenderPassEvent.AfterRenderingOpaques;
        public Material blitMaterial = null;
        public int blitMaterialPassIndex = 0;
        public bool setInverseViewMatrix = false;
        public bool requireDepthNormals = false;

        public Target srcType = Target.CameraColor;
        public string srcTextureId = "_CameraColorTexture";
        public RenderTexture srcTextureObject;

        public Target dstType = Target.CameraColor;
        public string dstTextureId = "_BlitPassTexture";
        public RenderTexture dstTextureObject;

        public bool overrideGraphicsFormat = false;
        public GraphicsFormat graphicsFormat;
    }

    public BlitSettings settings = new BlitSettings();
    
    public enum Target
    {
        CameraColor,
        TextureID,
        RenderTextureObject
    }
    
    class BlitPass : ScriptableRenderPass
    {
        public Material blitMaterial = null;
        public FilterMode filterMode { get; set; }
        
        private BlitSettings settings;
        
        private RTHandle source { get; set; }
        private RTHandle destination { get; set; }

        RTHandle m_TemporaryColorTexture;
        private RenderTexture m_renderTexture;
        RTHandle m_DestinationTexture;
        string m_ProfilerTag;
        public BlitPass(RenderPassEvent renderPassEvent, BlitSettings settings, string tag)
        {
            this.renderPassEvent = renderPassEvent;
            this.settings = settings;
            blitMaterial = settings.blitMaterial;
            m_ProfilerTag = tag;
           
        }
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            RenderingUtils.ReAllocateHandleIfNeeded(ref m_TemporaryColorTexture, opaqueDesc, FilterMode.Bilinear,
                TextureWrapMode.Clamp, name: "_TemporaryColorTexture");
            if (settings.dstType == Target.TextureID)
            {
                RenderingUtils.ReAllocateHandleIfNeeded(ref m_DestinationTexture, opaqueDesc, FilterMode.Bilinear,
                    TextureWrapMode.Clamp, name: settings.dstTextureId);
            }
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            var renderer = renderingData.cameraData.renderer;

            if (settings.srcType == Target.CameraColor)
            {
                // cmd.GenerateMips(renderer.cameraColorTarget);
                source = renderer.cameraColorTargetHandle;
            }else if (settings.srcType == Target.TextureID)
            {
                source = RTHandles.Alloc(settings.srcTextureId);
            }else if (settings.srcType == Target.RenderTextureObject)
            {
                source = RTHandles.Alloc(settings.srcTextureObject);
            }

            if (settings.dstType == Target.CameraColor)
            {
                destination = renderer.cameraColorTargetHandle;
            }else if (settings.dstType == Target.TextureID)
            {
                destination = RTHandles.Alloc(settings.dstTextureId);
            }else if (settings.dstType == Target.RenderTextureObject)
            {
                destination = RTHandles.Alloc(settings.dstTextureObject);
            }

            if (settings.setInverseViewMatrix)
            {
                Shader.SetGlobalMatrix("_InverseView", renderingData.cameraData.camera.cameraToWorldMatrix);
            }

            if (settings.dstType == Target.TextureID)
            {
                if (settings.overrideGraphicsFormat)
                {
                    opaqueDesc.graphicsFormat = settings.graphicsFormat;
                }
                RenderingUtils.ReAllocateHandleIfNeeded(ref m_DestinationTexture, opaqueDesc, filterMode,
                    TextureWrapMode.Clamp, name: settings.dstTextureId);
                // cmd.GetTemporaryRT(m_DestinationTexture.id, opaqueDesc, filterMode);
            }
            
            if (source == destination || (settings.srcType == settings.dstType && settings.srcType == Target.CameraColor))
            {
                RenderingUtils.ReAllocateHandleIfNeeded(ref m_TemporaryColorTexture, opaqueDesc, filterMode,
                    TextureWrapMode.Clamp, name: "_TemporaryColorTexture");
                // cmd.GetTemporaryRT(m_TemporaryColorTexture.id, opaqueDesc, filterMode);
                Blitter.BlitCameraTexture(cmd, source, m_TemporaryColorTexture, blitMaterial,settings.blitMaterialPassIndex);
                Blitter.BlitCameraTexture(cmd, m_TemporaryColorTexture, destination);
            }
            else
            {
                Blitter.BlitCameraTexture(cmd,source,destination,blitMaterial,settings.blitMaterialPassIndex);
            }
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (settings.dstType == Target.TextureID)
            {
                // cmd.ReleaseTemporaryRT(m_DestinationTexture.id);
                RTHandles.Release(m_DestinationTexture);
            }
            
            if (source == destination ||
                (settings.srcType == settings.dstType && settings.srcType == Target.CameraColor))
            {
                // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);
                RTHandles.Release(m_TemporaryColorTexture);
            }
        }

        public void Setup()
        {
            if (settings.requireDepthNormals)
            {
                ConfigureInput(ScriptableRenderPassInput.Normal);
            }
        }
    }

    BlitPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        var passIndex = settings.blitMaterial != null ? settings.blitMaterial.passCount - 1 : 1;
        settings.blitMaterialPassIndex = Mathf.Clamp(settings.blitMaterialPassIndex, -1, passIndex);
        m_ScriptablePass = new BlitPass(settings.PassEvent, settings, name);

        if (settings.graphicsFormat == GraphicsFormat.None)
        {
            settings.graphicsFormat = SystemInfo.GetGraphicsFormat(DefaultFormat.LDR);
        }

        // Configures where the render pass should be injected.
        // m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.blitMaterial == null)
        {
            Debug.LogErrorFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in assigned renderer.", GetType().Name);
        }
        
        m_ScriptablePass.Setup();
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


