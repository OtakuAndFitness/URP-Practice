using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomPostProcessingPassFeature : ScriptableRendererFeature
{
    class CustomPostProcessingPass : ScriptableRenderPass
    {
        private RenderTargetIdentifier m_ColorAttachment;
        private RenderTargetIdentifier m_CameraDepthAttachment;
        private RenderTargetHandle m_Destination;
        
        const string k_RenderPostProcessingTag = "Render AdditionalPostProcessing Effects";
        const string k_RenderFinalPostProcessingTag = "Render Final AdditionalPostProcessing Pass";

        private GaussianBlur m_GaussianBlur;
        private MaterialLibrary m_Materials;
        private CustomPostProcessingData m_Data;
        
        RenderTargetHandle m_TemporaryColorTexture01;
        RenderTargetHandle m_TemporaryColorTexture02;
        RenderTargetHandle m_TemporaryColorTexture03;

        public CustomPostProcessingPass(CustomPostProcessingData data)
        {
            m_TemporaryColorTexture01.Init("m_TemporaryColorTexture01");
            m_TemporaryColorTexture02.Init("m_TemporaryColorTexture02");
            m_TemporaryColorTexture03.Init("m_TemporaryColorTexture03");
            m_Data = data;
            m_Materials = new MaterialLibrary(m_Data);
        }
        
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var stack = VolumeManager.instance.stack;
            m_GaussianBlur = stack.GetComponent<GaussianBlur>();
            var cmd = CommandBufferPool.Get(k_RenderFinalPostProcessingTag);
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        private void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;
            if (m_GaussianBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupGaussianBlur(cmd, ref renderingData, m_Materials.gaussianBlur);
            }
            
        }

        private void SetupGaussianBlur(CommandBuffer cmd, ref RenderingData renderingData, Material gaussianBlur)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = opaqueDesc.width >> m_GaussianBlur.downSample.value;
            opaqueDesc.height = opaqueDesc.height >> m_GaussianBlur.downSample.value;
            opaqueDesc.depthBufferBits = 0;
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_GaussianBlur.filterMode.value);
            cmd.GetTemporaryRT(m_TemporaryColorTexture02.id, opaqueDesc, m_GaussianBlur.filterMode.value);
            cmd.GetTemporaryRT(m_TemporaryColorTexture03.id, opaqueDesc, m_GaussianBlur.filterMode.value);
            cmd.BeginSample("GaussianBlur");
            cmd.Blit(this.m_ColorAttachment, m_TemporaryColorTexture03.Identifier());
            for (int i = 0; i < m_GaussianBlur.blurCount.value; i++) {
                gaussianBlur.SetVector("_Offset", new Vector4(0, m_GaussianBlur.indensity.value, 0, 0));
                cmd.Blit(m_TemporaryColorTexture03.Identifier(), m_TemporaryColorTexture01.Identifier(), gaussianBlur);
                gaussianBlur.SetVector("_Offset", new Vector4(m_GaussianBlur.indensity.value, 0, 0, 0));
                cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_TemporaryColorTexture02.Identifier(), gaussianBlur);
                cmd.Blit(m_TemporaryColorTexture02.Identifier(), m_TemporaryColorTexture03.Identifier());
            }
            cmd.Blit(m_TemporaryColorTexture03.Identifier(), m_ColorAttachment);
            cmd.EndSample("GaussianBlur");
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }

        public void Setup(RenderPassEvent @event, RenderTargetIdentifier cameraColorTarget, RenderTargetIdentifier cameraDepth, RenderTargetHandle dest)
        {
            renderPassEvent = @event;
            m_ColorAttachment = cameraColorTarget;
            m_CameraDepthAttachment = cameraDepth;
            m_Destination = dest;
            // m_Materials = new MaterialLibrary(data);
        }
    }

    CustomPostProcessingPass m_ScriptablePass;
    public RenderPassEvent evt = RenderPassEvent.AfterRenderingTransparents;
    public CustomPostProcessingData m_CustomPostProcessingData;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomPostProcessingPass(m_CustomPostProcessingData);

        // Configures where the render pass should be injected.
        // m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (m_CustomPostProcessingData == null)
        {
            Debug.LogError("There is no Custom Post Processing Data assigned!!!");
            return;
        }
        
        var cameraColorTarget = renderer.cameraColorTarget;
        var cameraDepth = renderer.cameraDepthTarget;
        var dest = RenderTargetHandle.CameraTarget;

        m_ScriptablePass.Setup(evt, cameraColorTarget, cameraDepth, dest);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


