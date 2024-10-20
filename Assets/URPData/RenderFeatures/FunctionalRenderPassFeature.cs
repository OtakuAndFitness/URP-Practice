using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FunctionalRenderPassFeature : ScriptableRendererFeature
{

    [Serializable]
    public class ViewSettings
    {
        public BufferType buffer = BufferType.Depth;
    }

    public enum BufferType
    {
        Depth,
        WorldPostion,
        Opaque
    }

    public ViewSettings settings = new ViewSettings();
    
    class FunctionalRenderPass : ScriptableRenderPass
    {
        private Material blitMaterial = null;

        public int blitShaderPassIndex = 0;
        private RTHandle source { set; get; }
        private RTHandle destination { set; get; }

        RTHandle m_TemporaryColorTexture;

         string m_ProfilerTag;

        public FunctionalRenderPass(Material mat, string tag)
        {
            blitMaterial = mat;
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
            
            RenderingUtils.ReAllocateHandleIfNeeded(ref m_TemporaryColorTexture, opaqueDesc, FilterMode.Bilinear, TextureWrapMode.Clamp, name:"_TemporaryColorTexture");

        }

        public void Setup(RTHandle source, RTHandle destination)
        {
            this.source = source;
            this.destination = destination;
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
            // cmd.GetTemporaryRT(Shader.PropertyToID(m_TemporaryColorTexture.name), opaqueDesc, FilterMode.Bilinear);
            Blitter.BlitCameraTexture(cmd, source, m_TemporaryColorTexture, blitMaterial, blitShaderPassIndex);
            Blitter.BlitCameraTexture(cmd, m_TemporaryColorTexture,source);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // cmd.ReleaseTemporaryRT(Shader.PropertyToID(m_TemporaryColorTexture.name));
            if (m_TemporaryColorTexture != null)
            {
                RTHandles.Release(m_TemporaryColorTexture);
                m_TemporaryColorTexture = null;
            }
        }
    }

    FunctionalRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new FunctionalRenderPass(CoreUtils.CreateEngineMaterial("Otaku/FunctionalShader"), "FunctionalShader");

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        m_ScriptablePass.blitShaderPassIndex = (int) settings.buffer;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ScriptablePass.Setup(renderer.cameraColorTargetHandle, RTHandles.Alloc(BuiltinRenderTextureType.CameraTarget));
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


