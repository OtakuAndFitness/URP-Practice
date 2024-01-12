using System;
using PostProcessingExtends;
using PostProcessingExtends.Effects;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UPostProcessingExtends.Effects;
using WhiteBalance = PostProcessingExtends.Effects.WhiteBalance;

public class CustomPostProcessingPassFeature : ScriptableRendererFeature
{
    class CustomPostProcessingPass : ScriptableRenderPass
    {
        // private RenderTargetIdentifier m_ColorAttachment;
        // private RenderTargetIdentifier m_CameraDepthAttachment;
        // private RenderTargetHandle m_Destination;

        // const string k_RenderPostProcessingTag = "Render AdditionalPostProcessing Effects";
        const string k_RenderCustomPostProcessingTag = "Render Custom PostProcessing Pass";
        
        private int _SourceTextureId = Shader.PropertyToID("_SourceTexture");

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
        // private Level[] m_Pyramid;
        // const int k_MaxPyramidSize = 16;
        // private Vector4 mGoldenRot;
        // struct Level
        // {
        //     internal int down;
        //     internal int up;
        // }

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
        private float _TimeX = 1.0f;
        private float _randomFrequency;
        private int _frameCount = 0;
        private Texture2D _noiseTexture;
        private RenderTexture _trashFrame1;
        private RenderTexture _trashFrame2;
        private float _screenJumpTime;

        //Edge Dectection
        private Roberts m_Roberts;
        private RobertsNeon m_RobertsNeon;
        private Scharr m_Scharr;
        private ScharrNeon m_ScharrNeon;
        private Sobel m_Sobel;
        private SobelNeon m_SobelNeon;

        //Pixelise
        private Circle m_Circle;
        private Diamond m_Diamond;
        private Hexagon m_Hexagon;
        private HexagonGrid m_HexagonGrid;
        private Leaf m_Leaf;
        private Led m_Led;
        private Quad m_Quad;
        private Sector m_Sector;
        private Triangle m_Triangle;

        //Vignette
        private Aurora m_Aurora;
        private RapidOldTV m_RapidOldTV;
        private RapidOldTVV2 m_RapidOldTVV2;
        private Rapid m_Rapid;
        private RapidV2 m_RapidV2;

        //Sharpen
        private SharpenV1 m_V1;
        private SharpenV2 m_V2;
        private SharpenV3 m_V3;

        //ColorAdjustment
        private BleachBypass m_BleachBypass;
        private Brightness m_Brightness;
        private Hue m_Hue;
        private Tint m_Tint;
        private WhiteBalance m_WhiteBalance;
        private LensFilter m_LensFilter;
        private Saturation m_Saturation;
        private Technicolor m_Technicolor;
        private ColorReplace m_ColorReplace;
        private ColorReplaceV2 m_ColorReplaceV2;
        private Contrast m_Contrast;
        private ContrastV2 m_ContrastV2;
        private ContrastV3 m_ContrastV3;


        private MaterialLibrary m_Materials;
        private CustomPostProcessingData m_Data;

        RTHandle m_TemporaryColorTexture01;
        RTHandle m_TemporaryColorTexture02;

        // RTHandle m_TemporaryBlurTexture03;
        // RTHandle m_TemporaryBlurTexture04;

        // RenderTargetHandle m_TemporaryColorTexture03;
        private RTHandle _sourceRT;
        private RTHandle _destinationRT;
        private RTHandle _tempRT0;
        private RTHandle _tempRT1;
        
        public CustomPostProcessingPass(CustomPostProcessingData data)
        {
            m_Data = data;
            m_Materials = new MaterialLibrary(m_Data);
            
            // //for custom post processing
            // m_TemporaryColorTexture01.Init("m_TemporaryColorTexture01");
            // m_TemporaryColorTexture02.Init("m_TemporaryColorTexture02");
            //
            // //for some blur with dowmsampling
            // m_TemporaryBlurTexture03.Init("m_TemporaryBlurTexture03");
            // m_TemporaryBlurTexture04.Init("m_TemporaryBlurTexture04");

            //for dual blur
            // m_Pyramid = new Level[k_MaxPyramidSize];
            // for (int i = 0; i < k_MaxPyramidSize; i++)
            // {
            //     m_Pyramid[i] = new Level
            //     {
            //         down = Shader.PropertyToID("_BlurMipDown" + i),
            //         up = Shader.PropertyToID("_BlurMipUp" + i)
            //     };
            // }

            // mGoldenRot = new Vector4();
            // Precompute rotations
            // float c = Mathf.Cos(2.39996323f);
            // float s = Mathf.Sin(2.39996323f);
            // mGoldenRot.Set(c, s, -s, c);
            // m_TemporaryColorTexture03.Init("m_TemporaryColorTexture03");
        }

        public void Cleanup()
        {
            m_Materials.Cleanup();
            _tempRT0?.Release();
            _tempRT1?.Release();
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
            // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Point);
            // cmd.GetTemporaryRT(m_TemporaryColorTexture02.id, opaqueDesc, FilterMode.Point);

            //for custom post processing
            // m_TemporaryColorTexture01.Init("m_TemporaryColorTexture01");
            // m_TemporaryColorTexture02.Init("m_TemporaryColorTexture02");
            RenderingUtils.ReAllocateIfNeeded(ref m_TemporaryColorTexture01, opaqueDesc, name: "_TemporaryRenderTexture01",
                wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
            RenderingUtils.ReAllocateIfNeeded(ref m_TemporaryColorTexture02, opaqueDesc, name: "_TemporaryRenderTexture02",
                wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);

            //for some blur with dowmsampling
            // m_TemporaryBlurTexture03.Init("m_TemporaryBlurTexture03");
            // m_TemporaryBlurTexture04.Init("m_TemporaryBlurTexture04");
            
            RenderingUtils.ReAllocateIfNeeded(ref _tempRT0, opaqueDesc, name: "_tempRT0");
            RenderingUtils.ReAllocateIfNeeded(ref _tempRT1, opaqueDesc, name: "_tempRT1");
        }
        
        
        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture02.id);
            _sourceRT = null;
            _destinationRT = null;
            m_TemporaryColorTexture01?.Release();
            m_TemporaryColorTexture02?.Release();
        }

        // public void Setup(RenderPassEvent @event, RenderTargetIdentifier cameraColorTarget)
        // {
        //     renderPassEvent = @event;
        //     m_ColorAttachment = cameraColorTarget;
        //     // m_CameraDepthAttachment = cameraDepth;
        //     // m_Destination = dest;
        //     // m_Materials = new MaterialLibrary(data);
        // }
        

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
            m_ScharrNeon = stack.GetComponent<ScharrNeon>();
            m_Sobel = stack.GetComponent<Sobel>();
            m_SobelNeon = stack.GetComponent<SobelNeon>();
            //Pixelise
            m_Circle = stack.GetComponent<Circle>();
            m_Diamond = stack.GetComponent<Diamond>();
            m_Hexagon = stack.GetComponent<Hexagon>();
            m_HexagonGrid = stack.GetComponent<HexagonGrid>();
            m_Leaf = stack.GetComponent<Leaf>();
            m_Led = stack.GetComponent<Led>();
            m_Quad = stack.GetComponent<Quad>();
            m_Sector = stack.GetComponent<Sector>();
            m_Triangle = stack.GetComponent<Triangle>();
            //Vignette
            m_Aurora = stack.GetComponent<Aurora>();
            m_RapidOldTV = stack.GetComponent<RapidOldTV>();
            m_RapidOldTVV2 = stack.GetComponent<RapidOldTVV2>();
            m_Rapid = stack.GetComponent<Rapid>();
            m_RapidV2 = stack.GetComponent<RapidV2>();
            //Sharpen
            m_V1 = stack.GetComponent<SharpenV1>();
            m_V2 = stack.GetComponent<SharpenV2>();
            m_V3 = stack.GetComponent<SharpenV3>();
            //ColorAdjustment
            m_BleachBypass = stack.GetComponent<BleachBypass>();
            m_Brightness = stack.GetComponent<Brightness>();
            m_Hue = stack.GetComponent<Hue>();
            m_Tint = stack.GetComponent<Tint>();
            m_WhiteBalance = stack.GetComponent<WhiteBalance>();
            m_LensFilter = stack.GetComponent<LensFilter>();
            m_Saturation = stack.GetComponent<Saturation>();
            m_Technicolor = stack.GetComponent<Technicolor>();
            m_ColorReplace = stack.GetComponent<ColorReplace>();
            m_ColorReplaceV2 = stack.GetComponent<ColorReplaceV2>();
            m_Contrast = stack.GetComponent<Contrast>();
            m_ContrastV2 = stack.GetComponent<ContrastV2>();
            m_ContrastV3 = stack.GetComponent<ContrastV3>();

            _destinationRT = renderingData.cameraData.renderer.cameraColorTargetHandle;
            _sourceRT = renderingData.cameraData.renderer.cameraColorTargetHandle;
            
            var cmd = CommandBufferPool.Get(k_RenderCustomPostProcessingTag);
            Blitter.BlitCameraTexture(cmd, _sourceRT, _tempRT0);
            Render(cmd, ref renderingData, _tempRT0, _tempRT1);
            Blitter.BlitCameraTexture(cmd, _tempRT1, _destinationRT);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        private void Render(CommandBuffer cmd, ref RenderingData renderingData, in RTHandle source,
            in RTHandle destination)
        {
            ref var cameraData = ref renderingData.cameraData;

            #region Blur

            if (m_GaussianBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.GaussianBlur)))
                {
                    SetupGaussianBlur(cmd, ref renderingData, m_Materials.gaussianBlur, source, destination);
                }
            }

            if (m_BoxBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.BoxBlur)))
                {
                    SetupBoxBlur(cmd, ref renderingData, m_Materials.boxBlur, source, destination);
                }
            }

            if (m_KawaseBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.KawaseBlur)))
                {
                    SetupKawaseBlur(cmd, ref renderingData, m_Materials.kawaseBlur, source, destination);
                }

            }

            if (m_DualKawaseBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.DualKawaseBlur)))
                {
                    SetupDualKawaseBlur(cmd, ref renderingData, m_Materials.dualKawaseBlur, source, destination);
                }
            }

            if (m_BokehBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.BokehBlur)))
                {
                    SetupBokehBlur(cmd, ref renderingData, m_Materials.bokehBlur, source, destination);
                }

            }

            if (m_TiltShiftBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.TiltShiftBlur)))
                {
                    SetupTiltShiftBlur(cmd, ref renderingData, m_Materials.tiltShiftBlur, source, destination);
                }
            }

            if (m_IrisBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.IrisBlur)))
                {
                    SetupIrisBlur(cmd, ref renderingData, m_Materials.irisBlur, source, destination);
                }
            }

            if (m_GrainyBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.GrainyBlur)))
                {
                    SetupGrainyBlur(cmd, ref renderingData, m_Materials.grainyBlur, source, destination);
                }
            }

            if (m_RadialBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.RadialBlur)))
                {
                    SetupRadialBlur(cmd, ref renderingData, m_Materials.radialBlur, source, destination);
                }
            }

            if (m_DirectionalBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.DirectionalBlur)))
                {
                    SetupDirectionalBlur(cmd, ref renderingData, m_Materials.directionalBlur, source, destination);
                }

            }

            #endregion

            #region Glitch

            if (m_RGBSplit.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.RGBSplit)))
                {
                    SetupRGBSplit(cmd, ref renderingData, m_Materials.rgbSplit, source, destination);
                }
            }

            if (m_ImageBlock.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.ImageBlock)))
                {
                    SetupImageBlock(cmd, ref renderingData, m_Materials.imageBlock, source, destination);
                }
            }

            if (m_LineBlock.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.LineBlock)))
                {
                    SetupLineBlock(cmd, ref renderingData, m_Materials.lineBlock, source, destination);
                }
            }

            if (m_TileJitter.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.TileJitter)))
                {
                    SetupTileJitter(cmd, ref renderingData, m_Materials.tileJitter, source, destination);
                }
            }

            if (m_ScanLineJitter.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.ScanLineJitter)))
                {
                    SetupScanLineJitter(cmd, ref renderingData, m_Materials.scanLineJitter, source, destination);
                }
            }

            if (m_DigitalStripe.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.DigitalStripe)))
                {
                    SetupDigitalStripe(cmd, ref renderingData, m_Materials.digitalStripe, source, destination);
                }
            }

            if (m_AnalogNoise.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.AnalogNoise)))
                {
                    SetupAnalogNoise(cmd, ref renderingData, m_Materials.analogNoise, source, destination);
                }
            }

            if (m_ScreenJump.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.ScreenJump)))
                {
                    SetupScreenJump(cmd, ref renderingData, m_Materials.screenJump, source, destination);
                }
            }

            if (m_ScreenShake.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.ScreenShake)))
                {
                    SetupScreenShake(cmd, ref renderingData, m_Materials.screenShake, source, destination);
                }
            }

            if (m_WaveJitter.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.WaveJitter)))
                {
                    SetupWaveJitter(cmd, ref renderingData, m_Materials.waveJitter, source, destination);
                }
            }

            #endregion

            #region EdgeDetection

            if (m_Roberts.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Roberts)))
                {
                    SetupRoberts(cmd, ref renderingData, m_Materials.roberts, source, destination);
                }
            }

            if (m_RobertsNeon.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.RobertsNeon)))
                {
                    SetupRobertsNeon(cmd, ref renderingData, m_Materials.robertsNeon, source, destination);
                }
            }

            if (m_Scharr.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Scharr)))
                {
                    SetupScharr(cmd, ref renderingData, m_Materials.scharr, source, destination);
                }
            }

            if (m_ScharrNeon.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.ScharrNeon)))
                {
                    SetupScharrNeon(cmd, ref renderingData, m_Materials.scharrNeon, source, destination);
                }
            }

            if (m_Sobel.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Sobel)))
                {
                    SetupSobel(cmd, ref renderingData, m_Materials.sobel, source, destination);
                }
            }

            if (m_SobelNeon.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.SobelNeon)))
                {
                    SetupSobelNeon(cmd, ref renderingData, m_Materials.sobelNeon, source, destination);
                }
            }

            #endregion

            #region Pixelise

            if (m_Circle.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Circle)))
                {
                    SetupCircle(cmd, ref renderingData, m_Materials.circle, source, destination);
                }
            }

            if (m_Diamond.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Diamond)))
                {
                    SetupDiamond(cmd, ref renderingData, m_Materials.diamond, source, destination);
                }
            }

            if (m_Hexagon.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Hexagon)))
                {
                    SetupHexagon(cmd, ref renderingData, m_Materials.hexagon, source, destination);
                }
            }

            if (m_HexagonGrid.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.HexagonGrid)))
                {
                    SetupHexagonGrid(cmd, ref renderingData, m_Materials.hexagonGrid, source, destination);
                }
            }

            if (m_Leaf.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Leaf)))
                {
                    SetupLeaf(cmd, ref renderingData, m_Materials.leaf, source, destination);
                }
            }

            if (m_Led.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Led)))
                {
                    SetupLed(cmd, ref renderingData, m_Materials.led, source, destination);
                }
            }

            if (m_Quad.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Quad)))
                {
                    SetupQuad(cmd, ref renderingData, m_Materials.quad, source, destination);
                }
            }

            if (m_Sector.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Sector)))
                {
                    SetupSector(cmd, ref renderingData, m_Materials.sector, source, destination);
                }
            }

            if (m_Triangle.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Triangle)))
                {
                    SetupTriangle(cmd, ref renderingData, m_Materials.triangle, source, destination);
                }
            }

            #endregion

            #region Vignette

            if (m_Aurora.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Aurora)))
                {
                    SetupAurora(cmd, ref renderingData, m_Materials.aurora, source, destination);
                }
            }

            if (m_RapidOldTV.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.RapidOldTV)))
                {
                    SetupRapidOldTV(cmd, ref renderingData, m_Materials.rapidOldTV, source, destination);
                }
            }

            if (m_RapidOldTVV2.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.RapidOldTVV2)))
                {
                    SetupRapidOldTVV2(cmd, ref renderingData, m_Materials.rapidOldTVV2, source, destination);
                }
            }

            if (m_Rapid.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Rapid)))
                {
                    SetupRapid(cmd, ref renderingData, m_Materials.rapid, source, destination);
                }
            }

            if (m_RapidV2.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.RapidV2)))
                {
                    SetupRapidV2(cmd, ref renderingData, m_Materials.rapidV2, source, destination);
                }
            }

            #endregion

            #region Sharpen

            if (m_V1.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.SharpenV1)))
                {
                    SetupSharpenV1(cmd, ref renderingData, m_Materials.sharpenV1, source, destination);
                }
            }

            if (m_V2.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.SharpenV2)))
                {
                    SetupSharpenV2(cmd, ref renderingData, m_Materials.sharpenV2, source, destination);
                }
            }

            if (m_V3.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.SharpenV3)))
                {
                    SetupSharpenV3(cmd, ref renderingData, m_Materials.sharpenV3, source, destination);
                }
            }

            #endregion

            #region BleachBypass

            if (m_BleachBypass.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.BleachBypass)))
                {
                    SetupBleachBypass(cmd, ref renderingData, m_Materials.bleachBypass, source, destination);
                }
            }

            if (m_Brightness.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Brightness)))
                {
                    SetupBrightness(cmd, ref renderingData, m_Materials.brightness, source, destination);
                }
            }

            if (m_Hue.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Hue)))
                {
                    SetupHue(cmd, ref renderingData, m_Materials.hue, source, destination);

                }
            }

            if (m_Tint.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Tint)))
                {
                    SetupTint(cmd, ref renderingData, m_Materials.tint, source, destination);
                }
            }

            if (m_WhiteBalance.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.WhiteBalance)))
                {
                    SetupWhiteBalance(cmd, ref renderingData, m_Materials.whiteBalance, source, destination);
                }
            }

            if (m_LensFilter.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.LensFilter)))
                {
                    SetupLensFilter(cmd, ref renderingData, m_Materials.lensFilter, source, destination);
                }
            }

            if (m_Saturation.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Saturation)))
                {
                    SetupSaturation(cmd, ref renderingData, m_Materials.saturation, source, destination);
                }
            }

            if (m_Technicolor.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Technicolor)))
                {
                    SetupTechnicolor(cmd, ref renderingData, m_Materials.technicolor, source, destination);
                }
            }

            if (m_ColorReplace.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.ColorReplace)))
                {
                    SetupColorReplace(cmd, ref renderingData, m_Materials.colorReplace, source, destination);
                }
            }
            
            if (m_ColorReplaceV2.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.ColorReplace)))
                {
                    SetupColorReplaceV2(cmd, ref renderingData, m_Materials.colorReplaceV2, source, destination);
                }
            }

            if (m_Contrast.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Contrast)))
                {
                    SetupContrast(cmd, ref renderingData, m_Materials.contrast, source, destination);
                }
            }

            if (m_ContrastV2.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.ContrastV2)))
                {
                    SetupContrastV2(cmd, ref renderingData, m_Materials.contrastV2, source, destination);
                }
            }

            if (m_ContrastV3.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.ContrastV3)))
                {
                    SetupContrastV3(cmd, ref renderingData, m_Materials.contrastV3, source, destination);
                }
            }

            #endregion

        }
        
        public void Draw(CommandBuffer cmd, Material mat, in RTHandle source, in RTHandle destination, int pass = -1)
        {
            Material material = mat;
            
            cmd.SetGlobalTexture(_SourceTextureId, source);
            cmd.SetRenderTarget(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            if (pass == -1 || material == null)
            {
                cmd.DrawProcedural(Matrix4x4.identity, m_Materials.copyMaterial, 0, MeshTopology.Triangles, 3);
            }
            else
            {
                cmd.DrawProcedural(Matrix4x4.identity, material, pass, MeshTopology.Triangles, 3);
            }
        }

        #region Contrast

        private void SetupContrastV3(CommandBuffer cmd, ref RenderingData renderingData, Material contrastV3, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("ContrastV3");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // contrastV3.SetVector(CustomPostProcessingShaderConstants._Contrast, m_ContrastV3.contrast.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, contrastV3);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("ContrastV3");
            int contrastV3Keyword = Shader.PropertyToID("ContrastV3");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalVector(contrastV3Keyword, m_ContrastV3.contrast.value);
            Draw(cmd, contrastV3, m_TemporaryColorTexture01, destination, 0);
        }

        private void SetupContrastV2(CommandBuffer cmd, ref RenderingData renderingData, Material contrastV2, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("ContrastV2");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // contrastV2.SetFloat(CustomPostProcessingShaderConstants._Contrast, m_ContrastV2.contrast.value + 1);
            // contrastV2.SetColor(CustomPostProcessingShaderConstants._ContrastFactorRGB, new Color(m_ContrastV2.contrastFactorR.value, m_ContrastV2.contrastFactorG.value,m_ContrastV2.contrastFactorB.value));
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, contrastV2);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("ContrastV2");
            
            int contrastV2Keyword = Shader.PropertyToID("_ContrastV2");
            int contrastV2ColorKeyword = Shader.PropertyToID("_ContrastV2FactorRGB");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalFloat(contrastV2Keyword, m_ContrastV2.contrast.value + 1);
            cmd.SetGlobalColor(contrastV2ColorKeyword, new Color(m_ContrastV2.contrastFactorR.value, m_ContrastV2.contrastFactorG.value,m_ContrastV2.contrastFactorB.value));
            Draw(cmd, contrastV2, m_TemporaryColorTexture01, destination, 0);
        }

        private void SetupContrast(CommandBuffer cmd, ref RenderingData renderingData, Material contrast, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Contrast");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // contrast.SetFloat(CustomPostProcessingShaderConstants._Contrast, m_Contrast.contrast.value + 1);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, contrast);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Contrast");
            int contrastKeyword = Shader.PropertyToID("_Contrast");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalFloat(contrastKeyword, m_Contrast.contrast.value + 1);
            Draw(cmd, contrast, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region ColorReplace

        private void SetupColorReplace(CommandBuffer cmd, ref RenderingData renderingData, Material colorReplace, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("ColorReplace");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // colorReplace.SetFloat(CustomPostProcessingShaderConstants._Range, m_ColorReplace.Range.value);
            // colorReplace.SetFloat(CustomPostProcessingShaderConstants._Fuzziness, m_ColorReplace.Fuzziness.value);
            // if (m_ColorReplace.colorType == ColorType.Original)
            // {
            //     colorReplace.SetColor(CustomPostProcessingShaderConstants._FromColor, m_ColorReplace.FromColor.value);
            //     colorReplace.SetColor(CustomPostProcessingShaderConstants._ToColor, m_ColorReplace.ToColor.value);
            // }
            // else
            // {
            //     TimeX += (Time.deltaTime * m_ColorReplace.gridentSpeed.value);
            //     if (TimeX > 100)
            //     {
            //         TimeX = 0;
            //     }
            //
            //     if (m_ColorReplace.FromGradientColor.value != null)
            //     {
            //         colorReplace.SetColor(CustomPostProcessingShaderConstants._FromColor, m_ColorReplace.FromGradientColor.value.Evaluate(TimeX * 0.01f));
            //     }
            //
            //     if (m_ColorReplace.ToGradientColor.value != null)
            //     {
            //         colorReplace.SetColor(CustomPostProcessingShaderConstants._ToColor, m_ColorReplace.ToGradientColor.value.Evaluate(TimeX * 0.01f));
            //     }
            // }
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, colorReplace);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("ColorReplace");
            
            int rangeKeyword = Shader.PropertyToID("_ColorReplaceRange");
            int fuzzinessKeyword = Shader.PropertyToID("_ColorReplaceFuzziness");
            int fromColorKeyword = Shader.PropertyToID("_ColorReplaceFromColor");
            int toColorKeyword = Shader.PropertyToID("_ColorReplaceToColor");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            cmd.SetGlobalFloat(rangeKeyword, m_ColorReplace.Range.value);
            cmd.SetGlobalFloat(fuzzinessKeyword, m_ColorReplace.Fuzziness.value);
            cmd.SetGlobalColor(fromColorKeyword, m_ColorReplace.FromColor.value);
            cmd.SetGlobalColor(toColorKeyword, m_ColorReplace.ToColor.value);
            
            Draw(cmd, colorReplace, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion

        #region ColorReplaceV2

        private void SetupColorReplaceV2(CommandBuffer cmd, ref RenderingData renderingData, Material colorReplaceV2, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("ColorReplace");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // colorReplace.SetFloat(CustomPostProcessingShaderConstants._Range, m_ColorReplace.Range.value);
            // colorReplace.SetFloat(CustomPostProcessingShaderConstants._Fuzziness, m_ColorReplace.Fuzziness.value);
            // if (m_ColorReplace.colorType == ColorType.Original)
            // {
            //     colorReplace.SetColor(CustomPostProcessingShaderConstants._FromColor, m_ColorReplace.FromColor.value);
            //     colorReplace.SetColor(CustomPostProcessingShaderConstants._ToColor, m_ColorReplace.ToColor.value);
            // }
            // else
            // {
            //     TimeX += (Time.deltaTime * m_ColorReplace.gridentSpeed.value);
            //     if (TimeX > 100)
            //     {
            //         TimeX = 0;
            //     }
            //
            //     if (m_ColorReplace.FromGradientColor.value != null)
            //     {
            //         colorReplace.SetColor(CustomPostProcessingShaderConstants._FromColor, m_ColorReplace.FromGradientColor.value.Evaluate(TimeX * 0.01f));
            //     }
            //
            //     if (m_ColorReplace.ToGradientColor.value != null)
            //     {
            //         colorReplace.SetColor(CustomPostProcessingShaderConstants._ToColor, m_ColorReplace.ToGradientColor.value.Evaluate(TimeX * 0.01f));
            //     }
            // }
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, colorReplace);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("ColorReplace");
            
            int rangeKeyword = Shader.PropertyToID("_ColorReplaceV2Range");
            int fuzzinessKeyword = Shader.PropertyToID("_ColorReplaceV2Fuzziness");
            int fromColorKeyword = Shader.PropertyToID("_ColorReplaceV2FromColor");
            int toColorKeyword = Shader.PropertyToID("_ColorReplaceV2ToColor");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            cmd.SetGlobalFloat(rangeKeyword, m_ColorReplaceV2.Range.value);
            cmd.SetGlobalFloat(fuzzinessKeyword, m_ColorReplaceV2.Fuzziness.value);
            
            _TimeX += (Time.deltaTime * m_ColorReplaceV2.gridentSpeed.value);
            if (_TimeX > 100)
            {
                _TimeX = 0;
            }

            if (m_ColorReplaceV2.FromGradientColor.value != null)
            {
                cmd.SetGlobalColor(fromColorKeyword, m_ColorReplaceV2.FromGradientColor.value.Evaluate(_TimeX * 0.01f));
            }

            if (m_ColorReplaceV2.ToGradientColor.value != null)
            {
                cmd.SetGlobalColor(toColorKeyword, m_ColorReplaceV2.ToGradientColor.value.Evaluate(_TimeX * 0.01f));
            }
            
            Draw(cmd, colorReplaceV2, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion

        #region Technicolor

        private void SetupTechnicolor(CommandBuffer cmd, ref RenderingData renderingData, Material technicolor, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Technicolor");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // technicolor.SetFloat(CustomPostProcessingShaderConstants._Exposure, 8.01f - m_Technicolor.exposure.value);
            // technicolor.SetFloat(CustomPostProcessingShaderConstants._Indensity, m_Technicolor.indensity.value);
            // technicolor.SetColor(CustomPostProcessingShaderConstants._ColorBalance, Color.white - new Color(m_Technicolor.colorBalanceR.value, m_Technicolor.colorBalanceG.value, m_Technicolor.colorBalanceB.value));
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, technicolor);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Technicolor");
            
            int exposureKeyword = Shader.PropertyToID("_TechnicolorExposure");
            int indensityKeyword = Shader.PropertyToID("_TechnicolorIndensity");
            int TechnicolorKeyword = Shader.PropertyToID("_Technicolor");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalFloat(exposureKeyword, 8.01f - m_Technicolor.exposure.value);
            cmd.SetGlobalFloat(indensityKeyword, m_Technicolor.indensity.value);
            cmd.SetGlobalColor(TechnicolorKeyword, Color.white - new Color(m_Technicolor.colorBalanceR.value, m_Technicolor.colorBalanceG.value, m_Technicolor.colorBalanceB.value));
            Draw(cmd, technicolor, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region Saturation

        private void SetupSaturation(CommandBuffer cmd, ref RenderingData renderingData, Material saturation, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Saturation");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // saturation.SetFloat(CustomPostProcessingShaderConstants._Saturation, m_Saturation.saturation.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, saturation);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Saturation");
            int saturationKeyword = Shader.PropertyToID("_Saturation");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalFloat(saturationKeyword, m_Saturation.saturation.value);
            Draw(cmd, saturation, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region LensFilter

        private void SetupLensFilter(CommandBuffer cmd, ref RenderingData renderingData, Material lensFilter, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("LensFilter");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // lensFilter.SetFloat(CustomPostProcessingShaderConstants._Indensity, m_LensFilter.Indensity.value);
            // lensFilter.SetColor(CustomPostProcessingShaderConstants._LensColor, m_LensFilter.LensColor.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, lensFilter);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("LensFilter");
            
            int indensityKeyword = Shader.PropertyToID("_LensFilterIndensity");
            int lensColorKeyword = Shader.PropertyToID("_LensColor");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalFloat(indensityKeyword, m_LensFilter.Indensity.value);
            cmd.SetGlobalColor(lensColorKeyword, m_LensFilter.LensColor.value);
            Draw(cmd, lensFilter, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region WhiteBalance

        private void SetupWhiteBalance(CommandBuffer cmd, ref RenderingData renderingData, Material whiteBalance, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("WhiteBalance");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // whiteBalance.SetFloat(CustomPostProcessingShaderConstants._Temperature, m_WhiteBalance.temperature.value);
            // whiteBalance.SetFloat(CustomPostProcessingShaderConstants._Tint, m_WhiteBalance.tint.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, whiteBalance);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("WhiteBalance");
            
            int temperatureKeyword = Shader.PropertyToID("_WhiteBalanceTemperature");
            int tintKeyword = Shader.PropertyToID("_WhiteBalanceTint");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalFloat(temperatureKeyword, m_WhiteBalance.temperature.value);
            cmd.SetGlobalFloat(tintKeyword, m_WhiteBalance.tint.value);
            Draw(cmd, whiteBalance, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region Tint

        private void SetupTint(CommandBuffer cmd, ref RenderingData renderingData, Material tint, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Tint");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // tint.SetFloat(CustomPostProcessingShaderConstants._Indensity, m_Tint.indensity.value);
            // tint.SetColor(CustomPostProcessingShaderConstants._ColorTint, m_Tint.colorTint.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, tint);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Tint");
            
            int indensityKeyword = Shader.PropertyToID("_TintIndensity");
            int colorTintKeyword = Shader.PropertyToID("_ColorTint");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalFloat(indensityKeyword, m_Tint.indensity.value);
            cmd.SetGlobalColor(colorTintKeyword, m_Tint.colorTint.value);
            Draw(cmd, tint, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region Hue

        private void SetupHue(CommandBuffer cmd, ref RenderingData renderingData, Material hue, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Hue");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // hue.SetFloat(CustomPostProcessingShaderConstants._HueDegree, m_Hue.HueDegree.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, hue);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Hue");
            
            int hueDegreeKeyword = Shader.PropertyToID("_HueDegree");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalFloat(hueDegreeKeyword, m_Hue.HueDegree.value);
            Draw(cmd, hue, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region Brightness

        private void SetupBrightness(CommandBuffer cmd, ref RenderingData renderingData, Material brightness, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Brightness");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // brightness.SetFloat(CustomPostProcessingShaderConstants._Brightness, m_Brightness.brightness.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, brightness);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Brightness");
            
            int _brightnessKeyword = Shader.PropertyToID("_Brightness");
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalFloat(_brightnessKeyword, m_Brightness.brightness.value);
            Draw(cmd, brightness, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region BleachBypass

        private void SetupBleachBypass(CommandBuffer cmd, ref RenderingData renderingData, Material bleachBypass, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("BleachBypass");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // bleachBypass.SetFloat(CustomPostProcessingShaderConstants._Indensity, m_BleachBypass.Indensity.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, bleachBypass);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("BleachBypass");
            
            int indensityKeyword = Shader.PropertyToID("_BleachBypassIndensity");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalFloat(indensityKeyword, m_BleachBypass.Indensity.value);
            Draw(cmd, bleachBypass, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region Sharpen

        private void SetupSharpenV3(CommandBuffer cmd, ref RenderingData renderingData, Material v3, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("SharpenV3");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // Vector2 p = new Vector2(1.0f + (3.2f * m_V3.Sharpness.value), 0.8f * m_V3.Sharpness.value);
            // v3.SetVector(CustomPostProcessingShaderConstants._Params, p);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, v3);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("SharpenV3");
            
            int parametersKeyword = Shader.PropertyToID("_SharpenV3");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            Vector2 p = new Vector2(1.0f + (3.2f * m_V3.Sharpness.value), 0.8f * m_V3.Sharpness.value);
            cmd.SetGlobalVector(parametersKeyword, p);
            Draw(cmd, v3, m_TemporaryColorTexture01, destination, 0);
        }

        private void SetupSharpenV2(CommandBuffer cmd, ref RenderingData renderingData, Material v2, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("SharpenV2");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // v2.SetFloat(CustomPostProcessingShaderConstants._Sharpness, m_V2.Sharpness.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, v2);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("SharpenV2");
            
            int parametersKeyword = Shader.PropertyToID("_SharpenV2");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalFloat(parametersKeyword, m_V2.Sharpness.value);
            Draw(cmd, v2, m_TemporaryColorTexture01, destination, 0);
        }

        private void SetupSharpenV1(CommandBuffer cmd, ref RenderingData renderingData, Material v1, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("SharpenV1");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // v1.SetVector(CustomPostProcessingShaderConstants._Params, new Vector2(m_V1.Strength.value, m_V1.Threshold.value));
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, v1);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("SharpenV1");
            
            int parametersKeyword = Shader.PropertyToID("_SharpenV1");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalVector(parametersKeyword, new Vector2(m_V1.Strength.value, m_V1.Threshold.value));
            Draw(cmd, v1, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region RapidV2

        private void SetupRapidV2(CommandBuffer cmd, ref RenderingData renderingData, Material rapidV2, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Rapid");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // rapidV2.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_RapidV2.vignetteIndensity.value, m_RapidV2.vignetteSharpness.value, m_RapidV2.vignetteCenter.value.x, m_RapidV2.vignetteCenter.value.y));
            // if (m_RapidV2.vignetteType.value == VignetteType.ColorMode)
            // {
            //     rapidV2.SetColor(CustomPostProcessingShaderConstants._VignetteColor, m_RapidV2.vignetteColor.value);
            // }
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, rapidV2, (int)m_RapidV2.vignetteType.value);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Rapid");
            
            int parametersKeyword = Shader.PropertyToID("_RapidV2Parameters");
            int colorKeyword = Shader.PropertyToID("_RapidV2Color");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            cmd.SetGlobalVector(parametersKeyword, new Vector4(m_RapidV2.vignetteIndensity.value, m_RapidV2.vignetteSharpness.value, m_RapidV2.vignetteCenter.value.x, m_RapidV2.vignetteCenter.value.y));
            if (m_RapidV2.vignetteType.value == VignetteType.ColorMode)
            {
                cmd.SetGlobalColor(colorKeyword, m_RapidV2.vignetteColor.value);
            }
            
            Draw(cmd, rapidV2, m_TemporaryColorTexture01, destination, (int)m_RapidV2.vignetteType.value);
        }

        #endregion


        #region Rapid

        private void SetupRapid(CommandBuffer cmd, ref RenderingData renderingData, Material rapid, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Rapid");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // rapid.SetVector(CustomPostProcessingShaderConstants._Params, new Vector3(m_Rapid.vignetteIndensity.value, m_Rapid.vignetteCenter.value.x, m_Rapid.vignetteCenter.value.y));
            // if (m_Rapid.vignetteType.value == VignetteType.ColorMode)
            // {
            //     rapid.SetColor(CustomPostProcessingShaderConstants._VignetteColor, m_Rapid.vignetteColor.value);
            // }
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, rapid, (int)m_Rapid.vignetteType.value);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Rapid");
            
            int parametersKeyword = Shader.PropertyToID("_RapidParameters");
            int colorKeyword = Shader.PropertyToID("_RapidColor");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            cmd.SetGlobalVector(parametersKeyword, new Vector3(m_Rapid.vignetteIndensity.value, m_Rapid.vignetteCenter.value.x, m_Rapid.vignetteCenter.value.y));
            if (m_Rapid.vignetteType.value == VignetteType.ColorMode)
            {
                cmd.SetGlobalColor(colorKeyword, m_Rapid.vignetteColor.value);
            }
            
            Draw(cmd, rapid, m_TemporaryColorTexture01, destination, (int)m_Rapid.vignetteType.value);
        }

        #endregion


        #region RapidOldTVV2

        private void SetupRapidOldTVV2(CommandBuffer cmd, ref RenderingData renderingData, Material rapidOldTvv2, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("RapidOldTVV2");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // rapidOldTvv2.SetVector(CustomPostProcessingShaderConstants._Params, new Vector2(m_RapidOldTVV2.vignetteSize.value, m_RapidOldTVV2.sizeOffset.value));
            // if (m_RapidOldTVV2.vignetteType.value == VignetteType.ColorMode)
            // {
            //     rapidOldTvv2.SetColor(CustomPostProcessingShaderConstants._VignetteColor, m_RapidOldTVV2.vignetteColor.value);
            // }
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, rapidOldTvv2, (int)m_RapidOldTVV2.vignetteType.value);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("RapidOldTVV2");
            
            int parametersKeyword = Shader.PropertyToID("_RapidOldTVV2Parameters");
            int colorKeyword = Shader.PropertyToID("_RapidOldTVV2Color");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            cmd.SetGlobalVector(parametersKeyword, new Vector2(m_RapidOldTVV2.vignetteSize.value, m_RapidOldTVV2.sizeOffset.value));
            if (m_RapidOldTVV2.vignetteType.value == VignetteType.ColorMode)
            {
                cmd.SetGlobalColor(colorKeyword, m_RapidOldTVV2.vignetteColor.value);
            }
            
            Draw(cmd, rapidOldTvv2, m_TemporaryColorTexture01, destination, (int)m_RapidOldTVV2.vignetteType.value);
        }

        #endregion


        #region RapidOldTV

        private void SetupRapidOldTV(CommandBuffer cmd, ref RenderingData renderingData, Material rapidOldTV, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("RapidOldTV");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // rapidOldTV.SetVector(CustomPostProcessingShaderConstants._Params, new Vector3(m_RapidOldTV.vignetteIndensity.value, m_RapidOldTV.vignetteCenter.value.x, m_RapidOldTV.vignetteCenter.value.y));
            // if (m_RapidOldTV.vignetteType.value == VignetteType.ColorMode)
            // {
            //     rapidOldTV.SetColor(CustomPostProcessingShaderConstants._VignetteColor, m_RapidOldTV.vignetteColor.value);
            // }
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, rapidOldTV, (int)m_RapidOldTV.vignetteType.value);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("RapidOldTV");
            
            int parametersKeyword = Shader.PropertyToID("_RapidOldTVParameters");
            int colorKeyword = Shader.PropertyToID("_RapidOldTVColor");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            cmd.SetGlobalVector(parametersKeyword, new Vector3(m_RapidOldTV.vignetteIndensity.value, m_RapidOldTV.vignetteCenter.value.x, m_RapidOldTV.vignetteCenter.value.y));
            if (m_RapidOldTV.vignetteType.value == VignetteType.ColorMode)
            {
                cmd.SetGlobalColor(colorKeyword, m_RapidOldTV.vignetteColor.value);
            }
            
            Draw(cmd, rapidOldTV, m_TemporaryColorTexture01, destination, (int)m_RapidOldTV.vignetteType.value);

        }

        #endregion


        #region Aurora

        private void SetupAurora(CommandBuffer cmd, ref RenderingData renderingData, Material aurora, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Aurora");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // TimeX += Time.deltaTime;
            // if (TimeX > 100)
            // {
            //     TimeX = 0;
            // }
            // aurora.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_Aurora.vignetteArea.value, m_Aurora.vignetteSmothness.value, m_Aurora.colorChange.value, TimeX));
            // aurora.SetVector(CustomPostProcessingShaderConstants._Params2, new Vector4(m_Aurora.colorFactorR.value, m_Aurora.colorFactorG.value,m_Aurora.colorFactorB.value, m_Aurora.vignetteFading.value));
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, aurora);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Aurora");
            
            int parametersKeyword = Shader.PropertyToID("_AuroraParameters");
            int parameters2Keyword = Shader.PropertyToID("_AuroraParameters2");
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            _TimeX += Time.deltaTime;
            if (_TimeX > 100)
            {
                _TimeX = 0;
            }
            cmd.SetGlobalVector(parametersKeyword, new Vector4(m_Aurora.vignetteArea.value, m_Aurora.vignetteSmothness.value, m_Aurora.colorChange.value, _TimeX * m_Aurora.flowSpeed.value));
            cmd.SetGlobalVector(parameters2Keyword, new Vector4(m_Aurora.colorFactorR.value, m_Aurora.colorFactorG.value,m_Aurora.colorFactorB.value, m_Aurora.vignetteFading.value));
            
            Draw(cmd, aurora, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region Triangle

        private void SetupTriangle(CommandBuffer cmd, ref RenderingData renderingData, Material triangle, in RTHandle source, in RTHandle destination)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Triangle");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // float size = (1.01f - m_Triangle.pixelSize.value) * 5f;
            //
            // float ratio = m_Triangle.pixelRatio.value;
            // if (m_Triangle.useAutoScreenRatio.value)
            // {
            //     ratio = (float)(opaqueDesc.width / (float)opaqueDesc.height);
            //     if (ratio == 0)
            //     {
            //         ratio = 1f;
            //     }
            // }
            // triangle.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(size, ratio, m_Triangle.pixelScaleX.value * 20, m_Triangle.pixelScaleY.value * 20));
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, triangle);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Triangle");
            
            int paramsKeyword = Shader.PropertyToID("_TriangleParams");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            float size = (1.01f - m_Triangle.pixelSize.value) * 5f;

            float ratio = m_Triangle.pixelRatio.value;
            if (m_Triangle.useAutoScreenRatio.value)
            {
                ratio = (opaqueDesc.width / (float)opaqueDesc.height);
                if (ratio == 0)
                {
                    ratio = 1f;
                }
            }
            cmd.SetGlobalVector(paramsKeyword, new Vector4(size, ratio, m_Triangle.pixelScaleX.value * 20, m_Triangle.pixelScaleY.value * 20));
            
            Draw(cmd, triangle, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region Sector

        private void SetupSector(CommandBuffer cmd, ref RenderingData renderingData, Material sector, in RTHandle source, in RTHandle destination)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Sector");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // float size = (1.01f - m_Sector.pixelSize.value) * 300f;
            // Vector4 parameters = new Vector4(size, ((opaqueDesc.width * 2 / (float)opaqueDesc.height) * size / Mathf.Sqrt(3f)), m_Sector.circleRadius.value, 0f);
            // sector.SetVector(CustomPostProcessingShaderConstants._Params, parameters);
            // sector.SetVector(CustomPostProcessingShaderConstants._Params2, new Vector2(m_Sector.pixelIntervalX.value, m_Sector.pixelIntervalY.value));
            // sector.SetColor(CustomPostProcessingShaderConstants._BackgroundColor, m_Sector.backgroundColor.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, sector);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Sector");
            
            int paramsKeyword = Shader.PropertyToID("_SectorParams");
            int params2Keyword = Shader.PropertyToID("_SectorParams2");
            int backgroundKeyword = Shader.PropertyToID("_SectorBackground");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            float size = (1.01f - m_Sector.pixelSize.value) * 300f;
            Vector4 parameters = new Vector4(size, ((opaqueDesc.width * 2 / (float)opaqueDesc.height) * size / Mathf.Sqrt(3f)), m_Sector.circleRadius.value, 0f);
            cmd.SetGlobalVector(paramsKeyword, parameters);
            cmd.SetGlobalVector(params2Keyword, new Vector2(m_Sector.pixelIntervalX.value, m_Sector.pixelIntervalY.value));
            cmd.SetGlobalColor(backgroundKeyword, m_Sector.backgroundColor.value);
            
            Draw(cmd, sector, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region Quad

        private void SetupQuad(CommandBuffer cmd, ref RenderingData renderingData, Material quad, in RTHandle source, in RTHandle destination)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Quad");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // float size = (1.01f - m_Quad.pixelSize.value) * 200f;
            // quad.SetFloat(CustomPostProcessingShaderConstants._PixelSize, size);
            // float ratio = m_Quad.pixelRatio.value;
            // if (m_Quad.useAutoScreenRatio.value)
            // {
            //     ratio = (float)(opaqueDesc.width / (float)opaqueDesc.height) ;
            //     if (ratio==0)
            //     {
            //         ratio = 1f;
            //     }
            // }
            // quad.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(size, ratio, m_Quad.pixelScaleX.value, m_Quad.pixelScaleY.value));
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, quad);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Quad");
            
            int paramsKeyword = Shader.PropertyToID("_QuadParams");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            float size = (1.01f - m_Quad.pixelSize.value) * 200f;
            float ratio = m_Quad.pixelRatio.value;
            if (m_Quad.useAutoScreenRatio.value)
            {
                ratio = opaqueDesc.width / (float)opaqueDesc.height;
                if (ratio==0)
                {
                    ratio = 1f;
                }
            }
            cmd.SetGlobalVector(paramsKeyword, new Vector4(size, ratio, m_Quad.pixelScaleX.value, m_Quad.pixelScaleY.value));
            
            Draw(cmd, quad, m_TemporaryColorTexture01, destination, 0);
        }


        #endregion


        #region Led

        private void SetupLed(CommandBuffer cmd, ref RenderingData renderingData, Material led, in RTHandle source, in RTHandle destination)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Led");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // float size = (1.01f - m_Led.pixelSize.value) * 300f;
            //
            // float ratio = m_Led.pixelRatio.value;
            // if (m_Led.useAutoScreenRatio.value)
            // {
            //     ratio = (float)(opaqueDesc.width / (float)opaqueDesc.height);
            //     if (ratio == 0)
            //     {
            //         ratio = 1f;
            //     }
            // }
            // led.SetVector(CustomPostProcessingShaderConstants._Params, new Vector3(size, ratio, m_Led.ledRadius.value));
            // led.SetColor(CustomPostProcessingShaderConstants._BackgroundColor, m_Led.backgroundColor.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, led);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Led");
            
            int paramsKeyword = Shader.PropertyToID("_LedParams");
            int backgroundKeyword = Shader.PropertyToID("_LedBackground");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            float size = (1.01f - m_Led.pixelSize.value) * 300f;

            float ratio = m_Led.pixelRatio.value;
            if (m_Led.useAutoScreenRatio.value)
            {
                ratio = (opaqueDesc.width / (float)opaqueDesc.height);
                if (ratio == 0)
                {
                    ratio = 1f;
                }
            }
            cmd.SetGlobalVector(paramsKeyword, new Vector3(size, ratio, m_Led.ledRadius.value));
            cmd.SetGlobalColor(backgroundKeyword, m_Led.backgroundColor.value);
            
            Draw(cmd, led, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region Leaf

        private void SetupLeaf(CommandBuffer cmd, ref RenderingData renderingData, Material leaf, in RTHandle source, in RTHandle destination)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Leaf");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // float size = (1.01f - m_Leaf.pixelSize.value) * 10f;
            //
            // float ratio = m_Leaf.pixelRatio.value;
            // if (m_Leaf.useAutoScreenRatio.value)
            // {
            //     ratio = (float)(opaqueDesc.width / (float)opaqueDesc.height);
            //     if (ratio == 0)
            //     {
            //         ratio = 1f;
            //     }
            // }
            // leaf.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(size,ratio, m_Leaf.pixelScaleX.value * 20,m_Leaf.pixelScaleY.value * 20));
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, leaf);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Leaf");
            
            int paramsKeyword = Shader.PropertyToID("_LeafParams");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            float size = (1.01f - m_Leaf.pixelSize.value) * 10f;

            float ratio = m_Leaf.pixelRatio.value;
            if (m_Leaf.useAutoScreenRatio.value)
            {
                ratio = opaqueDesc.width / (float)opaqueDesc.height;
                if (ratio == 0)
                {
                    ratio = 1f;
                }
            }
            cmd.SetGlobalVector(paramsKeyword, new Vector4(size,ratio, m_Leaf.pixelScaleX.value * 20,m_Leaf.pixelScaleY.value * 20));
            
            Draw(cmd, leaf, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region HexagonGrid

        private void SetupHexagonGrid(CommandBuffer cmd, ref RenderingData renderingData, Material hexagonGrid, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("HexagonGrid");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // hexagonGrid.SetVector(CustomPostProcessingShaderConstants._Params, new Vector2(m_HexagonGrid.pixelSize.value, m_HexagonGrid.gridWidth.value));
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, hexagonGrid);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("HexagonGrid");
            
            int paramsKeyword = Shader.PropertyToID("_HexagonGridParams");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            cmd.SetGlobalVector(paramsKeyword, new Vector2(m_HexagonGrid.pixelSize.value, m_HexagonGrid.gridWidth.value));
            
            Draw(cmd, hexagonGrid, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region Hexagon

        private void SetupHexagon(CommandBuffer cmd, ref RenderingData renderingData, Material hexagon, in RTHandle source, in RTHandle destination)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Hexagon");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // hexagon.SetFloat(CustomPostProcessingShaderConstants._PixelSize, m_Hexagon.pixelSize.value);
            // float size = m_Hexagon.pixelSize.value * 0.2f;
            // float ratio = m_Hexagon.pixelRatio.value;
            // if (m_Hexagon.useAutoScreenRatio.value)
            // {
            //     ratio = (float)(opaqueDesc.width / (float)opaqueDesc.height);
            //     if (ratio == 0)
            //     {
            //         ratio = 1f;
            //     }
            // }
            // hexagon.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(size,ratio, m_Hexagon.pixelScaleX.value,m_Hexagon.pixelScaleY.value));
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, hexagon);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Hexagon");
            
            int paramsKeyword = Shader.PropertyToID("_HexagonParams");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            // cmd.SetGlobalFloat(_pixelSizeKeyword, pixelSize.value);
            float size = m_Hexagon.pixelSize.value * 0.2f;
            float ratio = m_Hexagon.pixelRatio.value;
            if (m_Hexagon.useAutoScreenRatio.value)
            {
                ratio = (opaqueDesc.width / (float)opaqueDesc.height);
                if (ratio == 0)
                {
                    ratio = 1f;
                }
            }
            cmd.SetGlobalVector(paramsKeyword, new Vector4(size,ratio, m_Hexagon.pixelScaleX.value,m_Hexagon.pixelScaleY.value));
            
            Draw(cmd, hexagon, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region Diamond

        private void SetupDiamond(CommandBuffer cmd, ref RenderingData renderingData, Material diamond, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Diamond");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // diamond.SetFloat(CustomPostProcessingShaderConstants._PixelSize, m_Diamond.pixelSize.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, diamond);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Diamond");
            
            int pixelSizeKeyword = Shader.PropertyToID("_DiamondPixelSize");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            cmd.SetGlobalFloat(pixelSizeKeyword, m_Diamond.pixelSize.value);
            
            Draw(cmd, diamond, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region Circle

        private void SetupCircle(CommandBuffer cmd, ref RenderingData renderingData, Material circle, in RTHandle source, in RTHandle destination)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Circle");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // float size = (1.01f - m_Circle.pixelSize.value) * 300f;
            // Vector4 parameters = new Vector4(size, ((opaqueDesc.width * 2 / (float)opaqueDesc.height) * size / Mathf.Sqrt(3f)), m_Circle.circleRadius.value, 0f);
            // circle.SetVector(CustomPostProcessingShaderConstants._Params, parameters);
            // circle.SetVector(CustomPostProcessingShaderConstants._Params2, new Vector2(m_Circle.pixelIntervalX.value, m_Circle.pixelIntervalY.value));
            // circle.SetColor(CustomPostProcessingShaderConstants._BackgroundColor, m_Circle.backgroundColor.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, circle);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Circle");
            
            int paramsKeyword = Shader.PropertyToID("_CircleParams");
            int params2Keyword = Shader.PropertyToID("_CircleParams2");
            int backgroundKeyword = Shader.PropertyToID("_CircleBackground");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            float size = (1.01f - m_Circle.pixelSize.value) * 300f;
            Vector4 parameters = new Vector4(size, ((opaqueDesc.width * 2 / (float)opaqueDesc.height) * size / Mathf.Sqrt(3f)), m_Circle.circleRadius.value, 0f);
            cmd.SetGlobalVector(paramsKeyword, parameters);
            cmd.SetGlobalVector(params2Keyword, new Vector2(m_Circle.pixelIntervalX.value, m_Circle.pixelIntervalY.value));
            cmd.SetGlobalColor(backgroundKeyword, m_Circle.backgroundColor.value);
            
            Draw(cmd, circle, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region SobelNeon

        private void SetupSobelNeon(CommandBuffer cmd, ref RenderingData renderingData, Material sobelNeon, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("SobelNeon");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // sobelNeon.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_SobelNeon.edgeWidth.value, m_SobelNeon.edgeNeonFade.value, m_SobelNeon.brightness.value, m_SobelNeon.backgroundFade.value));
            // sobelNeon.SetColor(CustomPostProcessingShaderConstants._BackgroundColor, m_SobelNeon.backgroundColor.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, sobelNeon);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("SobelNeon");
            
            int paramsKeyword = Shader.PropertyToID("_SobelNeonParams");
            int backgroundKeyword = Shader.PropertyToID("_SobelNeonBackgroundColor");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalVector(paramsKeyword, new Vector4(m_SobelNeon.edgeWidth.value, m_SobelNeon.edgeNeonFade.value, m_SobelNeon.brightness.value, m_SobelNeon.backgroundFade.value));
            cmd.SetGlobalColor(backgroundKeyword, m_SobelNeon.backgroundColor.value);
            Draw(cmd, sobelNeon, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region Sobel

        private void SetupSobel(CommandBuffer cmd, ref RenderingData renderingData, Material sobel, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Sobel");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // sobel.SetVector(CustomPostProcessingShaderConstants._Params, new Vector2(m_Sobel.edgeWidth.value, m_Sobel.backgroundFade.value));
            // sobel.SetColor(CustomPostProcessingShaderConstants._EdgeColor, m_Sobel.edgeColor.value);
            // sobel.SetColor(CustomPostProcessingShaderConstants._BackgroundColor, m_Sobel.backgroundColor.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, sobel);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Sobel");
            
            int paramsKeyword = Shader.PropertyToID("_SobelParams");
            int edgeColorKeyword = Shader.PropertyToID("_SobelEdgeColor");
            int backgroundKeyword = Shader.PropertyToID("_SobelBackgroundColor");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalVector(paramsKeyword, new Vector2(m_Sobel.edgeWidth.value, m_Sobel.backgroundFade.value));
            cmd.SetGlobalColor(edgeColorKeyword, m_Sobel.edgeColor.value);
            cmd.SetGlobalColor(backgroundKeyword, m_Sobel.backgroundColor.value);
            Draw(cmd, sobel, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region ScharrNeon

        private void SetupScharrNeon(CommandBuffer cmd, ref RenderingData renderingData, Material scharrNeon, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("ScharrNeon");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // scharrNeon.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_ScharrNeon.edgeWidth.value, m_ScharrNeon.edgeNeonFade.value, m_ScharrNeon.brightness.value, m_ScharrNeon.backgroundFade.value));
            // scharrNeon.SetColor(CustomPostProcessingShaderConstants._BackgroundColor, m_ScharrNeon.backgroundColor.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, scharrNeon);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("ScharrNeon");
            
            int paramsKeyword = Shader.PropertyToID("_ScharrNeonParams");
            int backgroundKeyword = Shader.PropertyToID("_ScharrNeonBackgroundColor");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalVector(paramsKeyword, new Vector4(m_ScharrNeon.edgeWidth.value, m_ScharrNeon.edgeNeonFade.value, m_ScharrNeon.brightness.value, m_ScharrNeon.backgroundFade.value));
            cmd.SetGlobalColor(backgroundKeyword, m_ScharrNeon.backgroundColor.value);
            Draw(cmd, scharrNeon, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region Scharr

        private void SetupScharr(CommandBuffer cmd, ref RenderingData renderingData, Material scharr, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Scharr");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // scharr.SetVector(CustomPostProcessingShaderConstants._Params, new Vector2(m_Scharr.edgeWidth.value, m_Scharr.backgroundFade.value));
            // scharr.SetColor(CustomPostProcessingShaderConstants._EdgeColor, m_Scharr.edgeColor.value);
            // scharr.SetColor(CustomPostProcessingShaderConstants._BackgroundColor, m_Scharr.backgroundColor.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, scharr);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Scharr");
            
            int paramsKeyword = Shader.PropertyToID("_ScharrParams");
            int edgeColorKeyword = Shader.PropertyToID("_ScharrEdgeColor");
            int backgroundKeyword = Shader.PropertyToID("_ScharrBackgroundColor");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalVector(paramsKeyword, new Vector2(m_Scharr.edgeWidth.value, m_Scharr.backgroundFade.value));
            cmd.SetGlobalColor(edgeColorKeyword, m_Scharr.edgeColor.value);
            cmd.SetGlobalColor(backgroundKeyword, m_Scharr.backgroundColor.value);
            Draw(cmd, scharr, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region RobertsNeon

        private void SetupRobertsNeon(CommandBuffer cmd, ref RenderingData renderingData, Material robertsNeon, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("RobertsNeon");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // robertsNeon.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_RobertsNeon.edgeWidth.value, m_RobertsNeon.edgeNeonFade.value, m_RobertsNeon.brightness.value, m_RobertsNeon.backgroundFade.value));
            // robertsNeon.SetColor(CustomPostProcessingShaderConstants._BackgroundColor, m_RobertsNeon.backgroundColor.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, robertsNeon);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("RobertsNeon");
            
            int paramsKeyword = Shader.PropertyToID("_RobertsNeonParams");
            int backgroundKeyword = Shader.PropertyToID("_RobertsNeonBackgroundColor");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalVector(paramsKeyword, new Vector4(m_RobertsNeon.edgeWidth.value, m_RobertsNeon.edgeNeonFade.value, m_RobertsNeon.brightness.value, m_RobertsNeon.backgroundFade.value));
            cmd.SetGlobalColor(backgroundKeyword, m_RobertsNeon.backgroundColor.value);
            Draw(cmd, robertsNeon, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region Roberts

        private void SetupRoberts(CommandBuffer cmd, ref RenderingData renderingData, Material roberts, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("Roberts");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // roberts.SetVector(CustomPostProcessingShaderConstants._Params, new Vector2(m_Roberts.edgeWidth.value, m_Roberts.backgroundFade.value));
            // roberts.SetColor(CustomPostProcessingShaderConstants._EdgeColor, m_Roberts.edgeColor.value);
            // roberts.SetColor(CustomPostProcessingShaderConstants._BackgroundColor, m_Roberts.backgroundColor.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, roberts);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("Roberts");
            
            int paramsKeyword = Shader.PropertyToID("_RobertsParams");
            int edgeColorKeyword = Shader.PropertyToID("_RobertsEdgeColor");
            int backgroundKeyword = Shader.PropertyToID("_RobertsBackgroundColor");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalVector(paramsKeyword, new Vector2(m_Roberts.edgeWidth.value, m_Roberts.backgroundFade.value));
            cmd.SetGlobalColor(edgeColorKeyword, m_Roberts.edgeColor.value);
            cmd.SetGlobalColor(backgroundKeyword, m_Roberts.backgroundColor.value);
            Draw(cmd, roberts, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region WaveJitter

        private void SetupWaveJitter(CommandBuffer cmd, ref RenderingData renderingData, Material waveJitter, in RTHandle source, in RTHandle destination)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("WaveJitter");
            // UpdateFrequencyWJ(waveJitter);
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // waveJitter.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_WaveJitter.IntervalType.value == IntervalType.Random ? randomFrequency : m_WaveJitter.Frequency.value, m_WaveJitter.RGBSplit.value , m_WaveJitter.Speed.value, m_WaveJitter.Amount.value));
            // waveJitter.SetVector(CustomPostProcessingShaderConstants._Resolution, m_WaveJitter.CustomResolution.value ? m_WaveJitter.Resolution.value : new Vector2(opaqueDesc.width,opaqueDesc.height));
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, waveJitter, (int)m_WaveJitter.JitterDirection.value);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("WaveJitter");
            
            int paramsKeyword = Shader.PropertyToID("_WaveJitterParams");
            int resolutionKeyword = Shader.PropertyToID("_WaveJitterResolution");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            UpdateFrequencyWJ(waveJitter);
            cmd.SetGlobalVector(paramsKeyword, new Vector4(m_WaveJitter.IntervalType.value == IntervalType.Random ? _randomFrequency : m_WaveJitter.Frequency.value, m_WaveJitter.RGBSplit.value , m_WaveJitter.Speed.value, m_WaveJitter.Amount.value));
            cmd.SetGlobalVector(resolutionKeyword, m_WaveJitter.CustomResolution.value ? m_WaveJitter.Resolution.value : new Vector2(opaqueDesc.width,opaqueDesc.height));
            
            Draw(cmd, waveJitter, m_TemporaryColorTexture01, destination, (int)m_WaveJitter.JitterDirection.value);
        }

        private void UpdateFrequencyWJ(Material waveJitter)
        {
            if (m_WaveJitter.IntervalType.value == IntervalType.Random)
            {
                _randomFrequency = UnityEngine.Random.Range(0, m_WaveJitter.Frequency.value);
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

        private void SetupScreenShake(CommandBuffer cmd, ref RenderingData renderingData, Material screenShake, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("ScreenShake");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // screenShake.SetFloat(CustomPostProcessingShaderConstants._ScreenShake, m_ScreenShake.ScreenShakeIndensity.value);
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, screenShake, (int)m_ScreenShake.ScreenShakeDirection.value);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("ScreenShake");
            
            int paramsKeyword = Shader.PropertyToID("_ScreenShakeParams");
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            cmd.SetGlobalFloat(paramsKeyword, m_ScreenShake.ScreenShakeIndensity.value);
            
            Draw(cmd, screenShake, m_TemporaryColorTexture01, destination, (int)m_ScreenShake.ScreenShakeDirection.value);
        }

        #endregion


        #region ScreenJump

        private void SetupScreenJump(CommandBuffer cmd, ref RenderingData renderingData, Material screenJump, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("ScreenJump");
            // ScreenJumpTime += Time.deltaTime * m_ScreenJump.ScreenJumpIndensity.value * 9.8f;
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // screenJump.SetVector(CustomPostProcessingShaderConstants._Params, new Vector2(m_ScreenJump.ScreenJumpIndensity.value, m_ScreenJump.isHorizontalReverse.value ? -ScreenJumpTime : ScreenJumpTime));
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, screenJump, (int)m_ScreenJump.ScreenJumpDirection.value);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("ScreenJump");
            
            int paramsKeyword = Shader.PropertyToID("_ScanLineJitterParams");
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            _screenJumpTime += Time.deltaTime * m_ScreenJump.ScreenJumpIndensity.value * 9.8f;
            cmd.SetGlobalVector(paramsKeyword, new Vector2(m_ScreenJump.ScreenJumpIndensity.value, m_ScreenJump.isHorizontalReverse.value ? -_screenJumpTime : _screenJumpTime));
            
            Draw(cmd, screenJump, m_TemporaryColorTexture01, destination, (int)m_ScreenJump.ScreenJumpDirection.value);
        }

        #endregion


        #region AnalogNoise

        private void SetupAnalogNoise(CommandBuffer cmd, ref RenderingData renderingData, Material analogNoise, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("AnalogNoise");
            // TimeX += Time.deltaTime;
            // if (TimeX > 100)
            // {
            //     TimeX = 0;
            // }
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // analogNoise.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_AnalogNoise.NoiseSpeed.value, m_AnalogNoise.NoiseFading.value, m_AnalogNoise.LuminanceJitterThreshold.value, TimeX));
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, analogNoise);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("AnalogNoise");
            
            int paramsKeyword = Shader.PropertyToID("_AnalogNoiseParams");
            _TimeX += Time.deltaTime;
            if (_TimeX > 100)
            {
                _TimeX = 0;
            }
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            cmd.SetGlobalVector(paramsKeyword, new Vector4(m_AnalogNoise.NoiseSpeed.value, m_AnalogNoise.NoiseFading.value, m_AnalogNoise.LuminanceJitterThreshold.value, _TimeX));
            
            Draw(cmd, analogNoise, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region DigitalStripe

        private void SetupDigitalStripe(CommandBuffer cmd, ref RenderingData renderingData, Material digitalStripe, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("DigitalStripe");
            // UpdateFrequencyDS(m_DigitalStripe.frequency.value, m_DigitalStripe.noiseTextureWidth.value, m_DigitalStripe.noiseTextureHeight.value, m_DigitalStripe.stripeLength.value);
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // digitalStripe.SetFloat(CustomPostProcessingShaderConstants._Indensity, m_DigitalStripe.indensity.value);
            // if (_noiseTexture != null)
            // {
            //     digitalStripe.SetTexture(CustomPostProcessingShaderConstants._NoiseTex, _noiseTexture);
            // }
            // if (m_DigitalStripe.needStripColorAdjust.value)
            // {
            //     digitalStripe.EnableKeyword("NEED_TRASH_FRAME");
            //     digitalStripe.SetColor(CustomPostProcessingShaderConstants._StripColorAdjustColor, m_DigitalStripe.stripColorAdjustColor.value);
            //     digitalStripe.SetFloat(CustomPostProcessingShaderConstants._StripColorAdjustIndensity, m_DigitalStripe.stripColorAdjustIndensity.value);
            // }
            // else
            // {
            //     digitalStripe.DisableKeyword("NEED_TRASH_FRAME");
            // }
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, digitalStripe);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("DigitalStripe");
            
            int colorKeyword = Shader.PropertyToID("_StripColorAdjustColor");
            int indensityKeyword = Shader.PropertyToID("_DigitalStripeIndensity");
            int colorIndensityKeyword = Shader.PropertyToID("_StripColorAdjustIndensity");
            int texKeyword = Shader.PropertyToID("_DigitalStripeNoiseTex");
            
            UpdateFrequencyDS(m_DigitalStripe.frequency.value, m_DigitalStripe.noiseTextureWidth.value, m_DigitalStripe.noiseTextureHeight.value, m_DigitalStripe.stripeLength.value);
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalFloat(indensityKeyword, m_DigitalStripe.indensity.value);
            if (_noiseTexture != null)
            {
                cmd.SetGlobalTexture(texKeyword, _noiseTexture);
            }
            if (m_DigitalStripe.needStripColorAdjust.value)
            {
                digitalStripe.EnableKeyword("NEED_TRASH_FRAME");
                cmd.SetGlobalColor(colorKeyword, m_DigitalStripe.stripColorAdjustColor.value);
                cmd.SetGlobalFloat(colorIndensityKeyword, m_DigitalStripe.stripColorAdjustIndensity.value);
            }
            else
            {
                digitalStripe.DisableKeyword("NEED_TRASH_FRAME");
            }
            
            Draw(cmd, digitalStripe, m_TemporaryColorTexture01, destination, 0);
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

            // _trashFrame1 = new RenderTexture(Screen.width, Screen.height, 0);
            // _trashFrame2 = new RenderTexture(Screen.width, Screen.height, 0);
            // _trashFrame1.hideFlags = HideFlags.DontSave;
            // _trashFrame2.hideFlags = HideFlags.DontSave;

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

        private void SetupScanLineJitter(CommandBuffer cmd, ref RenderingData renderingData, Material scanLineJitter, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("ScanLineJitter");
            // UpdateFrequencySLJ(scanLineJitter);
            // float displacement = 0.005f + Mathf.Pow(m_ScanLineJitter.JitterIndensity.value, 3) * 0.1f;
            // float threshold = Mathf.Clamp01(1.0f - m_ScanLineJitter.JitterIndensity.value * 1.2f);
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // scanLineJitter.SetVector(CustomPostProcessingShaderConstants._Params, new Vector3(displacement, threshold, m_ScanLineJitter.IntervalType.value == IntervalType.Random ? randomFrequency : m_ScanLineJitter.Frequency.value));
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, scanLineJitter, (int)m_ScanLineJitter.JitterDirection.value);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("ScanLineJitter");
            
            int paramsKeyword = Shader.PropertyToID("_ScanLineJitterParams");

            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            UpdateFrequencySLJ(scanLineJitter);
            float displacement = 0.005f + Mathf.Pow(m_ScanLineJitter.JitterIndensity.value, 3) * 0.1f;
            float threshold = Mathf.Clamp01(1.0f - m_ScanLineJitter.JitterIndensity.value * 1.2f);
            cmd.SetGlobalVector(paramsKeyword, new Vector3(displacement, threshold, m_ScanLineJitter.IntervalType.value == IntervalType.Random ? _randomFrequency : m_ScanLineJitter.Frequency.value));
            
            Draw(cmd, scanLineJitter, m_TemporaryColorTexture01, destination, (int)m_ScanLineJitter.JitterDirection.value);
        }

        private void UpdateFrequencySLJ(Material scanLineJitter)
        {
            if (m_ScanLineJitter.IntervalType.value == IntervalType.Random)
            {
                _randomFrequency = UnityEngine.Random.Range(0, m_ScanLineJitter.Frequency.value);
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

        private void SetupTileJitter(CommandBuffer cmd, ref RenderingData renderingData, Material tileJitter, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("TileJitter");
            // UpdateFrequencyTJ(tileJitter);
            // if (m_TileJitter.JitterDirection.value == Direction.Horizontal)
            // {
            //     tileJitter.EnableKeyword("JITTER_DIRECTION_HORIZONTAL");
            // }
            // else
            // {
            //     tileJitter.DisableKeyword("JITTER_DIRECTION_HORIZONTAL");
            // }
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // tileJitter.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_TileJitter.SplittingNumber.value, m_TileJitter.Amount.value , m_TileJitter.Speed.value * 100f, m_TileJitter.IntervalType.value == IntervalType.Random ? randomFrequency : m_TileJitter.Frequency.value));
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, tileJitter, m_TileJitter.SplittingDirection.value == Direction.Horizontal ? 0 : 1);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("TileJitter");
            
            int paramsKeyword = Shader.PropertyToID("_TileJitterParams");

            UpdateFrequencyTJ(tileJitter);
            if (m_TileJitter.JitterDirection.value == Direction.Horizontal)
            {
                tileJitter.EnableKeyword("JITTER_DIRECTION_HORIZONTAL");
            }
            else
            {
                tileJitter.DisableKeyword("JITTER_DIRECTION_HORIZONTAL");
            }
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalVector(paramsKeyword, new Vector4(m_TileJitter.SplittingNumber.value, m_TileJitter.Amount.value , m_TileJitter.Speed.value * 100f, m_TileJitter.IntervalType.value == IntervalType.Random ? _randomFrequency : m_TileJitter.Frequency.value));
            
            Draw(cmd, tileJitter, m_TemporaryColorTexture01, destination, m_TileJitter.SplittingDirection.value == Direction.Horizontal ? 0 : 1);
        }

        private void UpdateFrequencyTJ(Material tileJitter)
        {
            if (m_TileJitter.IntervalType.value == IntervalType.Random)
            {
                _randomFrequency = UnityEngine.Random.Range(0, m_TileJitter.Frequency.value);
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

        private void SetupLineBlock(CommandBuffer cmd, ref RenderingData renderingData, Material lineBlock, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("LineBlock");
            // UpdateFrequency(lineBlock);
            // TimeX += Time.deltaTime;
            // if (TimeX > 100)
            // {
            //     TimeX = 0;
            // }
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // lineBlock.SetVector(CustomPostProcessingShaderConstants._Params, new Vector3(
            //     m_LineBlock.IntervalType.value == IntervalType.Random ? randomFrequency : m_LineBlock.Frequency.value,
            //     TimeX * m_LineBlock.Speed.value * 0.2f , m_LineBlock.Amount.value));
            // lineBlock.SetVector(CustomPostProcessingShaderConstants._Params2, new Vector3(m_LineBlock.Offset.value, 1 / m_LineBlock.LinesWidth.value, m_LineBlock.Alpha.value));
            // int pass = (int)m_LineBlock.BlockDirection.value;
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, lineBlock,pass);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("LineBlock");
            
            int paramsKeyword = Shader.PropertyToID("_LineBlockParams");
            int params2Keyword = Shader.PropertyToID("_LineBlockParams2");
            _TimeX += Time.deltaTime;
            if (_TimeX > 100)
            {
                _TimeX = 0;
            }
            
            UpdateFrequency(lineBlock);
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalVector(paramsKeyword, new Vector3(
                m_LineBlock.IntervalType.value == IntervalType.Random ? _randomFrequency : m_LineBlock.Frequency.value,
                _TimeX * m_LineBlock.Speed.value * 0.2f , m_LineBlock.Amount.value));
            cmd.SetGlobalVector(params2Keyword, new Vector3(m_LineBlock.Offset.value, 1 / m_LineBlock.LinesWidth.value, m_LineBlock.Alpha.value));
            int pass = (int)m_LineBlock.BlockDirection.value;
            
            Draw(cmd, lineBlock, m_TemporaryColorTexture01, destination, pass);
        }

        private void UpdateFrequency(Material lineBlock)
        {
            if (m_LineBlock.IntervalType.value == IntervalType.Random)
            {
                if (_frameCount > m_LineBlock.Frequency.value)
                {

                    _frameCount = 0;
                    _randomFrequency = UnityEngine.Random.Range(0, m_LineBlock.Frequency.value);
                }
                _frameCount++;
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

        private void SetupImageBlock(CommandBuffer cmd, ref RenderingData renderingData, Material imageBlock, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("ImageBlock");
            // TimeX += Time.deltaTime;
            // if (TimeX > 100)
            // {
            //     TimeX = 0;
            // }
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // imageBlock.SetVector(CustomPostProcessingShaderConstants._Params, new Vector3(TimeX * m_ImageBlock.Speed.value, m_ImageBlock.Amount.value, m_ImageBlock.Fade.value));
            // imageBlock.SetVector(CustomPostProcessingShaderConstants._Params2, new Vector4(m_ImageBlock.BlockLayer1_U.value, m_ImageBlock.BlockLayer1_V.value, m_ImageBlock.BlockLayer2_U.value, m_ImageBlock.BlockLayer2_V.value));
            // imageBlock.SetVector(CustomPostProcessingShaderConstants._Params3, new Vector3(m_ImageBlock.RGBSplitIndensity.value, m_ImageBlock.BlockLayer1_Indensity.value, m_ImageBlock.BlockLayer2_Indensity.value));
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, imageBlock);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("ImageBlock");
            
            int paramsKeyword = Shader.PropertyToID("_ImageBlockParams");
            int params2Keyword = Shader.PropertyToID("_ImageBlockParams2");
            int params3Keyword = Shader.PropertyToID("_ImageBlockParams3");
            _TimeX += Time.deltaTime;
            if (_TimeX > 100)
            {
                _TimeX = 0;
            }
            Draw(cmd, null, source, m_TemporaryColorTexture01);

            cmd.SetGlobalVector(paramsKeyword, new Vector3(_TimeX * m_ImageBlock.Speed.value, m_ImageBlock.Amount.value, m_ImageBlock.Fade.value));
            cmd.SetGlobalVector(params2Keyword, new Vector4(m_ImageBlock.BlockLayer1_U.value, m_ImageBlock.BlockLayer1_V.value, m_ImageBlock.BlockLayer2_U.value, m_ImageBlock.BlockLayer2_V.value));
            cmd.SetGlobalVector(params3Keyword, new Vector3(m_ImageBlock.RGBSplitIndensity.value, m_ImageBlock.BlockLayer1_Indensity.value, m_ImageBlock.BlockLayer2_Indensity.value));
            
            Draw(cmd, imageBlock, m_TemporaryColorTexture01, destination, 0);
        }

        #endregion


        #region RGBSplit

        private void SetupRGBSplit(CommandBuffer cmd, ref RenderingData renderingData, Material rgbSplit, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("RGBSplit");
            // TimeX += Time.deltaTime;
            // if (TimeX > 100)
            // {
            //     TimeX = 0;
            // }
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // rgbSplit.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_RGBSplit.Fading.value, m_RGBSplit.Amount.value, m_RGBSplit.Speed.value, m_RGBSplit.CenterFading.value));
            // rgbSplit.SetVector(CustomPostProcessingShaderConstants._Params2, new Vector3(TimeX, m_RGBSplit.AmountR.value, m_RGBSplit.AmountB.value));
            // int pass = (int)m_RGBSplit.SplitDirection.value;
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, rgbSplit,pass);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("RGBSplit");
            
            int paramsKeyword = Shader.PropertyToID("_RGBSplitParams");
            int params2Keyword = Shader.PropertyToID("_RGBSplitParams2");
            _TimeX += Time.deltaTime;
            if (_TimeX > 100)
            {
                _TimeX = 0;
            }
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalVector(paramsKeyword, new Vector4(m_RGBSplit.Fading.value, m_RGBSplit.Amount.value, m_RGBSplit.Speed.value, m_RGBSplit.CenterFading.value));
            cmd.SetGlobalVector(params2Keyword, new Vector3(_TimeX, m_RGBSplit.AmountR.value, m_RGBSplit.AmountB.value));
            int pass = (int)m_RGBSplit.SplitDirection.value;
            
            Draw(cmd, rgbSplit, _tempRT0, destination, pass);
        }
        


        #endregion


        #region DirectionalBlur

        private void SetupDirectionalBlur(CommandBuffer cmd, ref RenderingData renderingData, Material directionalBlur, in RTHandle source, in RTHandle destination)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = (int)(opaqueDesc.width / m_DirectionalBlur.downScaling.value);
            opaqueDesc.height = (int)(opaqueDesc.height / m_DirectionalBlur.downScaling.value);
            opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("DirectionalBlur");
            // cmd.GetTemporaryRT(m_TemporaryBlurTexture03.id, opaqueDesc, FilterMode.Point);
            // cmd.Blit(m_ColorAttachment, m_TemporaryBlurTexture03.Identifier());
            // float sinVal = (Mathf.Sin(m_DirectionalBlur.angle.value) * m_DirectionalBlur.indensity.value * 0.05f) / m_DirectionalBlur.blurCount.value;
            // float cosVal = (Mathf.Cos(m_DirectionalBlur.angle.value) * m_DirectionalBlur.indensity.value * 0.05f) / m_DirectionalBlur.blurCount.value;  
            // directionalBlur.SetVector(CustomPostProcessingShaderConstants._Params, new Vector3(m_DirectionalBlur.blurCount.value, sinVal, cosVal));
            // cmd.Blit(m_TemporaryBlurTexture03.Identifier(), m_ColorAttachment, directionalBlur);
            // cmd.ReleaseTemporaryRT(m_TemporaryBlurTexture03.id);
            // cmd.EndSample("DirectionalBlur");
            
            int blurSizeKeyword = Shader.PropertyToID("_DirectionalBlurSize");
            
            float sinVal = (Mathf.Sin(m_DirectionalBlur.angle.value) * m_DirectionalBlur.blurSize.value * 0.05f) / m_DirectionalBlur.iteration.value;
            float cosVal = (Mathf.Cos(m_DirectionalBlur.angle.value) * m_DirectionalBlur.blurSize.value * 0.05f) / m_DirectionalBlur.iteration.value;
            cmd.SetGlobalVector(blurSizeKeyword, new Vector3(m_DirectionalBlur.iteration.value, sinVal, cosVal));

            if (m_DirectionalBlur.downScaling.value > 1.0f)
            {
                Draw(cmd, null, source, m_TemporaryColorTexture01);
                Draw(cmd, directionalBlur, m_TemporaryColorTexture01, destination, 0);

            }
            else
            {
                Draw(cmd, directionalBlur, source, destination,0);
            }

        }

        #endregion


        #region RadialBlur

        private void SetupRadialBlur(CommandBuffer cmd, ref RenderingData renderingData, Material radialBlur, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("RadialBlur");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // radialBlur.SetVector(CustomPostProcessingShaderConstants._Params, new Vector3(m_RadialBlur.indensity.value * 0.02f, m_RadialBlur.RadialCenterX.value, m_RadialBlur.RadialCenterY.value));
            // int pass = (int)m_RadialBlur.qualityLevel.value;
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, radialBlur, pass);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("RadialBlur");
            
            int blurParametersKeyword = Shader.PropertyToID("_RadialBlurParameters");
            
            cmd.SetGlobalVector(blurParametersKeyword, new Vector4(m_RadialBlur.iteration.value, m_RadialBlur.blurSize.value * 0.02f,m_RadialBlur.RadialCenterX.value, m_RadialBlur.RadialCenterY.value));
            Draw(cmd, radialBlur, source, destination, 0);
        }

        #endregion

        #region GrainyBlur

        private void SetupGrainyBlur(CommandBuffer cmd, ref RenderingData renderingData, Material grainyBlur, in RTHandle source, in RTHandle destination)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = (int)(opaqueDesc.width / m_GrainyBlur.downSample.value);
            opaqueDesc.height = (int)(opaqueDesc.height / m_GrainyBlur.downSample.value);
            opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("GrainyBlur");
            // cmd.GetTemporaryRT(m_TemporaryBlurTexture03.id, opaqueDesc, FilterMode.Point);
            // cmd.Blit(m_ColorAttachment, m_TemporaryBlurTexture03.Identifier());
            // grainyBlur.SetVector(CustomPostProcessingShaderConstants._Params, new Vector2(m_GrainyBlur.indensity.value / opaqueDesc.height, m_GrainyBlur.blurCount.value));
            // cmd.Blit(m_TemporaryBlurTexture03.Identifier(), m_ColorAttachment, grainyBlur);
            // cmd.ReleaseTemporaryRT(m_TemporaryBlurTexture03.id);
            // cmd.EndSample("GrainyBlur");
            
            int blurSizeKeyword = Shader.PropertyToID("_GrainyBlurSize");
            int iterationKeyword = Shader.PropertyToID("_GrainyIteration");

            cmd.SetGlobalFloat(blurSizeKeyword, m_GrainyBlur.blurRadius.value);
            cmd.SetGlobalFloat(iterationKeyword, m_GrainyBlur.iteration.value);

            if (m_GrainyBlur.downSample.value > 1.0f)
            {
                Draw(cmd, null, source, m_TemporaryColorTexture01);
                Draw(cmd, grainyBlur, m_TemporaryColorTexture01, destination, 0);
            }
            else
            {
                Draw(cmd, grainyBlur, source, destination, 0);

            }
        }

        #endregion

        #region IrisBlur

        private void SetupIrisBlur(CommandBuffer cmd, ref RenderingData renderingData, Material irisBlur, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("IrisBlur");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // irisBlur.SetVector(CustomPostProcessingShaderConstants._GoldenRot, mGoldenRot);
            // irisBlur.SetVector(CustomPostProcessingShaderConstants._Gradient, new Vector3(m_IrisBlur.centerOffsetX.value, m_IrisBlur.centerOffsetY.value, m_IrisBlur.areaSize.value * 0.1f));
            // irisBlur.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_IrisBlur.blurCount.value, m_IrisBlur.indensity.value, 1f / opaqueDesc.width, 1f / opaqueDesc.height));
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, irisBlur);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("IrisBlur");
            
            int gradientKeyword = Shader.PropertyToID("_IrisGradient");
            int parametersKeyword = Shader.PropertyToID("_IrisParameters");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalVector(gradientKeyword, new Vector3(m_IrisBlur.centerOffsetX.value, m_IrisBlur.centerOffsetY.value, m_IrisBlur.areaSize.value * 0.1f));
            cmd.SetGlobalVector(parametersKeyword, new Vector2(m_IrisBlur.iteration.value, m_IrisBlur.blurSize.value));
            Draw(cmd, irisBlur, m_TemporaryColorTexture01, destination, 0);

        }

        #endregion

        #region TiltShiftBlur

        private void SetupTiltShiftBlur(CommandBuffer cmd, ref RenderingData renderingData, Material tiltShiftBlur, in RTHandle source, in RTHandle destination)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("TiltShiftBlur");
            // // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // tiltShiftBlur.SetVector(CustomPostProcessingShaderConstants._GoldenRot, mGoldenRot);
            // tiltShiftBlur.SetVector(CustomPostProcessingShaderConstants._Gradient, new Vector3(m_TiltShiftBlur.centerOffset.value, m_TiltShiftBlur.areaSize.value, m_TiltShiftBlur.areaSmooth.value));
            // tiltShiftBlur.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_TiltShiftBlur.blurCount.value, m_TiltShiftBlur.indensity.value, 1f / opaqueDesc.width, 1f / opaqueDesc.height));
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, tiltShiftBlur);
            // // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.EndSample("TiltShiftBlur");
            
            int gradientKeyword = Shader.PropertyToID("_TiltShiftBlurGradient");
            int parametersKeyword = Shader.PropertyToID("_TiltShiftBlurParameters");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            cmd.SetGlobalVector(gradientKeyword, new Vector3(m_TiltShiftBlur.centerOffset.value, m_TiltShiftBlur.areaSize.value, m_TiltShiftBlur.areaSmooth.value));
            cmd.SetGlobalVector(parametersKeyword, new Vector2(m_TiltShiftBlur.iteration.value, m_TiltShiftBlur.blurSize.value));
            Draw(cmd, tiltShiftBlur,m_TemporaryColorTexture01, destination, 0);
        }

        #endregion

        #region BokehBlur

        private void SetupBokehBlur(CommandBuffer cmd, ref RenderingData renderingData, Material bokehBlur, in RTHandle source, in RTHandle destination)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = (int)(opaqueDesc.width / m_BokehBlur.downScaling.value);
            opaqueDesc.height = (int)(opaqueDesc.height / m_BokehBlur.downScaling.value);
            opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("BokehBlur");
            // cmd.GetTemporaryRT(m_TemporaryBlurTexture03.id, opaqueDesc, FilterMode.Point);
            // cmd.Blit(m_ColorAttachment, m_TemporaryBlurTexture03.Identifier());
            // bokehBlur.SetVector(CustomPostProcessingShaderConstants._GoldenRot, mGoldenRot);
            // bokehBlur.SetVector(CustomPostProcessingShaderConstants._Offset, new Vector4(m_BokehBlur.blurCount.value, m_BokehBlur.indensity.value, 1f / opaqueDesc.width, 1f / opaqueDesc.height));
            // cmd.Blit(m_TemporaryBlurTexture03.Identifier(), m_ColorAttachment, bokehBlur);
            // cmd.ReleaseTemporaryRT(m_TemporaryBlurTexture03.id);
            // cmd.EndSample("BokehBlur");
            
            int blurSizeKeyword = Shader.PropertyToID("_BokehBlurSize");
            int iterationKeyword = Shader.PropertyToID("_BokehIteration");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            cmd.SetGlobalFloat(blurSizeKeyword, m_BokehBlur.blurRadius.value);
            cmd.SetGlobalFloat(iterationKeyword, m_BokehBlur.iteration.value);
            
            Draw(cmd, bokehBlur,m_TemporaryColorTexture01, destination, 0);

        }

        #endregion

        #region DualKawaseBlur

        private void SetupDualKawaseBlur(CommandBuffer cmd, ref RenderingData renderingData, Material dualKawaseBlur, in RTHandle source, in RTHandle destination)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = (int)(opaqueDesc.width / m_DualKawaseBlur.downScaling.value);
            opaqueDesc.height = (int)(opaqueDesc.height / m_DualKawaseBlur.downScaling.value);

            // cmd.BeginSample("DualKawaseBlur");
            // dualKawaseBlur.SetFloat(CustomPostProcessingShaderConstants._Offset, m_DualKawaseBlur.indensity.value);
            // RenderTargetIdentifier lastDown = m_ColorAttachment;
            // for (int i = 0; i < m_DualKawaseBlur.blurCount.value; i++)
            // {
            //     int mipDown = m_Pyramid[i].down;
            //     int mipUp = m_Pyramid[i].up;
            //     cmd.GetTemporaryRT(mipDown, width, height, 0, FilterMode.Point);
            //     cmd.GetTemporaryRT(mipUp, width, height, 0, FilterMode.Point);
            //     cmd.Blit(lastDown, mipDown, dualKawaseBlur,0);
            //
            //     lastDown = mipDown;
            //     width = Mathf.Max(width / 2, 1);
            //     height = Mathf.Max(height / 2, 1);
            // }
            //
            // int lastUp = m_Pyramid[m_DualKawaseBlur.blurCount.value - 1].down;
            // for (int i = m_DualKawaseBlur.blurCount.value - 2; i >= 0; i--)
            // {
            //     int minUp = m_Pyramid[i].up;
            //     cmd.Blit(lastUp,minUp,dualKawaseBlur,1);
            //     lastUp = minUp;
            // }
            // cmd.Blit(lastUp,m_ColorAttachment,dualKawaseBlur,1);
            // for (int i = 0; i < m_DualKawaseBlur.blurCount.value; i++)
            // {
            //     if (m_Pyramid[i].down != lastUp)
            //         cmd.ReleaseTemporaryRT(m_Pyramid[i].down);
            //     if (m_Pyramid[i].up != lastUp)
            //         cmd.ReleaseTemporaryRT(m_Pyramid[i].up);
            // }
            // cmd.EndSample("DualKawaseBlur");
            
            int blurSizeKeyword = Shader.PropertyToID("_DualKawaseBlurSize");
            
            RTHandle[] tempRT = new RTHandle[m_DualKawaseBlur.iteration.value + 1];
            RenderingUtils.ReAllocateIfNeeded(ref tempRT[0], opaqueDesc, name: "tempRTName_0",
                wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);

            for (int i = 1; i <= m_DualKawaseBlur.iteration.value; i++)
            {
                opaqueDesc.width = Math.Max(opaqueDesc.width / 2, 1);
                opaqueDesc.height = Math.Max(opaqueDesc.height / 2, 1);
                
                RenderingUtils.ReAllocateIfNeeded(ref tempRT[i], opaqueDesc, name: "tempRTName_i",
                    wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
                
            }
            
            Draw(cmd, null, source, tempRT[0]);
            for (int i = 0; i < m_DualKawaseBlur.iteration.value; i++)
            {
                cmd.SetGlobalFloat(blurSizeKeyword, 1.0f + i * m_DualKawaseBlur.blurSize.value);
                Draw(cmd, dualKawaseBlur, tempRT[i], tempRT[i+1],0);
            }
            for (int i = m_DualKawaseBlur.iteration.value; i > 1; i--)
            {
                cmd.SetGlobalFloat(blurSizeKeyword, 1.0f + i * m_DualKawaseBlur.blurSize.value);
                Draw(cmd, dualKawaseBlur, tempRT[i], tempRT[i-1],1);
            }
            Draw(cmd, dualKawaseBlur, tempRT[1], destination,1);
            
            for (int i = 0; i < m_DualKawaseBlur.iteration.value; i++)
            {
                tempRT[i]?.Release();
            }

        }

        #endregion

        #region KawaseBlur

        private void SetupKawaseBlur(CommandBuffer cmd, ref RenderingData renderingData, Material kawaseBlur, in RTHandle source, in RTHandle destination)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = (int)(opaqueDesc.width / m_KawaseBlur.downSample.value);
            opaqueDesc.height = (int)(opaqueDesc.height / m_KawaseBlur.downSample.value);
            opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("KawaseBlur");
            // cmd.GetTemporaryRT(m_TemporaryBlurTexture03.id, opaqueDesc, FilterMode.Point);
            // cmd.GetTemporaryRT(m_TemporaryBlurTexture04.id, opaqueDesc, FilterMode.Point);
            // bool needSwitch = true;
            // cmd.Blit(m_ColorAttachment, m_TemporaryBlurTexture03.Identifier());
            // for (int i = 0; i < m_KawaseBlur.blurCount.value; i++) {
            //     kawaseBlur.SetFloat(CustomPostProcessingShaderConstants._Offset, i / m_KawaseBlur.downSample.value + m_KawaseBlur.indensity.value);
            //     cmd.Blit(needSwitch ? m_TemporaryBlurTexture03.Identifier() : m_TemporaryBlurTexture04.Identifier(), needSwitch ? m_TemporaryBlurTexture04.Identifier() : m_TemporaryBlurTexture03.Identifier(),kawaseBlur);
            //     needSwitch = !needSwitch;
            // }
            // kawaseBlur.SetFloat(CustomPostProcessingShaderConstants._Offset, m_KawaseBlur.blurCount.value / m_KawaseBlur.downSample.value + m_KawaseBlur.indensity.value);
            // cmd.Blit(needSwitch ? m_TemporaryBlurTexture03.Identifier() : m_TemporaryBlurTexture04.Identifier(), m_ColorAttachment);
            // cmd.ReleaseTemporaryRT(m_TemporaryBlurTexture03.id);
            // cmd.ReleaseTemporaryRT(m_TemporaryBlurTexture04.id);
            // cmd.EndSample("KawaseBlur");
            
            int blurSizeKeyword = Shader.PropertyToID("_KawaseBlurSize");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            for (int i = 0; i < m_KawaseBlur.iteration.value; i++)
            {
                cmd.SetGlobalFloat(blurSizeKeyword, 1.0f + i * m_KawaseBlur.blurSize.value);
                Draw(cmd, kawaseBlur, m_TemporaryColorTexture01, m_TemporaryColorTexture02, 0);
                CoreUtils.Swap(ref m_TemporaryColorTexture02, ref m_TemporaryColorTexture01);
            }
            Draw(cmd, null, m_TemporaryColorTexture01, destination);

        }

        #endregion

        #region BoxBlur

        private void SetupBoxBlur(CommandBuffer cmd, ref RenderingData renderingData, Material boxBlur, in RTHandle source, in RTHandle destination)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = (int)(opaqueDesc.width / m_BoxBlur.downScaling.value);
            opaqueDesc.height = (int)(opaqueDesc.height / m_BoxBlur.downScaling.value);
            opaqueDesc.depthBufferBits = 0;

            // cmd.BeginSample("BoxBlur");
            // cmd.GetTemporaryRT(m_TemporaryBlurTexture03.id, opaqueDesc, FilterMode.Point);
            // cmd.GetTemporaryRT(m_TemporaryBlurTexture04.id, opaqueDesc, FilterMode.Point);
            // cmd.Blit(m_ColorAttachment, m_TemporaryBlurTexture03.Identifier());
            // for (int i = 0; i < m_BoxBlur.blurCount.value; i++) {
            //     boxBlur.SetVector(CustomPostProcessingShaderConstants._Offset, new Vector4(m_BoxBlur.indensity.value, 0, 0, 0));
            //     cmd.Blit(m_TemporaryBlurTexture03.Identifier(), m_TemporaryBlurTexture04.Identifier(), boxBlur);
            //     boxBlur.SetVector(CustomPostProcessingShaderConstants._Offset, new Vector4(0, m_BoxBlur.indensity.value, 0, 0));
            //     cmd.Blit(m_TemporaryBlurTexture04.Identifier(), m_TemporaryBlurTexture03.Identifier(), boxBlur);
            // }
            // cmd.Blit(m_TemporaryBlurTexture03.Identifier(), m_ColorAttachment);
            // cmd.ReleaseTemporaryRT(m_TemporaryBlurTexture03.id);
            // cmd.ReleaseTemporaryRT(m_TemporaryBlurTexture04.id);
            // cmd.EndSample("BoxBlur");
            
            int blurSizeKeyword = Shader.PropertyToID("_BoxBlurSize");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);

            for (int i = 0; i < m_BoxBlur.iteration.value; i++) {
                cmd.SetGlobalVector(blurSizeKeyword, new Vector4(1.0f + m_BoxBlur.blurRadius.value, 0, 0, 0));
                Draw(cmd, boxBlur, m_TemporaryColorTexture01, m_TemporaryColorTexture02, 0);
                cmd.SetGlobalVector(blurSizeKeyword, new Vector4(0, 1.0f + m_BoxBlur.blurRadius.value, 0, 0));
                Draw(cmd, boxBlur, m_TemporaryColorTexture02, m_TemporaryColorTexture01, 0);
            }
            
            Draw(cmd, null, m_TemporaryColorTexture01, destination);
        }

        #endregion

        #region GaussianBlur

        private void SetupGaussianBlur(CommandBuffer cmd, ref RenderingData renderingData, Material gaussianBlur, in RTHandle source, in RTHandle destination)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = (int)(opaqueDesc.width / m_GaussianBlur.downScaling.value);
            opaqueDesc.height = (int)(opaqueDesc.height / m_GaussianBlur.downScaling.value);
            opaqueDesc.depthBufferBits = 0;

            // cmd.GetTemporaryRT(m_TemporaryColorTexture03.id, opaqueDesc, m_GaussianBlur.filterMode.value);

            // cmd.BeginSample("GaussianBlur");
            // cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Point);
            // cmd.GetTemporaryRT(m_TemporaryColorTexture02.id, opaqueDesc, FilterMode.Point);
            // cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            // for (int i = 0; i < m_GaussianBlur.blurCount.value; i++) {
            //     //y-direction
            //     gaussianBlur.SetVector(CustomPostProcessingShaderConstants._Offset, new Vector4(0, m_GaussianBlur.indensity.value, 0, 0));
            //     cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_TemporaryColorTexture02.Identifier(), gaussianBlur);
            //     //x-direction
            //     gaussianBlur.SetVector(CustomPostProcessingShaderConstants._Offset, new Vector4(m_GaussianBlur.indensity.value, 0, 0, 0));
            //     cmd.Blit(m_TemporaryColorTexture02.Identifier(), m_TemporaryColorTexture01.Identifier(), gaussianBlur);
            //
            // }
            // cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment);
            // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture02.id);
            // cmd.EndSample("GaussianBlur");

            int blurSizeKeyword = Shader.PropertyToID("_GaussianBlurSize");
            
            Draw(cmd, null, source, m_TemporaryColorTexture01);
            
            for (int i = 0; i < m_GaussianBlur.iteration.value; i++) {
                //y-direction
                cmd.SetGlobalVector(blurSizeKeyword, new Vector4(0, 1.0f + i * m_GaussianBlur.blurRadius.value, 0, 0));
                Draw(cmd, gaussianBlur, m_TemporaryColorTexture01, m_TemporaryColorTexture02, 0);
                //x-direction
                cmd.SetGlobalVector(blurSizeKeyword, new Vector4( 1.0f + i * m_GaussianBlur.blurRadius.value, 0, 0, 0));
                Draw(cmd, gaussianBlur, m_TemporaryColorTexture02, m_TemporaryColorTexture01, 0);
                
            }
            
            Draw(cmd, null, m_TemporaryColorTexture01, destination);
        }

        #endregion
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

        //check if camera is overlay
        if (renderingData.cameraData.renderType == CameraRenderType.Overlay)
        {
            //I don't want to do post processing in overlay camera
            return;
        }

        // var cameraColorTarget = renderer.cameraColorTarget;
        // var cameraDepth = renderer.cameraDepthTarget;
        // var dest = RenderTargetHandle.CameraTarget;

        m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Color);
        m_ScriptablePass.renderPassEvent = evt;
        // m_ScriptablePass.Setup(evt, cameraColorTarget);
        renderer.EnqueuePass(m_ScriptablePass);
    }


    protected override void Dispose(bool disposing)
    {
        // base.Dispose(disposing);
        m_ScriptablePass.Cleanup();
    }
}