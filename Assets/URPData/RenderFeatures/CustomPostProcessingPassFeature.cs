using System;
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

        //blur
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
        private Level[] m_Pyramid;
        const int k_MaxPyramidSize = 16;
        private Vector4 mGoldenRot;
        struct Level
        {
            internal int down;
            internal int up;
        }
        
        //glitch
        private RGBSplit m_RGBSplit;
        private ImageBlock m_ImageBlock;
        private LineBlock m_LineBlock;
        private TileJitter m_TileJitter;
        private ScanLineJitter m_ScanLineJitter;
        private DigitalStripe m_DigitalStripe;
        private AnalogNoise m_AnalogNoise;
        private ScreenJump m_ScreenJump;
        private ScreenShake m_ScreenShake;
        private WaveJitter m_WaveJitter;
        private float TimeX = 1.0f;
        private float randomFrequency;
        private int frameCount = 0;
        private Texture2D _noiseTexture;
        private RenderTexture _trashFrame1;
        private RenderTexture _trashFrame2;
        private float ScreenJumpTime;
        
        //Edge Dectection
        private Roberts m_Roberts;
        private RobertsNeon m_RobertsNeon;
        private Scharr m_Scharr;


        private MaterialLibrary m_Materials;
        private CustomPostProcessingData m_Data;
        
        RenderTargetHandle m_TemporaryColorTexture01;
        RenderTargetHandle m_TemporaryColorTexture02;
        
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
        
        public void Cleanup() => m_Materials.Cleanup();

        
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
            //blur
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
            //glitch
            m_RGBSplit = stack.GetComponent<RGBSplit>();
            m_ImageBlock = stack.GetComponent<ImageBlock>();
            m_LineBlock = stack.GetComponent<LineBlock>();
            m_TileJitter = stack.GetComponent<TileJitter>();
            m_ScanLineJitter = stack.GetComponent<ScanLineJitter>();
            m_DigitalStripe = stack.GetComponent<DigitalStripe>();
            m_AnalogNoise = stack.GetComponent<AnalogNoise>();
            m_ScreenJump = stack.GetComponent<ScreenJump>();
            m_ScreenShake = stack.GetComponent<ScreenShake>();
            m_WaveJitter = stack.GetComponent<WaveJitter>();
            //EdgeDetection
            m_Roberts = stack.GetComponent<Roberts>();
            m_RobertsNeon = stack.GetComponent<RobertsNeon>();
            m_Scharr = stack.GetComponent<Scharr>();
            var cmd = CommandBufferPool.Get(k_RenderCustomPostProcessingTag);
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        private void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;

            #region Blur

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

            #endregion

            #region Glitch

            if (m_RGBSplit.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupRGBSplit(cmd, ref renderingData, m_Materials.rgbSplit);
            }

            if (m_ImageBlock.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupImageBlock(cmd, ref renderingData, m_Materials.imageBlock);
            }

            if (m_LineBlock.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupLineBlock(cmd, ref renderingData, m_Materials.lineBlock);
            }

            if (m_TileJitter.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupTileJitter(cmd, ref renderingData, m_Materials.tileJitter);
            }

            if (m_ScanLineJitter.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupScanLineJitter(cmd, ref renderingData, m_Materials.scanLineJitter);
            }

            if (m_DigitalStripe.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupDigitalStripe(cmd, ref renderingData, m_Materials.digitalStripe);
            }

            if (m_AnalogNoise.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupAnalogNoise(cmd, ref renderingData, m_Materials.analogNoise);
            }

            if (m_ScreenJump.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupScreenJump(cmd, ref renderingData, m_Materials.screenJump);
            }

            if (m_ScreenShake.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupScreenShake(cmd, ref renderingData, m_Materials.screenShake);
            }

            if (m_WaveJitter.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupWaveJitter(cmd, ref renderingData, m_Materials.waveJitter);
            }

            #endregion

            #region EdgeDetection

            if (m_Roberts.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupRoberts(cmd, ref renderingData, m_Materials.roberts);
            }

            if (m_RobertsNeon.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupRobertsNeon(cmd, ref renderingData, m_Materials.robertsNeon);
            }

            if (m_Scharr.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupScharr(cmd, ref renderingData, m_Materials.scharr);
            }

            #endregion
            
        }

        private void SetupScharr(CommandBuffer cmd, ref RenderingData renderingData, Material scharr)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Scharr");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_Scharr.FilterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            scharr.SetVector("_Params", new Vector2(m_Scharr.edgeWidth.value, m_Scharr.backgroundFade.value));
            scharr.SetColor("_EdgeColor", m_Scharr.edgeColor.value);
            scharr.SetColor("_BackgroundColor", m_Scharr.backgroundColor.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, scharr);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Scharr");
        }

        #region RobertsNeon

        private void SetupRobertsNeon(CommandBuffer cmd, ref RenderingData renderingData, Material robertsNeon)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("RobertsNeon");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_RobertsNeon.FilterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            robertsNeon.SetVector("_Params", new Vector4(m_RobertsNeon.edgeWidth.value, m_RobertsNeon.edgeNeonFade.value, m_RobertsNeon.brigtness.value, m_RobertsNeon.backgroundFade.value));
            robertsNeon.SetColor("_BackgroundColor", m_RobertsNeon.backgroundColor.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, robertsNeon);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("RobertsNeon");
        }

        #endregion
        

        #region Roberts

        private void SetupRoberts(CommandBuffer cmd, ref RenderingData renderingData, Material roberts)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Roberts");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_Roberts.FilterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            roberts.SetVector("_Params", new Vector2(m_Roberts.edgeWidth.value, m_Roberts.backgroundFade.value));
            roberts.SetColor("_EdgeColor", m_Roberts.edgeColor.value);
            roberts.SetColor("_BackgroundColor", m_Roberts.backgroundColor.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, roberts);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Roberts");
        }

        #endregion
        

        #region WaveJitter

        private void SetupWaveJitter(CommandBuffer cmd, ref RenderingData renderingData, Material waveJitter)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("WaveJitter");
            UpdateFrequencyWJ(waveJitter);
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_WaveJitter.FilterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            waveJitter.SetVector("_Params", new Vector4(m_WaveJitter.IntervalType.value == IntervalType.Random ? randomFrequency : m_WaveJitter.Frequency.value, m_WaveJitter.RGBSplit.value , m_WaveJitter.Speed.value, m_WaveJitter.Amount.value));
            waveJitter.SetVector("_Resolution", m_WaveJitter.CustomResolution.value ? m_WaveJitter.Resolution.value : new Vector2(opaqueDesc.width,opaqueDesc.height));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, waveJitter, (int)m_WaveJitter.JitterDirection.value);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("WaveJitter");
        }

        private void UpdateFrequencyWJ(Material waveJitter)
        {
            if (m_WaveJitter.IntervalType.value == IntervalType.Random)
            {
                randomFrequency = UnityEngine.Random.Range(0, m_WaveJitter.Frequency.value);
            }

            if (m_WaveJitter.IntervalType.value == IntervalType.Infinite)
            {
                waveJitter.EnableKeyword("USING_FREQUENCY_INFINITE");
            }
            else
            {
                waveJitter.DisableKeyword("USING_FREQUENCY_INFINITE");
            }
        }

        #endregion
        

        #region ScreenShake

        private void SetupScreenShake(CommandBuffer cmd, ref RenderingData renderingData, Material screenShake)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("ScreenShake");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_ScreenShake.FilterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            screenShake.SetFloat("_ScreenShake", m_ScreenShake.ScreenShakeIndensity.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, screenShake, (int)m_ScreenShake.ScreenShakeDirection.value);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("ScreenShake");
        }

        #endregion
        

        #region ScreenJump

        private void SetupScreenJump(CommandBuffer cmd, ref RenderingData renderingData, Material screenJump)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("ScreenJump");
            ScreenJumpTime += Time.deltaTime * m_ScreenJump.ScreenJumpIndensity.value * 9.8f;
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_ScreenJump.FilterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            screenJump.SetVector("_Params", new Vector2(m_ScreenJump.ScreenJumpIndensity.value, m_ScreenJump.isHorizontalReverse.value ? -ScreenJumpTime : ScreenJumpTime));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, screenJump, (int)m_ScreenJump.ScreenJumpDirection.value);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("ScreenJump");
        }

        #endregion
        

        #region AnalogNoise

        private void SetupAnalogNoise(CommandBuffer cmd, ref RenderingData renderingData, Material analogNoise)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("AnalogNoise");
            TimeX += Time.deltaTime;
            if (TimeX > 100)
            {
                TimeX = 0;
            }
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_AnalogNoise.FilterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            analogNoise.SetVector("_Params", new Vector4(m_AnalogNoise.NoiseSpeed.value, m_AnalogNoise.NoiseFading.value, m_AnalogNoise.LuminanceJitterThreshold.value, TimeX));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, analogNoise);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("AnalogNoise");
        }

        #endregion
        


        #region DigitalStripe

        private void SetupDigitalStripe(CommandBuffer cmd, ref RenderingData renderingData, Material digitalStripe)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("DigitalStripe");
            UpdateFrequencyDS(m_DigitalStripe.frequency.value, m_DigitalStripe.noiseTextureWidth.value, m_DigitalStripe.noiseTextureHeight.value, m_DigitalStripe.stripeLength.value);
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_DigitalStripe.FilterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            digitalStripe.SetFloat("_Indensity", m_DigitalStripe.indensity.value);
            if (_noiseTexture != null)
            {
                digitalStripe.SetTexture("_NoiseTex", _noiseTexture);
            }
            if (m_DigitalStripe.needStripColorAdjust.value)
            {
                digitalStripe.EnableKeyword("NEED_TRASH_FRAME");
                digitalStripe.SetColor("_StripColorAdjustColor", m_DigitalStripe.stripColorAdjustColor.value);
                digitalStripe.SetFloat("_StripColorAdjustIndensity", m_DigitalStripe.stripColorAdjustIndensity.value);
            }
            else
            {
                digitalStripe.DisableKeyword("NEED_TRASH_FRAME");
            }
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, digitalStripe);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("DigitalStripe");
        }

        private void UpdateFrequencyDS(int frame, int noiseTextureWidth, int noiseTextureHeight, float stripLength)
        {
            int frameCount = Time.frameCount;
            if (frameCount % frame != 0)
            {
                return;
            }

            _noiseTexture = new Texture2D(noiseTextureWidth, noiseTextureHeight, TextureFormat.ARGB32, false);
            _noiseTexture.wrapMode = TextureWrapMode.Clamp;
            _noiseTexture.filterMode = FilterMode.Point;

            _trashFrame1 = new RenderTexture(Screen.width, Screen.height, 0);
            _trashFrame2 = new RenderTexture(Screen.width, Screen.height, 0);
            _trashFrame1.hideFlags = HideFlags.DontSave;
            _trashFrame2.hideFlags = HideFlags.DontSave;

            Color color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

            for (int y = 0; y < _noiseTexture.height; y++)
            {
                for (int x = 0; x < _noiseTexture.width; x++)
                {
                    //随机值若大于给定strip随机阈值，重新随机颜色
                    if (UnityEngine.Random.value > stripLength)
                    {
                        color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                    }
                    //设置贴图像素值
                    _noiseTexture.SetPixel(x, y, color);
                }
            }

            _noiseTexture.Apply();

        }

        #endregion
        

        #region ScanLineJitter

        private void SetupScanLineJitter(CommandBuffer cmd, ref RenderingData renderingData, Material scanLineJitter)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("ScanLineJitter");
            UpdateFrequencySLJ(scanLineJitter);
            float displacement = 0.005f + Mathf.Pow(m_ScanLineJitter.JitterIndensity.value, 3) * 0.1f;
            float threshold = Mathf.Clamp01(1.0f - m_ScanLineJitter.JitterIndensity.value * 1.2f);
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_ScanLineJitter.FilterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            scanLineJitter.SetVector("_Params", new Vector3(displacement, threshold, m_ScanLineJitter.IntervalType.value == IntervalType.Random ? randomFrequency : m_ScanLineJitter.Frequency.value));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, scanLineJitter, (int)m_ScanLineJitter.JitterDirection.value);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("ScanLineJitter");
        }

        private void UpdateFrequencySLJ(Material scanLineJitter)
        {
            if (m_ScanLineJitter.IntervalType.value == IntervalType.Random)
            {
                randomFrequency = UnityEngine.Random.Range(0, m_ScanLineJitter.Frequency.value);
            }

            if (m_ScanLineJitter.IntervalType.value == IntervalType.Infinite)
            {
                scanLineJitter.EnableKeyword("USING_FREQUENCY_INFINITE");
            }
            else
            {
                scanLineJitter.DisableKeyword("USING_FREQUENCY_INFINITE");
            }
        }

        #endregion
        

        #region TileJitter

        private void SetupTileJitter(CommandBuffer cmd, ref RenderingData renderingData, Material tileJitter)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("TileJitter");
            UpdateFrequencyTJ(tileJitter);
            if (m_TileJitter.JitterDirection.value == Direction.Horizontal)
            {
                tileJitter.EnableKeyword("JITTER_DIRECTION_HORIZONTAL");
            }
            else
            {
                tileJitter.DisableKeyword("JITTER_DIRECTION_HORIZONTAL");
            }
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_TileJitter.FilterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            tileJitter.SetVector("_Params", new Vector4(m_TileJitter.SplittingNumber.value, m_TileJitter.Amount.value , m_TileJitter.Speed.value * 100f, m_TileJitter.IntervalType.value == IntervalType.Random ? randomFrequency : m_TileJitter.Frequency.value));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, tileJitter, m_TileJitter.SplittingDirection.value == Direction.Horizontal ? 0 : 1);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("TileJitter");
        }

        private void UpdateFrequencyTJ(Material tileJitter)
        {
            if (m_TileJitter.IntervalType.value == IntervalType.Random)
            {
                randomFrequency = UnityEngine.Random.Range(0, m_TileJitter.Frequency.value);
            }

            if (m_TileJitter.IntervalType.value == IntervalType.Infinite)
            {
                tileJitter.EnableKeyword("USING_FREQUENCY_INFINITE");
            }
            else
            {
                tileJitter.DisableKeyword("USING_FREQUENCY_INFINITE");
            }
        }

        #endregion
        

        #region LineBlock

        private void SetupLineBlock(CommandBuffer cmd, ref RenderingData renderingData, Material lineBlock)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("LineBlock");
            UpdateFrequency(lineBlock);
            TimeX += Time.deltaTime;
            if (TimeX > 100)
            {
                TimeX = 0;
            }
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_LineBlock.FilterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            lineBlock.SetVector("_Params", new Vector3(
                m_LineBlock.IntervalType.value == IntervalType.Random ? randomFrequency : m_LineBlock.Frequency.value,
                TimeX * m_LineBlock.Speed.value * 0.2f , m_LineBlock.Amount.value));
            lineBlock.SetVector("_Params2", new Vector3(m_LineBlock.Offset.value, 1 / m_LineBlock.LinesWidth.value, m_LineBlock.Alpha.value));
            int pass = (int)m_LineBlock.BlockDirection.value;
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, lineBlock,pass);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("LineBlock");
        }

        private void UpdateFrequency(Material lineBlock)
        {
            if (m_LineBlock.IntervalType.value == IntervalType.Random)
            {
                if (frameCount > m_LineBlock.Frequency.value)
                {

                    frameCount = 0;
                    randomFrequency = UnityEngine.Random.Range(0, m_LineBlock.Frequency.value);
                }
                frameCount++;
            }

            if (m_LineBlock.IntervalType.value == IntervalType.Infinite)
            {
                lineBlock.EnableKeyword("USING_FREQUENCY_INFINITE");
            }
            else
            {
                lineBlock.DisableKeyword("USING_FREQUENCY_INFINITE");
            }
        }

        #endregion
        

        #region ImageBlock

        private void SetupImageBlock(CommandBuffer cmd, ref RenderingData renderingData, Material imageBlock)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            cmd.BeginSample("ImageBlock");
            TimeX += Time.deltaTime;
            if (TimeX > 100)
            {
                TimeX = 0;
            }
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_ImageBlock.filterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            imageBlock.SetVector("_Params", new Vector3(TimeX * m_ImageBlock.Speed.value, m_ImageBlock.Amount.value, m_ImageBlock.Fade.value));
            imageBlock.SetVector("_Params2", new Vector4(m_ImageBlock.BlockLayer1_U.value, m_ImageBlock.BlockLayer1_V.value, m_ImageBlock.BlockLayer2_U.value, m_ImageBlock.BlockLayer2_V.value));
            imageBlock.SetVector("_Params3", new Vector3(m_ImageBlock.RGBSplitIndensity.value, m_ImageBlock.BlockLayer1_Indensity.value, m_ImageBlock.BlockLayer2_Indensity.value));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, imageBlock);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("ImageBlock");
        }

        #endregion
        

        #region RGBSplit

        private void SetupRGBSplit(CommandBuffer cmd, ref RenderingData renderingData, Material rgbSplit)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            cmd.BeginSample("RGBSplit");
            TimeX += Time.deltaTime;
            if (TimeX > 100)
            {
                TimeX = 0;
            }
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, m_RGBSplit.filterMode.value);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            rgbSplit.SetVector("_Params", new Vector4(m_RGBSplit.Fading.value, m_RGBSplit.Amount.value, m_RGBSplit.Speed.value, m_RGBSplit.CenterFading.value));
            rgbSplit.SetVector("_Params2", new Vector3(TimeX, m_RGBSplit.AmountR.value, m_RGBSplit.AmountB.value));
            int pass = (int)m_RGBSplit.SplitDirection.value;
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, rgbSplit,pass);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("RGBSplit");
        }


        #endregion
        
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


    protected override void Dispose(bool disposing)
    {
        // base.Dispose(disposing);
        m_ScriptablePass.Cleanup();
    }
}


