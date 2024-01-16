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
        private RTHandle m_ColorAttachment;
        // private RenderTargetIdentifier m_CameraDepthAttachment;
        // private RenderTargetHandle m_Destination;

        // const string k_RenderPostProcessingTag = "Render AdditionalPostProcessing Effects";
        const string k_RenderCustomPostProcessingTag = "Render Custom PostProcessing Pass";
        
        //blur
        private GaussianBlur m_GaussianBlur;
        private BoxBlur m_BoxBlur;
        private KawaseBlur m_KawaseBlur;
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

        RTHandle m_TemporaryBlurTexture03;
        RTHandle m_TemporaryBlurTexture04;

        // RenderTargetHandle m_TemporaryColorTexture03;
        // private RTHandle _sourceRT;
        // private RTHandle _destinationRT;
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
            
            m_TemporaryColorTexture01?.Release();
            m_TemporaryColorTexture02?.Release();
            m_TemporaryBlurTexture03?.Release();
            m_TemporaryBlurTexture04?.Release();

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

            //for custom post processing
            RenderingUtils.ReAllocateIfNeeded(ref m_TemporaryColorTexture01, opaqueDesc, name: "_TemporaryRenderTexture01",
                wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
            RenderingUtils.ReAllocateIfNeeded(ref m_TemporaryColorTexture02, opaqueDesc, name: "_TemporaryRenderTexture02",
                wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);

            //for some blur with dowmsampling
            // m_TemporaryBlurTexture03.Init("m_TemporaryBlurTexture03");
            // m_TemporaryBlurTexture04.Init("m_TemporaryBlurTexture04");
            
            
            RenderingUtils.ReAllocateIfNeeded(ref _tempRT0, opaqueDesc, name: "_tempRT0");
            RenderingUtils.ReAllocateIfNeeded(ref _tempRT1, opaqueDesc, name: "_tempRT1");
            
            ConfigureTarget(m_ColorAttachment);
        }
        
        
        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture02.id);
            // _sourceRT = null;
            // _destinationRT = null;
        }

        public void Setup(RenderPassEvent @event, RTHandle cameraColorTarget)
        {
            renderPassEvent = @event;
            m_ColorAttachment = cameraColorTarget;
            // m_CameraDepthAttachment = cameraDepth;
            // m_Destination = dest;
            // m_Materials = new MaterialLibrary(data);
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
            
            var cmd = CommandBufferPool.Get(k_RenderCustomPostProcessingTag);
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        private void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;

            #region Blur
            
            Blitter.BlitCameraTexture(cmd, m_ColorAttachment, _tempRT0);

            if (m_GaussianBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.GaussianBlur)))
                {
                    SetupGaussianBlur(cmd, ref renderingData, m_Materials.gaussianBlur, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);

                }
            }

            if (m_BoxBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.BoxBlur)))
                {
                    SetupBoxBlur(cmd, ref renderingData, m_Materials.boxBlur, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);

                }
            }

            if (m_KawaseBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.KawaseBlur)))
                {
                    SetupKawaseBlur(cmd, ref renderingData, m_Materials.kawaseBlur, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);

                }

            }

            if (m_BokehBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.BokehBlur)))
                {
                    SetupBokehBlur(cmd, ref renderingData, m_Materials.bokehBlur, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);

                }

            }

            if (m_TiltShiftBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.TiltShiftBlur)))
                {
                    SetupTiltShiftBlur(cmd, ref renderingData, m_Materials.tiltShiftBlur, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);

                }
            }

            if (m_IrisBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.IrisBlur)))
                {
                    SetupIrisBlur(cmd, ref renderingData, m_Materials.irisBlur, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);

                }
            }

            if (m_GrainyBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.GrainyBlur)))
                {
                    SetupGrainyBlur(cmd, ref renderingData, m_Materials.grainyBlur, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);

                }
            }

            if (m_RadialBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.RadialBlur)))
                {
                    SetupRadialBlur(cmd, ref renderingData, m_Materials.radialBlur, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);

                }
            }

            if (m_DirectionalBlur.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.DirectionalBlur)))
                {
                    SetupDirectionalBlur(cmd, ref renderingData, m_Materials.directionalBlur, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }

            }

            #endregion

            #region Glitch

            if (m_RGBSplit.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.RGBSplit)))
                {
                    SetupRGBSplit(cmd, ref renderingData, m_Materials.rgbSplit, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_ImageBlock.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.ImageBlock)))
                {
                    SetupImageBlock(cmd, ref renderingData, m_Materials.imageBlock, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);

                }
            }

            if (m_LineBlock.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.LineBlock)))
                {
                    SetupLineBlock(cmd, ref renderingData, m_Materials.lineBlock, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_TileJitter.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.TileJitter)))
                {
                    SetupTileJitter(cmd, ref renderingData, m_Materials.tileJitter, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_ScanLineJitter.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.ScanLineJitter)))
                {
                    SetupScanLineJitter(cmd, ref renderingData, m_Materials.scanLineJitter, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_DigitalStripe.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.DigitalStripe)))
                {
                    SetupDigitalStripe(cmd, ref renderingData, m_Materials.digitalStripe, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_AnalogNoise.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.AnalogNoise)))
                {
                    SetupAnalogNoise(cmd, ref renderingData, m_Materials.analogNoise, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_ScreenJump.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.ScreenJump)))
                {
                    SetupScreenJump(cmd, ref renderingData, m_Materials.screenJump, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_ScreenShake.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.ScreenShake)))
                {
                    SetupScreenShake(cmd, ref renderingData, m_Materials.screenShake, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_WaveJitter.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.WaveJitter)))
                {
                    SetupWaveJitter(cmd, ref renderingData, m_Materials.waveJitter, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            #endregion

            #region EdgeDetection

            if (m_Roberts.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Roberts)))
                {
                    SetupRoberts(cmd, ref renderingData, m_Materials.roberts, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_RobertsNeon.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.RobertsNeon)))
                {
                    SetupRobertsNeon(cmd, ref renderingData, m_Materials.robertsNeon, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_Scharr.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Scharr)))
                {
                    SetupScharr(cmd, ref renderingData, m_Materials.scharr, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_ScharrNeon.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.ScharrNeon)))
                {
                    SetupScharrNeon(cmd, ref renderingData, m_Materials.scharrNeon, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_Sobel.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Sobel)))
                {
                    SetupSobel(cmd, ref renderingData, m_Materials.sobel, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_SobelNeon.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.SobelNeon)))
                {
                    SetupSobelNeon(cmd, ref renderingData, m_Materials.sobelNeon, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            #endregion

            #region Pixelise

            if (m_Circle.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Circle)))
                {
                    SetupCircle(cmd, ref renderingData, m_Materials.circle, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_Diamond.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Diamond)))
                {
                    SetupDiamond(cmd, ref renderingData, m_Materials.diamond, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_Hexagon.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Hexagon)))
                {
                    SetupHexagon(cmd, ref renderingData, m_Materials.hexagon, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_HexagonGrid.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.HexagonGrid)))
                {
                    SetupHexagonGrid(cmd, ref renderingData, m_Materials.hexagonGrid, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_Leaf.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Leaf)))
                {
                    SetupLeaf(cmd, ref renderingData, m_Materials.leaf, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_Led.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Led)))
                {
                    SetupLed(cmd, ref renderingData, m_Materials.led, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_Quad.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Quad)))
                {
                    SetupQuad(cmd, ref renderingData, m_Materials.quad, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_Sector.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Sector)))
                {
                    SetupSector(cmd, ref renderingData, m_Materials.sector, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_Triangle.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Triangle)))
                {
                    SetupTriangle(cmd, ref renderingData, m_Materials.triangle, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            #endregion

            #region Vignette

            if (m_Aurora.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Aurora)))
                {
                    SetupAurora(cmd, ref renderingData, m_Materials.aurora, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_RapidOldTV.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.RapidOldTV)))
                {
                    SetupRapidOldTV(cmd, ref renderingData, m_Materials.rapidOldTV, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_RapidOldTVV2.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.RapidOldTVV2)))
                {
                    SetupRapidOldTVV2(cmd, ref renderingData, m_Materials.rapidOldTVV2, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_Rapid.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Rapid)))
                {
                    SetupRapid(cmd, ref renderingData, m_Materials.rapid, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_RapidV2.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.RapidV2)))
                {
                    SetupRapidV2(cmd, ref renderingData, m_Materials.rapidV2, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            #endregion

            #region Sharpen

            if (m_V1.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.SharpenV1)))
                {
                    SetupSharpenV1(cmd, ref renderingData, m_Materials.sharpenV1, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_V2.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.SharpenV2)))
                {
                    SetupSharpenV2(cmd, ref renderingData, m_Materials.sharpenV2, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_V3.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.SharpenV3)))
                {
                    SetupSharpenV3(cmd, ref renderingData, m_Materials.sharpenV3, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            #endregion

            #region BleachBypass

            if (m_BleachBypass.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.BleachBypass)))
                {
                    SetupBleachBypass(cmd, ref renderingData, m_Materials.bleachBypass, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_Brightness.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Brightness)))
                {
                    SetupBrightness(cmd, ref renderingData, m_Materials.brightness, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_Hue.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Hue)))
                {
                    SetupHue(cmd, ref renderingData, m_Materials.hue, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);

                }
            }

            if (m_Tint.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Tint)))
                {
                    SetupTint(cmd, ref renderingData, m_Materials.tint, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_WhiteBalance.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.WhiteBalance)))
                {
                    SetupWhiteBalance(cmd, ref renderingData, m_Materials.whiteBalance, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_LensFilter.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.LensFilter)))
                {
                    SetupLensFilter(cmd, ref renderingData, m_Materials.lensFilter, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_Saturation.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Saturation)))
                {
                    SetupSaturation(cmd, ref renderingData, m_Materials.saturation, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_Technicolor.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Technicolor)))
                {
                    SetupTechnicolor(cmd, ref renderingData, m_Materials.technicolor, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_ColorReplace.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.ColorReplace)))
                {
                    SetupColorReplace(cmd, ref renderingData, m_Materials.colorReplace, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }
            
            if (m_ColorReplaceV2.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.ColorReplace)))
                {
                    SetupColorReplaceV2(cmd, ref renderingData, m_Materials.colorReplaceV2, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_Contrast.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.Contrast)))
                {
                    SetupContrast(cmd, ref renderingData, m_Materials.contrast, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_ContrastV2.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.ContrastV2)))
                {
                    SetupContrastV2(cmd, ref renderingData, m_Materials.contrastV2, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            if (m_ContrastV3.IsActive() && !cameraData.isSceneViewCamera)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(CustomPostProcessingProfileId.ContrastV3)))
                {
                    SetupContrastV3(cmd, ref renderingData, m_Materials.contrastV3, _tempRT0,  _tempRT1);
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }

            #endregion
            
            Blitter.BlitCameraTexture(cmd, _tempRT0, cameraData.renderer.cameraColorTargetHandle);

        }

        #region Contrast

        private void SetupContrastV3(CommandBuffer cmd, ref RenderingData renderingData, Material contrastV3, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int contrastV3Keyword = Shader.PropertyToID("ContrastV3");
            
            cmd.SetGlobalVector(contrastV3Keyword, m_ContrastV3.contrast.value);
            Blitter.BlitCameraTexture(cmd, source, dest, contrastV3, 0);
        }

        private void SetupContrastV2(CommandBuffer cmd, ref RenderingData renderingData, Material contrastV2, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int contrastV2Keyword = Shader.PropertyToID("_ContrastV2");
            int contrastV2ColorKeyword = Shader.PropertyToID("_ContrastV2FactorRGB");
            
            cmd.SetGlobalFloat(contrastV2Keyword, m_ContrastV2.contrast.value + 1);
            cmd.SetGlobalColor(contrastV2ColorKeyword, new Color(m_ContrastV2.contrastFactorR.value, m_ContrastV2.contrastFactorG.value,m_ContrastV2.contrastFactorB.value));
            Blitter.BlitCameraTexture(cmd, source, dest, contrastV2,0);
        }

        private void SetupContrast(CommandBuffer cmd, ref RenderingData renderingData, Material contrast, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int contrastKeyword = Shader.PropertyToID("_Contrast");
            
            cmd.SetGlobalFloat(contrastKeyword, m_Contrast.contrast.value + 1);
            Blitter.BlitCameraTexture(cmd, source, dest, contrast,0);

        }

        #endregion


        #region ColorReplace

        private void SetupColorReplace(CommandBuffer cmd, ref RenderingData renderingData, Material colorReplace, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int rangeKeyword = Shader.PropertyToID("_ColorReplaceRange");
            int fuzzinessKeyword = Shader.PropertyToID("_ColorReplaceFuzziness");
            int fromColorKeyword = Shader.PropertyToID("_ColorReplaceFromColor");
            int toColorKeyword = Shader.PropertyToID("_ColorReplaceToColor");
            
            cmd.SetGlobalFloat(rangeKeyword, m_ColorReplace.Range.value);
            cmd.SetGlobalFloat(fuzzinessKeyword, m_ColorReplace.Fuzziness.value);
            cmd.SetGlobalColor(fromColorKeyword, m_ColorReplace.FromColor.value);
            cmd.SetGlobalColor(toColorKeyword, m_ColorReplace.ToColor.value);
            
            Blitter.BlitCameraTexture(cmd, source, dest, colorReplace, 0);
        }

        #endregion

        #region ColorReplaceV2

        private void SetupColorReplaceV2(CommandBuffer cmd, ref RenderingData renderingData, Material colorReplaceV2, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int rangeKeyword = Shader.PropertyToID("_ColorReplaceV2Range");
            int fuzzinessKeyword = Shader.PropertyToID("_ColorReplaceV2Fuzziness");
            int fromColorKeyword = Shader.PropertyToID("_ColorReplaceV2FromColor");
            int toColorKeyword = Shader.PropertyToID("_ColorReplaceV2ToColor");
            
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
            
            Blitter.BlitCameraTexture(cmd, source, dest, colorReplaceV2, 0);
        }

        #endregion

        #region Technicolor

        private void SetupTechnicolor(CommandBuffer cmd, ref RenderingData renderingData, Material technicolor, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int exposureKeyword = Shader.PropertyToID("_TechnicolorExposure");
            int indensityKeyword = Shader.PropertyToID("_TechnicolorIndensity");
            int TechnicolorKeyword = Shader.PropertyToID("_Technicolor");
            
            cmd.SetGlobalFloat(exposureKeyword, 8.01f - m_Technicolor.exposure.value);
            cmd.SetGlobalFloat(indensityKeyword, m_Technicolor.indensity.value);
            cmd.SetGlobalColor(TechnicolorKeyword, Color.white - new Color(m_Technicolor.colorBalanceR.value, m_Technicolor.colorBalanceG.value, m_Technicolor.colorBalanceB.value));
            Blitter.BlitCameraTexture(cmd, source, dest, technicolor, 0);
        }

        #endregion


        #region Saturation

        private void SetupSaturation(CommandBuffer cmd, ref RenderingData renderingData, Material saturation, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int saturationKeyword = Shader.PropertyToID("_Saturation");
            
            cmd.SetGlobalFloat(saturationKeyword, m_Saturation.saturation.value);
            Blitter.BlitCameraTexture(cmd, source, dest, saturation,0);
        }

        #endregion


        #region LensFilter

        private void SetupLensFilter(CommandBuffer cmd, ref RenderingData renderingData, Material lensFilter, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int indensityKeyword = Shader.PropertyToID("_LensFilterIndensity");
            int lensColorKeyword = Shader.PropertyToID("_LensColor");
            
            cmd.SetGlobalFloat(indensityKeyword, m_LensFilter.Indensity.value);
            cmd.SetGlobalColor(lensColorKeyword, m_LensFilter.LensColor.value);
            Blitter.BlitCameraTexture(cmd, source, dest, lensFilter, 0);
        }

        #endregion


        #region WhiteBalance

        private void SetupWhiteBalance(CommandBuffer cmd, ref RenderingData renderingData, Material whiteBalance, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int temperatureKeyword = Shader.PropertyToID("_WhiteBalanceTemperature");
            int tintKeyword = Shader.PropertyToID("_WhiteBalanceTint");
            
            cmd.SetGlobalFloat(temperatureKeyword, m_WhiteBalance.temperature.value);
            cmd.SetGlobalFloat(tintKeyword, m_WhiteBalance.tint.value);
            Blitter.BlitCameraTexture(cmd, source, dest, whiteBalance, 0);
        }

        #endregion


        #region Tint

        private void SetupTint(CommandBuffer cmd, ref RenderingData renderingData, Material tint, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int indensityKeyword = Shader.PropertyToID("_TintIndensity");
            int colorTintKeyword = Shader.PropertyToID("_ColorTint");
            
            cmd.SetGlobalFloat(indensityKeyword, m_Tint.indensity.value);
            cmd.SetGlobalColor(colorTintKeyword, m_Tint.colorTint.value);
            Blitter.BlitCameraTexture(cmd, source, dest, tint, 0);
        }

        #endregion


        #region Hue

        private void SetupHue(CommandBuffer cmd, ref RenderingData renderingData, Material hue, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int hueDegreeKeyword = Shader.PropertyToID("_HueDegree");
            
            cmd.SetGlobalFloat(hueDegreeKeyword, m_Hue.HueDegree.value);
            Blitter.BlitCameraTexture(cmd, source, dest, hue, 0);
        }

        #endregion


        #region Brightness

        private void SetupBrightness(CommandBuffer cmd, ref RenderingData renderingData, Material brightness, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int _brightnessKeyword = Shader.PropertyToID("_Brightness");
            cmd.SetGlobalFloat(_brightnessKeyword, m_Brightness.brightness.value);
            Blitter.BlitCameraTexture(cmd, source, dest, brightness, 0);
        }

        #endregion


        #region BleachBypass

        private void SetupBleachBypass(CommandBuffer cmd, ref RenderingData renderingData, Material bleachBypass, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int indensityKeyword = Shader.PropertyToID("_BleachBypassIndensity");
            
            cmd.SetGlobalFloat(indensityKeyword, m_BleachBypass.Indensity.value);
            Blitter.BlitCameraTexture(cmd, source, dest, bleachBypass, 0);
        }

        #endregion


        #region Sharpen

        private void SetupSharpenV3(CommandBuffer cmd, ref RenderingData renderingData, Material v3, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int parametersKeyword = Shader.PropertyToID("_SharpenV3");
            
            Vector2 p = new Vector2(1.0f + (3.2f * m_V3.Sharpness.value), 0.8f * m_V3.Sharpness.value);
            cmd.SetGlobalVector(parametersKeyword, p);
            Blitter.BlitCameraTexture(cmd, source, dest, v3, 0);
        }

        private void SetupSharpenV2(CommandBuffer cmd, ref RenderingData renderingData, Material v2, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int parametersKeyword = Shader.PropertyToID("_SharpenV2");
            
            cmd.SetGlobalFloat(parametersKeyword, m_V2.Sharpness.value);
            Blitter.BlitCameraTexture(cmd, source, dest, v2, 0);
        }

        private void SetupSharpenV1(CommandBuffer cmd, ref RenderingData renderingData, Material v1, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int parametersKeyword = Shader.PropertyToID("_SharpenV1");
            
            cmd.SetGlobalVector(parametersKeyword, new Vector2(m_V1.Strength.value, m_V1.Threshold.value));
            Blitter.BlitCameraTexture(cmd, source, dest, v1, 0);
        }

        #endregion


        #region RapidV2

        private void SetupRapidV2(CommandBuffer cmd, ref RenderingData renderingData, Material rapidV2, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int parametersKeyword = Shader.PropertyToID("_RapidV2Parameters");
            int colorKeyword = Shader.PropertyToID("_RapidV2Color");
            
            cmd.SetGlobalVector(parametersKeyword, new Vector4(m_RapidV2.vignetteIndensity.value, m_RapidV2.vignetteSharpness.value, m_RapidV2.vignetteCenter.value.x, m_RapidV2.vignetteCenter.value.y));
            if (m_RapidV2.vignetteType.value == VignetteType.ColorMode)
            {
                cmd.SetGlobalColor(colorKeyword, m_RapidV2.vignetteColor.value);
            }
            
            Blitter.BlitCameraTexture(cmd, source, dest, rapidV2, (int)m_RapidV2.vignetteType.value);
        }

        #endregion


        #region Rapid

        private void SetupRapid(CommandBuffer cmd, ref RenderingData renderingData, Material rapid, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int parametersKeyword = Shader.PropertyToID("_RapidParameters");
            int colorKeyword = Shader.PropertyToID("_RapidColor");
            
            cmd.SetGlobalVector(parametersKeyword, new Vector3(m_Rapid.vignetteIndensity.value, m_Rapid.vignetteCenter.value.x, m_Rapid.vignetteCenter.value.y));
            if (m_Rapid.vignetteType.value == VignetteType.ColorMode)
            {
                cmd.SetGlobalColor(colorKeyword, m_Rapid.vignetteColor.value);
            }
            
            Blitter.BlitCameraTexture(cmd, source, dest, rapid, (int)m_Rapid.vignetteType.value);
        }

        #endregion


        #region RapidOldTVV2

        private void SetupRapidOldTVV2(CommandBuffer cmd, ref RenderingData renderingData, Material rapidOldTvv2, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int parametersKeyword = Shader.PropertyToID("_RapidOldTVV2Parameters");
            int colorKeyword = Shader.PropertyToID("_RapidOldTVV2Color");
            
            cmd.SetGlobalVector(parametersKeyword, new Vector2(m_RapidOldTVV2.vignetteSize.value, m_RapidOldTVV2.sizeOffset.value));
            if (m_RapidOldTVV2.vignetteType.value == VignetteType.ColorMode)
            {
                cmd.SetGlobalColor(colorKeyword, m_RapidOldTVV2.vignetteColor.value);
            }
            
            Blitter.BlitCameraTexture(cmd, source, dest, rapidOldTvv2, (int)m_RapidOldTVV2.vignetteType.value);
        }

        #endregion


        #region RapidOldTV

        private void SetupRapidOldTV(CommandBuffer cmd, ref RenderingData renderingData, Material rapidOldTV, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int parametersKeyword = Shader.PropertyToID("_RapidOldTVParameters");
            int colorKeyword = Shader.PropertyToID("_RapidOldTVColor");
            
            cmd.SetGlobalVector(parametersKeyword, new Vector3(m_RapidOldTV.vignetteIndensity.value, m_RapidOldTV.vignetteCenter.value.x, m_RapidOldTV.vignetteCenter.value.y));
            if (m_RapidOldTV.vignetteType.value == VignetteType.ColorMode)
            {
                cmd.SetGlobalColor(colorKeyword, m_RapidOldTV.vignetteColor.value);
            }
            
            Blitter.BlitCameraTexture(cmd, source, dest, rapidOldTV, (int)m_RapidOldTV.vignetteType.value);
        }

        #endregion


        #region Aurora

        private void SetupAurora(CommandBuffer cmd, ref RenderingData renderingData, Material aurora, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int parametersKeyword = Shader.PropertyToID("_AuroraParameters");
            int parameters2Keyword = Shader.PropertyToID("_AuroraParameters2");
            
            _TimeX += Time.deltaTime;
            if (_TimeX > 100)
            {
                _TimeX = 0;
            }
            cmd.SetGlobalVector(parametersKeyword, new Vector4(m_Aurora.vignetteArea.value, m_Aurora.vignetteSmothness.value, m_Aurora.colorChange.value, _TimeX * m_Aurora.flowSpeed.value));
            cmd.SetGlobalVector(parameters2Keyword, new Vector4(m_Aurora.colorFactorR.value, m_Aurora.colorFactorG.value,m_Aurora.colorFactorB.value, m_Aurora.vignetteFading.value));
            
            Blitter.BlitCameraTexture(cmd, source, dest, aurora, 0);
        }

        #endregion


        #region Triangle

        private void SetupTriangle(CommandBuffer cmd, ref RenderingData renderingData, Material triangle, in RTHandle source, in RTHandle dest)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_TriangleParams");
            
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
            
            Blitter.BlitCameraTexture(cmd, source, dest, triangle, 0);
        }

        #endregion


        #region Sector

        private void SetupSector(CommandBuffer cmd, ref RenderingData renderingData, Material sector, in RTHandle source, in RTHandle dest)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_SectorParams");
            int params2Keyword = Shader.PropertyToID("_SectorParams2");
            int backgroundKeyword = Shader.PropertyToID("_SectorBackground");
            
            float size = (1.01f - m_Sector.pixelSize.value) * 300f;
            Vector4 parameters = new Vector4(size, ((opaqueDesc.width * 2 / (float)opaqueDesc.height) * size / Mathf.Sqrt(3f)), m_Sector.circleRadius.value, 0f);
            cmd.SetGlobalVector(paramsKeyword, parameters);
            cmd.SetGlobalVector(params2Keyword, new Vector2(m_Sector.pixelIntervalX.value, m_Sector.pixelIntervalY.value));
            cmd.SetGlobalColor(backgroundKeyword, m_Sector.backgroundColor.value);
            
            Blitter.BlitCameraTexture(cmd, source, dest, sector, 0);
        }

        #endregion


        #region Quad

        private void SetupQuad(CommandBuffer cmd, ref RenderingData renderingData, Material quad, in RTHandle source, in RTHandle dest)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_QuadParams");
            
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
            
            Blitter.BlitCameraTexture(cmd, source, dest, quad, 0);
        }


        #endregion


        #region Led

        private void SetupLed(CommandBuffer cmd, ref RenderingData renderingData, Material led, in RTHandle source, in RTHandle dest)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_LedParams");
            int backgroundKeyword = Shader.PropertyToID("_LedBackground");
            
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
            
            Blitter.BlitCameraTexture(cmd, source, dest, led, 0);
        }

        #endregion


        #region Leaf

        private void SetupLeaf(CommandBuffer cmd, ref RenderingData renderingData, Material leaf, in RTHandle source, in RTHandle dest)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_LeafParams");
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
            
            Blitter.BlitCameraTexture(cmd, source, dest, leaf, 0);
        }

        #endregion


        #region HexagonGrid

        private void SetupHexagonGrid(CommandBuffer cmd, ref RenderingData renderingData, Material hexagonGrid, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_HexagonGridParams");
            cmd.SetGlobalVector(paramsKeyword, new Vector2(m_HexagonGrid.pixelSize.value, m_HexagonGrid.gridWidth.value));
            
            Blitter.BlitCameraTexture(cmd, source, dest, hexagonGrid, 0);
        }

        #endregion


        #region Hexagon

        private void SetupHexagon(CommandBuffer cmd, ref RenderingData renderingData, Material hexagon, in RTHandle source, in RTHandle dest)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_HexagonParams");
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
            
            Blitter.BlitCameraTexture(cmd, source, dest, hexagon, 0);
        }

        #endregion


        #region Diamond

        private void SetupDiamond(CommandBuffer cmd, ref RenderingData renderingData, Material diamond, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int pixelSizeKeyword = Shader.PropertyToID("_DiamondPixelSize");
            cmd.SetGlobalFloat(pixelSizeKeyword, m_Diamond.pixelSize.value);
            
            Blitter.BlitCameraTexture(cmd, source, dest, diamond, 0);
        }

        #endregion


        #region Circle

        private void SetupCircle(CommandBuffer cmd, ref RenderingData renderingData, Material circle, in RTHandle source, in RTHandle dest)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_CircleParams");
            int params2Keyword = Shader.PropertyToID("_CircleParams2");
            int backgroundKeyword = Shader.PropertyToID("_CircleBackground");
            
            
            float size = (1.01f - m_Circle.pixelSize.value) * 300f;
            Vector4 parameters = new Vector4(size, ((opaqueDesc.width * 2 / (float)opaqueDesc.height) * size / Mathf.Sqrt(3f)), m_Circle.circleRadius.value, 0f);
            cmd.SetGlobalVector(paramsKeyword, parameters);
            cmd.SetGlobalVector(params2Keyword, new Vector2(m_Circle.pixelIntervalX.value, m_Circle.pixelIntervalY.value));
            cmd.SetGlobalColor(backgroundKeyword, m_Circle.backgroundColor.value);
            
            Blitter.BlitCameraTexture(cmd, source, dest, circle, 0);
        }

        #endregion


        #region SobelNeon

        private void SetupSobelNeon(CommandBuffer cmd, ref RenderingData renderingData, Material sobelNeon, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_SobelNeonParams");
            int backgroundKeyword = Shader.PropertyToID("_SobelNeonBackgroundColor");
            
            cmd.SetGlobalVector(paramsKeyword, new Vector4(m_SobelNeon.edgeWidth.value, m_SobelNeon.edgeNeonFade.value, m_SobelNeon.brightness.value, m_SobelNeon.backgroundFade.value));
            cmd.SetGlobalColor(backgroundKeyword, m_SobelNeon.backgroundColor.value);
            Blitter.BlitCameraTexture(cmd, source, dest, sobelNeon, 0);
        }

        #endregion


        #region Sobel

        private void SetupSobel(CommandBuffer cmd, ref RenderingData renderingData, Material sobel, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_SobelParams");
            int edgeColorKeyword = Shader.PropertyToID("_SobelEdgeColor");
            int backgroundKeyword = Shader.PropertyToID("_SobelBackgroundColor");
            
            cmd.SetGlobalVector(paramsKeyword, new Vector2(m_Sobel.edgeWidth.value, m_Sobel.backgroundFade.value));
            cmd.SetGlobalColor(edgeColorKeyword, m_Sobel.edgeColor.value);
            cmd.SetGlobalColor(backgroundKeyword, m_Sobel.backgroundColor.value);
            Blitter.BlitCameraTexture(cmd, source, dest, sobel, 0);
        }

        #endregion


        #region ScharrNeon

        private void SetupScharrNeon(CommandBuffer cmd, ref RenderingData renderingData, Material scharrNeon, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_ScharrNeonParams");
            int backgroundKeyword = Shader.PropertyToID("_ScharrNeonBackgroundColor");
            
            cmd.SetGlobalVector(paramsKeyword, new Vector4(m_ScharrNeon.edgeWidth.value, m_ScharrNeon.edgeNeonFade.value, m_ScharrNeon.brightness.value, m_ScharrNeon.backgroundFade.value));
            cmd.SetGlobalColor(backgroundKeyword, m_ScharrNeon.backgroundColor.value);
            Blitter.BlitCameraTexture(cmd, source, dest, scharrNeon, 0);
        }

        #endregion


        #region Scharr

        private void SetupScharr(CommandBuffer cmd, ref RenderingData renderingData, Material scharr, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_ScharrParams");
            int edgeColorKeyword = Shader.PropertyToID("_ScharrEdgeColor");
            int backgroundKeyword = Shader.PropertyToID("_ScharrBackgroundColor");
            
            cmd.SetGlobalVector(paramsKeyword, new Vector2(m_Scharr.edgeWidth.value, m_Scharr.backgroundFade.value));
            cmd.SetGlobalColor(edgeColorKeyword, m_Scharr.edgeColor.value);
            cmd.SetGlobalColor(backgroundKeyword, m_Scharr.backgroundColor.value);
            Blitter.BlitCameraTexture(cmd, source, dest, scharr, 0);
        }

        #endregion


        #region RobertsNeon

        private void SetupRobertsNeon(CommandBuffer cmd, ref RenderingData renderingData, Material robertsNeon, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_RobertsNeonParams");
            int backgroundKeyword = Shader.PropertyToID("_RobertsNeonBackgroundColor");
            
            cmd.SetGlobalVector(paramsKeyword, new Vector4(m_RobertsNeon.edgeWidth.value, m_RobertsNeon.edgeNeonFade.value, m_RobertsNeon.brightness.value, m_RobertsNeon.backgroundFade.value));
            cmd.SetGlobalColor(backgroundKeyword, m_RobertsNeon.backgroundColor.value);
            Blitter.BlitCameraTexture(cmd, source, dest, robertsNeon, 0);
        }

        #endregion


        #region Roberts

        private void SetupRoberts(CommandBuffer cmd, ref RenderingData renderingData, Material roberts, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_RobertsParams");
            int edgeColorKeyword = Shader.PropertyToID("_RobertsEdgeColor");
            int backgroundKeyword = Shader.PropertyToID("_RobertsBackgroundColor");
            
            cmd.SetGlobalVector(paramsKeyword, new Vector2(m_Roberts.edgeWidth.value, m_Roberts.backgroundFade.value));
            cmd.SetGlobalColor(edgeColorKeyword, m_Roberts.edgeColor.value);
            cmd.SetGlobalColor(backgroundKeyword, m_Roberts.backgroundColor.value);
            Blitter.BlitCameraTexture(cmd, source, dest, roberts, 0);
        }

        #endregion


        #region WaveJitter

        private void SetupWaveJitter(CommandBuffer cmd, ref RenderingData renderingData, Material waveJitter, in RTHandle source, in RTHandle dest)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_WaveJitterParams");
            int resolutionKeyword = Shader.PropertyToID("_WaveJitterResolution");
            
            
            UpdateFrequencyWJ(waveJitter);
            cmd.SetGlobalVector(paramsKeyword, new Vector4(m_WaveJitter.IntervalType.value == IntervalType.Random ? _randomFrequency : m_WaveJitter.Frequency.value, m_WaveJitter.RGBSplit.value , m_WaveJitter.Speed.value, m_WaveJitter.Amount.value));
            cmd.SetGlobalVector(resolutionKeyword, m_WaveJitter.CustomResolution.value ? m_WaveJitter.Resolution.value : new Vector2(opaqueDesc.width,opaqueDesc.height));
            
            Blitter.BlitCameraTexture(cmd, source, dest, waveJitter, (int)m_WaveJitter.JitterDirection.value);
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

        private void SetupScreenShake(CommandBuffer cmd, ref RenderingData renderingData, Material screenShake, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_ScreenShakeParams");
            
            cmd.SetGlobalFloat(paramsKeyword, m_ScreenShake.ScreenShakeIndensity.value);
            
            Blitter.BlitCameraTexture(cmd, source, dest, screenShake, (int)m_ScreenShake.ScreenShakeDirection.value);
        }

        #endregion


        #region ScreenJump

        private void SetupScreenJump(CommandBuffer cmd, ref RenderingData renderingData, Material screenJump, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_ScanLineJitterParams");
            
            _screenJumpTime += Time.deltaTime * m_ScreenJump.ScreenJumpIndensity.value * 9.8f;
            cmd.SetGlobalVector(paramsKeyword, new Vector2(m_ScreenJump.ScreenJumpIndensity.value, m_ScreenJump.isHorizontalReverse.value ? -_screenJumpTime : _screenJumpTime));
            
            Blitter.BlitCameraTexture(cmd, source, dest, screenJump, (int)m_ScreenJump.ScreenJumpDirection.value);
        }

        #endregion


        #region AnalogNoise

        private void SetupAnalogNoise(CommandBuffer cmd, ref RenderingData renderingData, Material analogNoise, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_AnalogNoiseParams");
            _TimeX += Time.deltaTime;
            if (_TimeX > 100)
            {
                _TimeX = 0;
            }
            
            cmd.SetGlobalVector(paramsKeyword, new Vector4(m_AnalogNoise.NoiseSpeed.value, m_AnalogNoise.NoiseFading.value, m_AnalogNoise.LuminanceJitterThreshold.value, _TimeX));
            
            Blitter.BlitCameraTexture(cmd, source, dest, analogNoise, 0);
        }

        #endregion


        #region DigitalStripe

        private void SetupDigitalStripe(CommandBuffer cmd, ref RenderingData renderingData, Material digitalStripe, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int colorKeyword = Shader.PropertyToID("_StripColorAdjustColor");
            int indensityKeyword = Shader.PropertyToID("_DigitalStripeIndensity");
            int colorIndensityKeyword = Shader.PropertyToID("_StripColorAdjustIndensity");
            int texKeyword = Shader.PropertyToID("_DigitalStripeNoiseTex");
            
            UpdateFrequencyDS(m_DigitalStripe.frequency.value, m_DigitalStripe.noiseTextureWidth.value, m_DigitalStripe.noiseTextureHeight.value, m_DigitalStripe.stripeLength.value);
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
            
            Blitter.BlitCameraTexture(cmd, source, dest, digitalStripe, 0);
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

        private void SetupScanLineJitter(CommandBuffer cmd, ref RenderingData renderingData, Material scanLineJitter, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_ScanLineJitterParams");
            
            UpdateFrequencySLJ(scanLineJitter);
            float displacement = 0.005f + Mathf.Pow(m_ScanLineJitter.JitterIndensity.value, 3) * 0.1f;
            float threshold = Mathf.Clamp01(1.0f - m_ScanLineJitter.JitterIndensity.value * 1.2f);
            cmd.SetGlobalVector(paramsKeyword, new Vector3(displacement, threshold, m_ScanLineJitter.IntervalType.value == IntervalType.Random ? _randomFrequency : m_ScanLineJitter.Frequency.value));
            
            Blitter.BlitCameraTexture(cmd, source, dest, scanLineJitter, (int)m_ScanLineJitter.JitterDirection.value);
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

        private void SetupTileJitter(CommandBuffer cmd, ref RenderingData renderingData, Material tileJitter, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
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
            cmd.SetGlobalVector(paramsKeyword, new Vector4(m_TileJitter.SplittingNumber.value, m_TileJitter.Amount.value , m_TileJitter.Speed.value * 100f, m_TileJitter.IntervalType.value == IntervalType.Random ? _randomFrequency : m_TileJitter.Frequency.value));
            
            Blitter.BlitCameraTexture(cmd, source, dest, tileJitter, m_TileJitter.SplittingDirection.value == Direction.Horizontal ? 0 : 1);
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

        private void SetupLineBlock(CommandBuffer cmd, ref RenderingData renderingData, Material lineBlock, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_LineBlockParams");
            int params2Keyword = Shader.PropertyToID("_LineBlockParams2");
            _TimeX += Time.deltaTime;
            if (_TimeX > 100)
            {
                _TimeX = 0;
            }
            
            UpdateFrequency(lineBlock);
            
            cmd.SetGlobalVector(paramsKeyword, new Vector3(
                m_LineBlock.IntervalType.value == IntervalType.Random ? _randomFrequency : m_LineBlock.Frequency.value,
                _TimeX * m_LineBlock.Speed.value * 0.2f , m_LineBlock.Amount.value));
            cmd.SetGlobalVector(params2Keyword, new Vector3(m_LineBlock.Offset.value, 1 / m_LineBlock.LinesWidth.value, m_LineBlock.Alpha.value));
            int pass = (int)m_LineBlock.BlockDirection.value;
            
            Blitter.BlitCameraTexture(cmd, source, dest, lineBlock, pass);
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

        private void SetupImageBlock(CommandBuffer cmd, ref RenderingData renderingData, Material imageBlock, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_ImageBlockParams");
            int params2Keyword = Shader.PropertyToID("_ImageBlockParams2");
            int params3Keyword = Shader.PropertyToID("_ImageBlockParams3");
            _TimeX += Time.deltaTime;
            if (_TimeX > 100)
            {
                _TimeX = 0;
            }

            cmd.SetGlobalVector(paramsKeyword, new Vector3(_TimeX * m_ImageBlock.Speed.value, m_ImageBlock.Amount.value, m_ImageBlock.Fade.value));
            cmd.SetGlobalVector(params2Keyword, new Vector4(m_ImageBlock.BlockLayer1_U.value, m_ImageBlock.BlockLayer1_V.value, m_ImageBlock.BlockLayer2_U.value, m_ImageBlock.BlockLayer2_V.value));
            cmd.SetGlobalVector(params3Keyword, new Vector3(m_ImageBlock.RGBSplitIndensity.value, m_ImageBlock.BlockLayer1_Indensity.value, m_ImageBlock.BlockLayer2_Indensity.value));
            
            Blitter.BlitCameraTexture(cmd, source, dest, imageBlock, 0);
        }

        #endregion


        #region RGBSplit

        private void SetupRGBSplit(CommandBuffer cmd, ref RenderingData renderingData, Material rgbSplit, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int paramsKeyword = Shader.PropertyToID("_RGBSplitParams");
            int params2Keyword = Shader.PropertyToID("_RGBSplitParams2");
            _TimeX += Time.deltaTime;
            if (_TimeX > 100)
            {
                _TimeX = 0;
            }
            cmd.SetGlobalVector(paramsKeyword, new Vector4(m_RGBSplit.Fading.value, m_RGBSplit.Amount.value, m_RGBSplit.Speed.value, m_RGBSplit.CenterFading.value));
            cmd.SetGlobalVector(params2Keyword, new Vector3(_TimeX, m_RGBSplit.AmountR.value, m_RGBSplit.AmountB.value));
            int pass = (int)m_RGBSplit.SplitDirection.value;
            
            Blitter.BlitCameraTexture(cmd, source, dest, rgbSplit, pass);
        }
        


        #endregion


        #region DirectionalBlur

        private void SetupDirectionalBlur(CommandBuffer cmd, ref RenderingData renderingData, Material directionalBlur, in RTHandle source, in RTHandle dest)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = (int)(opaqueDesc.width / m_DirectionalBlur.downScaling.value);
            opaqueDesc.height = (int)(opaqueDesc.height / m_DirectionalBlur.downScaling.value);
            opaqueDesc.depthBufferBits = 0;
            
            int blurSizeKeyword = Shader.PropertyToID("_DirectionalBlurSize");
            RenderingUtils.ReAllocateIfNeeded(ref m_TemporaryBlurTexture03, opaqueDesc,
                name: "_TemporaryDirectionalBlurRT", wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
            
            float sinVal = (Mathf.Sin(m_DirectionalBlur.angle.value) * m_DirectionalBlur.blurSize.value * 0.05f) / m_DirectionalBlur.iteration.value;
            float cosVal = (Mathf.Cos(m_DirectionalBlur.angle.value) * m_DirectionalBlur.blurSize.value * 0.05f) / m_DirectionalBlur.iteration.value;
            cmd.SetGlobalVector(blurSizeKeyword, new Vector3(m_DirectionalBlur.iteration.value, sinVal, cosVal));

            if (m_DirectionalBlur.downScaling.value > 1.0f)
            {
                Blit(cmd, source, m_TemporaryBlurTexture03);
                Blitter.BlitCameraTexture(cmd, m_TemporaryBlurTexture03, dest, directionalBlur, 0);

            }
            else
            {
                Blitter.BlitCameraTexture(cmd, source, dest, directionalBlur, 0);
            }

        }

        #endregion


        #region RadialBlur

        private void SetupRadialBlur(CommandBuffer cmd, ref RenderingData renderingData, Material radialBlur, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int blurParametersKeyword = Shader.PropertyToID("_RadialBlurParameters");
            
            cmd.SetGlobalVector(blurParametersKeyword, new Vector4(m_RadialBlur.iteration.value, m_RadialBlur.blurSize.value * 0.02f,m_RadialBlur.RadialCenterX.value, m_RadialBlur.RadialCenterY.value));
            Blitter.BlitCameraTexture(cmd, source, dest, radialBlur, 0);
            
        }

        #endregion

        #region GrainyBlur

        private void SetupGrainyBlur(CommandBuffer cmd, ref RenderingData renderingData, Material grainyBlur, in RTHandle source, in RTHandle dest)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = (int)(opaqueDesc.width / m_GrainyBlur.downSample.value);
            opaqueDesc.height = (int)(opaqueDesc.height / m_GrainyBlur.downSample.value);
            opaqueDesc.depthBufferBits = 0;
            
            int blurSizeKeyword = Shader.PropertyToID("_GrainyBlurSize");
            int iterationKeyword = Shader.PropertyToID("_GrainyIteration");
            RenderingUtils.ReAllocateIfNeeded(ref m_TemporaryBlurTexture03, opaqueDesc, name: "_TemporaryBokeBlurRT",
                wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
            
            cmd.SetGlobalFloat(blurSizeKeyword, m_GrainyBlur.blurRadius.value);
            cmd.SetGlobalFloat(iterationKeyword, m_GrainyBlur.iteration.value);

            if (m_GrainyBlur.downSample.value > 1.0f)
            {
                Blit(cmd, source, m_TemporaryBlurTexture03);
                Blitter.BlitCameraTexture(cmd, m_TemporaryBlurTexture03, dest, grainyBlur, 0);
            }
            else
            {
                Blitter.BlitCameraTexture(cmd, source, dest, grainyBlur, 0);

            }
        }

        #endregion

        #region IrisBlur

        private void SetupIrisBlur(CommandBuffer cmd, ref RenderingData renderingData, Material irisBlur, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int gradientKeyword = Shader.PropertyToID("_IrisGradient");
            int parametersKeyword = Shader.PropertyToID("_IrisParameters");
            
            cmd.SetGlobalVector(gradientKeyword, new Vector3(m_IrisBlur.centerOffsetX.value, m_IrisBlur.centerOffsetY.value, m_IrisBlur.areaSize.value * 0.1f));
            cmd.SetGlobalVector(parametersKeyword, new Vector2(m_IrisBlur.iteration.value, m_IrisBlur.blurSize.value));
            Blitter.BlitCameraTexture(cmd, source, dest, irisBlur, 0);

        }

        #endregion

        #region TiltShiftBlur

        private void SetupTiltShiftBlur(CommandBuffer cmd, ref RenderingData renderingData, Material tiltShiftBlur, in RTHandle source, in RTHandle dest)
        {
            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;
            
            int gradientKeyword = Shader.PropertyToID("_TiltShiftBlurGradient");
            int parametersKeyword = Shader.PropertyToID("_TiltShiftBlurParameters");
            
            cmd.SetGlobalVector(gradientKeyword, new Vector3(m_TiltShiftBlur.centerOffset.value, m_TiltShiftBlur.areaSize.value, m_TiltShiftBlur.areaSmooth.value));
            cmd.SetGlobalVector(parametersKeyword, new Vector2(m_TiltShiftBlur.iteration.value, m_TiltShiftBlur.blurSize.value));
            Blitter.BlitCameraTexture(cmd, source, dest, tiltShiftBlur, 0);
        }

        #endregion

        #region BokehBlur

        private void SetupBokehBlur(CommandBuffer cmd, ref RenderingData renderingData, Material bokehBlur, in RTHandle source, in RTHandle dest)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = (int)(opaqueDesc.width / m_BokehBlur.downScaling.value);
            opaqueDesc.height = (int)(opaqueDesc.height / m_BokehBlur.downScaling.value);
            opaqueDesc.depthBufferBits = 0;
            
            int blurSizeKeyword = Shader.PropertyToID("_BokehBlurSize");
            int iterationKeyword = Shader.PropertyToID("_BokehIteration");
            RenderingUtils.ReAllocateIfNeeded(ref m_TemporaryBlurTexture03, opaqueDesc, name: "_TemporaryBokeBlurRT",
                wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
            
            Blit(cmd, source, m_TemporaryBlurTexture03);
            
            cmd.SetGlobalFloat(blurSizeKeyword, m_BokehBlur.blurRadius.value);
            cmd.SetGlobalFloat(iterationKeyword, m_BokehBlur.iteration.value);
            
            Blitter.BlitCameraTexture(cmd, m_TemporaryBlurTexture03, dest, bokehBlur, 0);
            
        }

        #endregion

        #region KawaseBlur

        private void SetupKawaseBlur(CommandBuffer cmd, ref RenderingData renderingData, Material kawaseBlur, in RTHandle source, in RTHandle dest)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = (int)(opaqueDesc.width / m_KawaseBlur.downSample.value);
            opaqueDesc.height = (int)(opaqueDesc.height / m_KawaseBlur.downSample.value);
            opaqueDesc.depthBufferBits = 0;
            
            int blurSizeKeyword = Shader.PropertyToID("_KawaseBlurSize");
            RenderingUtils.ReAllocateIfNeeded(ref m_TemporaryBlurTexture03, opaqueDesc, name: "_tempGaussianBlurRT03",
                wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
            RenderingUtils.ReAllocateIfNeeded(ref m_TemporaryBlurTexture04, opaqueDesc, name: "_tempGaussianBlurRT04",
                wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
            
            Blit(cmd, source, m_TemporaryBlurTexture03);
            for (int i = 0; i < m_KawaseBlur.iteration.value; i++)
            {
                cmd.SetGlobalFloat(blurSizeKeyword, 1.0f + i * m_KawaseBlur.blurSize.value);
                Blitter.BlitCameraTexture(cmd, m_TemporaryBlurTexture03, m_TemporaryBlurTexture04, kawaseBlur, 0);
                CoreUtils.Swap(ref m_TemporaryBlurTexture04, ref m_TemporaryBlurTexture03);
            }
            Blit(cmd, m_TemporaryBlurTexture03, dest);
        }

        #endregion

        #region BoxBlur

        private void SetupBoxBlur(CommandBuffer cmd, ref RenderingData renderingData, Material boxBlur, in RTHandle source, in RTHandle dest)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = (int)(opaqueDesc.width / m_BoxBlur.downScaling.value);
            opaqueDesc.height = (int)(opaqueDesc.height / m_BoxBlur.downScaling.value);
            opaqueDesc.depthBufferBits = 0;
            
            int blurSizeKeyword = Shader.PropertyToID("_BoxBlurSize");
            RenderingUtils.ReAllocateIfNeeded(ref m_TemporaryBlurTexture03, opaqueDesc, name: "_tempGaussianBlurRT03",
                wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
            RenderingUtils.ReAllocateIfNeeded(ref m_TemporaryBlurTexture04, opaqueDesc, name: "_tempGaussianBlurRT04",
                wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
            Blit(cmd, source, m_TemporaryBlurTexture03);

            for (int i = 0; i < m_BoxBlur.iteration.value; i++) {
                cmd.SetGlobalVector(blurSizeKeyword, new Vector4(1.0f + m_BoxBlur.blurRadius.value, 0, 0, 0));
                Blitter.BlitCameraTexture(cmd, m_TemporaryBlurTexture03, m_TemporaryBlurTexture04, boxBlur, 0);
                cmd.SetGlobalVector(blurSizeKeyword, new Vector4(0, 1.0f + m_BoxBlur.blurRadius.value, 0, 0));
                Blitter.BlitCameraTexture(cmd, m_TemporaryBlurTexture04, m_TemporaryBlurTexture03, boxBlur, 0);

            }
            
            Blit(cmd, m_TemporaryBlurTexture03, dest);
        }

        #endregion

        #region GaussianBlur

        private void SetupGaussianBlur(CommandBuffer cmd, ref RenderingData renderingData, Material gaussianBlur, in RTHandle source, in RTHandle dest)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width = (int)(opaqueDesc.width / m_GaussianBlur.downScaling.value);
            opaqueDesc.height = (int)(opaqueDesc.height / m_GaussianBlur.downScaling.value);
            opaqueDesc.depthBufferBits = 0;

            int blurSizeKeyword = Shader.PropertyToID("_GaussianBlurSize");
            RenderingUtils.ReAllocateIfNeeded(ref m_TemporaryBlurTexture03, opaqueDesc, name: "_tempGaussianBlurRT03",
                wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
            RenderingUtils.ReAllocateIfNeeded(ref m_TemporaryBlurTexture04, opaqueDesc, name: "_tempGaussianBlurRT04",
                wrapMode: TextureWrapMode.Clamp, filterMode: FilterMode.Bilinear);
            
            Blit(cmd, source, m_TemporaryBlurTexture03);
            
            for (int i = 0; i < m_GaussianBlur.iteration.value; i++) {
                //y-direction
                cmd.SetGlobalVector(blurSizeKeyword, new Vector4(0, 1.0f + i * m_GaussianBlur.blurRadius.value, 0, 0));
                Blitter.BlitCameraTexture(cmd, m_TemporaryBlurTexture03, m_TemporaryBlurTexture04, gaussianBlur, 0);
                //x-direction
                cmd.SetGlobalVector(blurSizeKeyword, new Vector4( 1.0f + i * m_GaussianBlur.blurRadius.value, 0, 0, 0));
                Blitter.BlitCameraTexture(cmd, m_TemporaryBlurTexture04, m_TemporaryBlurTexture03, gaussianBlur, 0);
                
            }
            
            Blit(cmd, m_TemporaryBlurTexture03, dest);
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

        // //check if camera is overlay
        // if (renderingData.cameraData.renderType == CameraRenderType.Overlay)
        // {
        //     //I don't want to do post processing in overlay camera
        //     return;
        // }

        // var cameraColorTarget = renderer.cameraColorTarget;
        // var cameraDepth = renderer.cameraDepthTarget;
        // var dest = RenderTargetHandle.CameraTarget;
        
        // m_ScriptablePass.Setup(evt, cameraColorTarget);
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Color);
            m_ScriptablePass.Setup(evt, renderer.cameraColorTargetHandle);
        }
    }

    protected override void Dispose(bool disposing)
    {
        // base.Dispose(disposing);
        m_ScriptablePass.Cleanup();
    }
}