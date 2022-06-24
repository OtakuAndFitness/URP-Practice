using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomPostProcessingPassFeature : ScriptableRendererFeature
{
    class CustomPostProcessingPass : ScriptableRenderPass
    {
        private RenderTargetIdentifier m_ColorAttachment;
        // private RenderTargetIdentifier m_CameraDepthAttachment;
        // private RenderTargetHandle m_Destination;
        
        // const string k_RenderPostProcessingTag = "Render AdditionalPostProcessing Effects";
        const string k_RenderCustomPostProcessingTag = "Render Custom PostProcessing Pass";

        private GaussianBlur m_GaussianBlur;
        private BoxBlur m_BoxBlur;
        private KawaseBlur m_KawaseBlur;
        private DualKawaseBlur m_DualKawaseBlur;
        private BokehBlur m_BokehBlur;
        private TiltShiftBlur m_TiltShiftBlur;
        private IrisBlur m_IrisBlur;
        private GrainyBlur m_GrainyBlur;
        private RadialBlur m_RadialBlur;
        private DirectionalBlur m_DirectionalBlur;
        
        private MaterialLibrary m_Materials;
        private CustomPostProcessingData m_Data;
        
        RenderTargetHandle m_TemporaryColorTexture01;
        RenderTargetHandle m_TemporaryColorTexture02;

        private Level[] m_Pyramid;
        const int k_MaxPyramidSize = 16;

        private Vector4 mGoldenRot;
        
        struct Level
        {
            internal int down;
            internal int up;
        }
        // RenderTargetHandle m_TemporaryColorTexture03;

        public CustomPostProcessingPass(CustomPostProcessingData data)
        {
            m_Data = data;
            m_Materials = new MaterialLibrary(m_Data);
            
            //for blur
            m_TemporaryColorTexture01.Init("m_TemporaryColorTexture01");
            m_TemporaryColorTexture02.Init("m_TemporaryColorTexture02");
            
            //for dual blur
            m_Pyramid = new Level[k_MaxPyramidSize];
            for (int i = 0; i < k_MaxPyramidSize; i++)
            {
                m_Pyramid[i] = new Level
                {
                    down = Shader.PropertyToID("_BlurMipDown" + i),
                    up = Shader.PropertyToID("_BlurMipUp" + i)
                };
            }

            mGoldenRot = new Vector4();
            // Precompute rotations
            float c = Mathf.Cos(2.39996323f);
            float s = Mathf.Sin(2.39996323f);
            mGoldenRot.Set(c, s, -s, c);
            // m_TemporaryColorTexture03.Init("m_TemporaryColorTexture03");
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
            m_BoxBlur = stack.GetComponent<BoxBlur>();
            m_KawaseBlur = stack.GetComponent<KawaseBlur>();
            m_DualKawaseBlur = stack.GetComponent<DualKawaseBlur>();
            m_BokehBlur = stack.GetComponent<BokehBlur>();
            m_TiltShiftBlur = stack.GetComponent<TiltShiftBlur>();
            m_IrisBlur = stack.GetComponent<IrisBlur>();
            m_GrainyBlur = stack.GetComponent<GrainyBlur>();
            m_RadialBlur = stack.GetComponent<RadialBlur>();
            m_DirectionalBlur = stack.GetComponent<DirectionalBlur>();
            var cmd = CommandBufferPool.Get(k_RenderCustomPostProcessingTag);
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

            if (m_BoxBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupBoxBlur(cmd, ref renderingData, m_Materials.boxBlur);
            }

            if (m_KawaseBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupKawaseBlur(cmd, ref renderingData, m_Materials.kawaseBlur);
            }

            if (m_DualKawaseBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupDualKawaseBlur(cmd, ref renderingData, m_Materials.dualKawaseBlur);
            }

            if (m_BokehBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupBokehBlur(cmd, ref renderingData, m_Materials.bokehBlur);
            }

            if (m_TiltShiftBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupTiltShiftBlur(cmd, ref renderingData, m_Materials.tiltShiftBlur);
            }

            if (m_IrisBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupIrisBlur(cmd, ref renderingData, m_Materials.irisBlur);
            }

            if (m_GrainyBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupGrainyBlur(cmd, ref renderingData, m_Materials.grainyBlur);
            }

            if (m_RadialBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupRadialBlur(cmd, ref renderingData, m_Materials.radialBlur);
            }

            if (m_DirectionalBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupDirectionalBlur(cmd, ref renderingData, m_Materials.directionalBlur);
            }
            
            // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture02.id);
        }

        #region DirectionalBlur

        private void SetupDirectionalBlur(CommandBuffer cmd, ref RenderingData renderingData, Material directionalBlur)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = opaqueDesc.width >> m_DirectionalBlur.downSample.value;
            opaqueDesc.height = opaqueDesc.height >> m_DirectionalBlur.downSample.value;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("DirectionalBlur");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_DirectionalBlur.filterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            float sinVal = (Mathf.Sin(m_DirectionalBlur.angle.value) * m_DirectionalBlur.indensity.value * 0.05f) / m_DirectionalBlur.blurCount.value;
            float cosVal = (Mathf.Cos(m_DirectionalBlur.angle.value) * m_DirectionalBlur.indensity.value * 0.05f) / m_DirectionalBlur.blurCount.value;  
            directionalBlur.SetVector("_Params", new Vector3(m_DirectionalBlur.blurCount.value, sinVal, cosVal));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, directionalBlur);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("DirectionalBlur");
            
        }

        #endregion
        

        #region RadialBlur

        private void SetupRadialBlur(CommandBuffer cmd, ref RenderingData renderingData, Material radialBlur)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("RadialBlur");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_RadialBlur.filterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            radialBlur.SetVector("_Params", new Vector3(m_RadialBlur.indensity.value * 0.02f, m_RadialBlur.RadialCenterX.value, m_RadialBlur.RadialCenterY.value));
            int pass = (int)m_RadialBlur.qualityLevel.value;
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, radialBlur, pass);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("RadialBlur");
            
        }

        #endregion
        
        #region GrainyBlur

        private void SetupGrainyBlur(CommandBuffer cmd, ref RenderingData renderingData, Material grainyBlur)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = opaqueDesc.width >> m_GrainyBlur.downSample.value;
            opaqueDesc.height = opaqueDesc.height >> m_GrainyBlur.downSample.value;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("GrainyBlur");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_GrainyBlur.filterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            grainyBlur.SetVector("_Params", new Vector2(m_GrainyBlur.indensity.value / opaqueDesc.height, m_GrainyBlur.blurCount.value));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, grainyBlur);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("GrainyBlur");
            
        }

        #endregion
        
        #region IrisBlur

        private void SetupIrisBlur(CommandBuffer cmd, ref RenderingData renderingData, Material irisBlur)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("IrisBlur");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_IrisBlur.filterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            irisBlur.SetVector("_GoldenRot", mGoldenRot);
            irisBlur.SetVector("_Gradient", new Vector3(m_IrisBlur.centerOffsetX.value, m_IrisBlur.centerOffsetY.value, m_IrisBlur.areaSize.value * 0.1f));
            irisBlur.SetVector("_Params", new Vector4(m_IrisBlur.blurCount.value, m_IrisBlur.indensity.value, 1f / opaqueDesc.width, 1f / opaqueDesc.height));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, irisBlur);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("IrisBlur");
            
        }

        #endregion
        
        #region TiltShiftBlur

        private void SetupTiltShiftBlur(CommandBuffer cmd, ref RenderingData renderingData, Material tiltShiftBlur)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("TiltShiftBlur");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_TiltShiftBlur.filterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            tiltShiftBlur.SetVector("_GoldenRot", mGoldenRot);
            tiltShiftBlur.SetVector("_Gradient", new Vector3(m_TiltShiftBlur.centerOffset.value, m_TiltShiftBlur.areaSize.value, m_TiltShiftBlur.areaSmooth.value));
            tiltShiftBlur.SetVector("_Params", new Vector4(m_TiltShiftBlur.blurCount.value, m_TiltShiftBlur.indensity.value, 1f / opaqueDesc.width, 1f / opaqueDesc.height));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, tiltShiftBlur);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("TiltShiftBlur");
            
        }

        #endregion
        
        #region BokehBlur

        private void SetupBokehBlur(CommandBuffer cmd, ref RenderingData renderingData, Material bokehBlur)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = opaqueDesc.width >> m_BokehBlur.downSample.value;
            opaqueDesc.height = opaqueDesc.height >> m_BokehBlur.downSample.value;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("BokehBlur");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_BokehBlur.filterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            bokehBlur.SetVector("_GoldenRot", mGoldenRot);
            bokehBlur.SetVector("_Offset", new Vector4(m_BokehBlur.blurCount.value, m_BokehBlur.indensity.value, 1f / opaqueDesc.width, 1f / opaqueDesc.height));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, bokehBlur);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("BokehBlur");
            
        }
        
        #endregion

        #region DualKawaseBlur

        private void SetupDualKawaseBlur(CommandBuffer cmd, ref RenderingData renderingData, Material dualKawaseBlur)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            int width = opaqueDesc.width >> m_DualKawaseBlur.downSample.value;
            int height = opaqueDesc.height >> m_DualKawaseBlur.downSample.value;
            
            cmd.BeginSample("DualKawaseBlur");
            dualKawaseBlur.SetFloat("_Offset", m_DualKawaseBlur.indensity.value);
            RenderTargetIdentifier lastDown = m_ColorAttachment;
            for (int i = 0; i < m_DualKawaseBlur.blurCount.value; i++)
            {
                int mipDown = m_Pyramid[i].down;
                int mipUp = m_Pyramid[i].up;
                cmd.GetTemporaryRT(mipDown, width, height, 0, m_DualKawaseBlur.filterMode.value);
                cmd.GetTemporaryRT(mipUp, width, height, 0, m_DualKawaseBlur.filterMode.value);
                cmd.Blit(lastDown, mipDown, dualKawaseBlur,0);
            
                lastDown = mipDown;
                width = Mathf.Max(width / 2, 1);
                height = Mathf.Max(height / 2, 1);
            }
            
            int lastUp = m_Pyramid[m_DualKawaseBlur.blurCount.value - 1].down;
            for (int i = m_DualKawaseBlur.blurCount.value - 2; i >= 0; i--)
            {
                int minUp = m_Pyramid[i].up;
                cmd.Blit(lastUp,minUp,dualKawaseBlur,1);
                lastUp = minUp;
            }
            cmd.Blit(lastUp,m_ColorAttachment,dualKawaseBlur,1);
            for (int i = 0; i < m_DualKawaseBlur.blurCount.value; i++)
            {
                if (m_Pyramid[i].down != lastUp)
                    cmd.ReleaseTemporaryRT(m_Pyramid[i].down);
                if (m_Pyramid[i].up != lastUp)
                    cmd.ReleaseTemporaryRT(m_Pyramid[i].up);
            }
            cmd.EndSample("DualKawaseBlur");

        }

        #endregion

        #region KawaseBlur

        private void SetupKawaseBlur(CommandBuffer cmd, ref RenderingData renderingData, Material kawaseBlur)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = opaqueDesc.width >> m_KawaseBlur.downSample.value;
            opaqueDesc.height = opaqueDesc.height >> m_KawaseBlur.downSample.value;
            opaqueDesc.depthBufferBits = 0;

            cmd.BeginSample("KawaseBlur");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_KawaseBlur.filterMode.value);
            cmd.GetTemporaryRT(m_TemporaryColorTexture02.id, opaqueDesc, m_KawaseBlur.filterMode.value);
            bool needSwitch = true;
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            for (int i = 0; i < m_KawaseBlur.blurCount.value; i++) {
                kawaseBlur.SetFloat("_Offset", i / m_KawaseBlur.downSample.value + m_KawaseBlur.indensity.value);
                cmd.Blit(needSwitch ? m_TemporaryColorTexture01.Identifier() : m_TemporaryColorTexture02.Identifier(), needSwitch ? m_TemporaryColorTexture02.Identifier() : m_TemporaryColorTexture01.Identifier(),kawaseBlur);
                needSwitch = !needSwitch;
            }
            kawaseBlur.SetFloat("_Offset", m_KawaseBlur.blurCount.value / m_KawaseBlur.downSample.value + m_KawaseBlur.indensity.value);
            cmd.Blit(needSwitch ? m_TemporaryColorTexture01.Identifier() : m_TemporaryColorTexture02.Identifier(), m_ColorAttachment);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture02.id);
            cmd.EndSample("KawaseBlur");
            
        }

        #endregion
        
        #region BoxBlur

        private void SetupBoxBlur(CommandBuffer cmd, ref RenderingData renderingData, Material boxBlur)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = opaqueDesc.width >> m_BoxBlur.downSample.value;
            opaqueDesc.height = opaqueDesc.height >> m_BoxBlur.downSample.value;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("BoxBlur");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_BoxBlur.filterMode.value);
            cmd.GetTemporaryRT(m_TemporaryColorTexture02.id, opaqueDesc, m_BoxBlur.filterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            for (int i = 0; i < m_BoxBlur.blurCount.value; i++) {
                boxBlur.SetVector("_Offset", new Vector4(m_BoxBlur.indensity.value, m_BoxBlur.indensity.value, 0, 0));
                cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_TemporaryColorTexture02.Identifier(), boxBlur);
                boxBlur.SetVector("_Offset", new Vector4(m_BoxBlur.indensity.value, m_BoxBlur.indensity.value, 0, 0));
                cmd.Blit(m_TemporaryColorTexture02.Identifier(), m_TemporaryColorTexture01.Identifier(), boxBlur);
            }
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture02.id);
            cmd.EndSample("BoxBlur");

        }

        #endregion
        
        #region GaussianBlur

        private void SetupGaussianBlur(CommandBuffer cmd, ref RenderingData renderingData, Material gaussianBlur)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = opaqueDesc.width >> m_GaussianBlur.downSample.value;
            opaqueDesc.height = opaqueDesc.height >> m_GaussianBlur.downSample.value;
            opaqueDesc.depthBufferBits = 0;
            
            // cmd.GetTemporaryRT(m_TemporaryColorTexture03.id, opaqueDesc, m_GaussianBlur.filterMode.value);
            
            cmd.BeginSample("GaussianBlur");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_GaussianBlur.filterMode.value);
            cmd.GetTemporaryRT(m_TemporaryColorTexture02.id, opaqueDesc, m_GaussianBlur.filterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            for (int i = 0; i < m_GaussianBlur.blurCount.value; i++) {
                //y-direction
                gaussianBlur.SetVector("_Offset", new Vector4(0, m_GaussianBlur.indensity.value, 0, 0));
                cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_TemporaryColorTexture02.Identifier(), gaussianBlur);
                //x-direction
                gaussianBlur.SetVector("_Offset", new Vector4(m_GaussianBlur.indensity.value, 0, 0, 0));
                cmd.Blit(m_TemporaryColorTexture02.Identifier(), m_TemporaryColorTexture01.Identifier(), gaussianBlur);
                
            }
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture02.id);
            cmd.EndSample("GaussianBlur");
            
            
        }

        #endregion

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            
        }

        public void Setup(RenderPassEvent @event, RenderTargetIdentifier cameraColorTarget)
        {
            renderPassEvent = @event;
            m_ColorAttachment = cameraColorTarget;
            // m_CameraDepthAttachment = cameraDepth;
            // m_Destination = dest;
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
        // var cameraDepth = renderer.cameraDepthTarget;
        // var dest = RenderTargetHandle.CameraTarget;
        
        m_ScriptablePass.Setup(evt, cameraColorTarget);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


