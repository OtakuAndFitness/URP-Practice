using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DistortionMaskRenderPassFeature : ScriptableRendererFeature
{
    [Serializable]
    public class RenderMaskSetting
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        public LayerMask layerMask;
        public Material material;
        public int RTWidth;
        public int RTHeight;
    }
    
    class DistortionMask : ScriptableRenderPass
    {
        // private int m_MaskTexID = 0;
    
        private ShaderTagId m_ShaderTag = new ShaderTagId("UniversalForward");
    
        private RenderMaskSetting m_RenderMaskSetting;
    
        private FilteringSettings m_FilteringSettings;
    
        public DistortionMask(RenderMaskSetting setting)
        {
            m_RenderMaskSetting = setting;
            m_FilteringSettings = new FilteringSettings(RenderQueueRange.all, m_RenderMaskSetting.layerMask);
        }
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }
    
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            int temp = Shader.PropertyToID("_MaskTex");
            // m_MaskTexID = temp;
            RenderTextureDescriptor desc = new RenderTextureDescriptor(m_RenderMaskSetting.RTWidth, m_RenderMaskSetting.RTHeight);
            cmd.GetTemporaryRT(temp, desc);
            ConfigureTarget(temp);
            ConfigureClear(ClearFlag.All, Color.black);
        }
    
        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var drawMask = CreateDrawingSettings(m_ShaderTag, ref renderingData,
                renderingData.cameraData.defaultOpaqueSortFlags);
            drawMask.overrideMaterial = m_RenderMaskSetting.material;
            drawMask.overrideMaterialPassIndex = 0;
            context.DrawRenderers(renderingData.cullResults, ref drawMask, ref m_FilteringSettings);
        }
    
        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }
    
    public RenderMaskSetting renderMaskSetting;
    DistortionMask m_ScriptablePass;
    
    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new DistortionMask(renderMaskSetting);
        m_ScriptablePass.renderPassEvent = renderMaskSetting.renderPassEvent;
    
        // Configures where the render pass should be injected.
        // m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }
    
    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderMaskSetting.material == null)
        {
            Debug.LogErrorFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in assigned renderer.", GetType().Name);
        }
        renderer.EnqueuePass(m_ScriptablePass);
    }
    
}


