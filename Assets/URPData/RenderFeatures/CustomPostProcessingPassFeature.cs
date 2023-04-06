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
        private Contrast m_Contrast;
        private ContrastV2 m_ContrastV2;
        private ContrastV3 m_ContrastV3;


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
            m_Contrast = stack.GetComponent<Contrast>();
            m_ContrastV2 = stack.GetComponent<ContrastV2>();
            m_ContrastV3 = stack.GetComponent<ContrastV3>();

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

            if (m_ScharrNeon.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupScharrNeon(cmd, ref renderingData, m_Materials.scharrNeon);
            }

            if (m_Sobel.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupSobel(cmd, ref renderingData, m_Materials.sobel);
            }

            if (m_SobelNeon.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupSobelNeon(cmd, ref renderingData, m_Materials.sobelNeon);
            }

            #endregion

            #region Pixelise

            if (m_Circle.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupCircle(cmd, ref renderingData, m_Materials.circle);
            }
            
            if (m_Diamond.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupDiamond(cmd, ref renderingData, m_Materials.diamond);
            }
            
            if (m_Hexagon.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupHexagon(cmd, ref renderingData, m_Materials.hexagon);
            }
            
            if (m_HexagonGrid.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupHexagonGrid(cmd, ref renderingData, m_Materials.hexagonGrid);
            }
            
            if (m_Leaf.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupLeaf(cmd, ref renderingData, m_Materials.leaf);
            }
            
            if (m_Led.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupLed(cmd, ref renderingData, m_Materials.led);
            }
            
            if (m_Quad.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupQuad(cmd, ref renderingData, m_Materials.quad);
            }
            
            if (m_Sector.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupSector(cmd, ref renderingData, m_Materials.sector);
            }
            
            if (m_Triangle.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupTriangle(cmd, ref renderingData, m_Materials.triangle);
            }

            #endregion

            #region Vignette

            if (m_Aurora.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupAurora(cmd, ref renderingData, m_Materials.aurora);
            }

            if (m_RapidOldTV.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupRapidOldTV(cmd, ref renderingData, m_Materials.rapidOldTV);
            }

            if (m_RapidOldTVV2.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupRapidOldTVV2(cmd, ref renderingData, m_Materials.rapidOldTVV2);
            }

            if (m_Rapid.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupRapid(cmd, ref renderingData, m_Materials.rapid);
            }

            if (m_RapidV2.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupRapidV2(cmd, ref renderingData, m_Materials.rapidV2);
            }

            #endregion

            #region Sharpen

            if (m_V1.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupSharpenV1(cmd, ref renderingData, m_Materials.sharpenV1);
            }
            
            if (m_V2.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupSharpenV2(cmd, ref renderingData, m_Materials.sharpenV2);
            }
            
            if (m_V3.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupSharpenV3(cmd, ref renderingData, m_Materials.sharpenV3);
            }

            #endregion

            #region BleachBypass

            if (m_BleachBypass.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupBleachBypass(cmd, ref renderingData, m_Materials.bleachBypass);
            }

            if (m_Brightness.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupBrightness(cmd, ref renderingData, m_Materials.brightness);
            }

            if (m_Hue.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupHue(cmd, ref renderingData, m_Materials.hue);
            }

            if (m_Tint.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupTint(cmd, ref renderingData, m_Materials.tint);
            }

            if (m_WhiteBalance.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupWhiteBalance(cmd, ref renderingData, m_Materials.whiteBalance);
            }

            if (m_LensFilter.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupLensFilter(cmd, ref renderingData, m_Materials.lensFilter);
            }

            if (m_Saturation.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupSaturation(cmd, ref renderingData, m_Materials.saturation);
            }

            if (m_Technicolor.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupTechnicolor(cmd, ref renderingData, m_Materials.technicolor);
            }

            if (m_ColorReplace.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupColorReplace(cmd, ref renderingData, m_Materials.colorReplace);
            }

            if (m_Contrast.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupContrast(cmd, ref renderingData, m_Materials.contrast);
            }
            
            if (m_ContrastV2.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupContrastV2(cmd, ref renderingData, m_Materials.contrastV2);
            }
            
            if (m_ContrastV3.IsActive() && !cameraData.isSceneViewCamera)
            {
                SetupContrastV3(cmd, ref renderingData, m_Materials.contrastV3);
            }

            #endregion
            
        }

        #region Contrast

        private void SetupContrastV3(CommandBuffer cmd, ref RenderingData renderingData, Material contrastV3)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("ContrastV3");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            contrastV3.SetVector(CustomPostProcessingShaderConstants._Contrast, m_ContrastV3.contrast.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, contrastV3);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("ContrastV3");
        }

        private void SetupContrastV2(CommandBuffer cmd, ref RenderingData renderingData, Material contrastV2)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("ContrastV2");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            contrastV2.SetFloat(CustomPostProcessingShaderConstants._Contrast, m_ContrastV2.contrast.value + 1);
            contrastV2.SetColor(CustomPostProcessingShaderConstants._ContrastFactorRGB, new Color(m_ContrastV2.contrastFactorR.value, m_ContrastV2.contrastFactorG.value,m_ContrastV2.contrastFactorB.value));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, contrastV2);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("ContrastV2");
        }

        private void SetupContrast(CommandBuffer cmd, ref RenderingData renderingData, Material contrast)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Contrast");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            contrast.SetFloat(CustomPostProcessingShaderConstants._Contrast, m_Contrast.contrast.value + 1);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, contrast);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Contrast");
        }

        #endregion
        

        #region ColorReplace

        private void SetupColorReplace(CommandBuffer cmd, ref RenderingData renderingData, Material colorReplace)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("ColorReplace");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            colorReplace.SetFloat(CustomPostProcessingShaderConstants._Range, m_ColorReplace.Range.value);
            colorReplace.SetFloat(CustomPostProcessingShaderConstants._Fuzziness, m_ColorReplace.Fuzziness.value);
            if (m_ColorReplace.colorType == ColorType.Original)
            {
                colorReplace.SetColor(CustomPostProcessingShaderConstants._FromColor, m_ColorReplace.FromColor.value);
                colorReplace.SetColor(CustomPostProcessingShaderConstants._ToColor, m_ColorReplace.ToColor.value);
            }
            else
            {
                TimeX += (Time.deltaTime * m_ColorReplace.gridentSpeed.value);
                if (TimeX > 100)
                {
                    TimeX = 0;
                }

                if (m_ColorReplace.FromGradientColor.value != null)
                {
                    colorReplace.SetColor(CustomPostProcessingShaderConstants._FromColor, m_ColorReplace.FromGradientColor.value.Evaluate(TimeX * 0.01f));
                }

                if (m_ColorReplace.ToGradientColor.value != null)
                {
                    colorReplace.SetColor(CustomPostProcessingShaderConstants._ToColor, m_ColorReplace.ToGradientColor.value.Evaluate(TimeX * 0.01f));
                }
            }
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, colorReplace);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("ColorReplace");
        }

        #endregion
        

        #region Technicolor

        private void SetupTechnicolor(CommandBuffer cmd, ref RenderingData renderingData, Material technicolor)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Technicolor");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            technicolor.SetFloat(CustomPostProcessingShaderConstants._Exposure, 8.01f - m_Technicolor.exposure.value);
            technicolor.SetFloat(CustomPostProcessingShaderConstants._Indensity, m_Technicolor.indensity.value);
            technicolor.SetColor(CustomPostProcessingShaderConstants._ColorBalance, Color.white - new Color(m_Technicolor.colorBalanceR.value, m_Technicolor.colorBalanceG.value, m_Technicolor.colorBalanceB.value));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, technicolor);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Technicolor");
        }

        #endregion
        

        #region Saturation

        private void SetupSaturation(CommandBuffer cmd, ref RenderingData renderingData, Material saturation)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Saturation");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            saturation.SetFloat(CustomPostProcessingShaderConstants._Saturation, m_Saturation.saturation.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, saturation);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Saturation");
        }

        #endregion
        

        #region LensFilter

        private void SetupLensFilter(CommandBuffer cmd, ref RenderingData renderingData, Material lensFilter)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("LensFilter");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            lensFilter.SetFloat(CustomPostProcessingShaderConstants._Indensity, m_LensFilter.Indensity.value);
            lensFilter.SetColor(CustomPostProcessingShaderConstants._LensColor, m_LensFilter.LensColor.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, lensFilter);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("LensFilter");
        }

        #endregion
        

        #region WhiteBalance

        private void SetupWhiteBalance(CommandBuffer cmd, ref RenderingData renderingData, Material whiteBalance)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("WhiteBalance");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            whiteBalance.SetFloat(CustomPostProcessingShaderConstants._Temperature, m_WhiteBalance.temperature.value);
            whiteBalance.SetFloat(CustomPostProcessingShaderConstants._Tint, m_WhiteBalance.tint.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, whiteBalance);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("WhiteBalance");
        }

        #endregion
        

        #region Tint

        private void SetupTint(CommandBuffer cmd, ref RenderingData renderingData, Material tint)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Tint");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            tint.SetFloat(CustomPostProcessingShaderConstants._Indensity, m_Tint.indensity.value);
            tint.SetColor(CustomPostProcessingShaderConstants._ColorTint, m_Tint.colorTint.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, tint);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Tint");
        }

        #endregion
        

        #region Hue

        private void SetupHue(CommandBuffer cmd, ref RenderingData renderingData, Material hue)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Hue");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            hue.SetFloat(CustomPostProcessingShaderConstants._HueDegree, m_Hue.HueDegree.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, hue);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Hue");
        }

        #endregion
        

        #region Brightness

        private void SetupBrightness(CommandBuffer cmd, ref RenderingData renderingData, Material brightness)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Brightness");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            brightness.SetFloat(CustomPostProcessingShaderConstants._Brightness, m_Brightness.brightness.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, brightness);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Brightness");
        }

        #endregion
        

        #region BleachBypass

        private void SetupBleachBypass(CommandBuffer cmd, ref RenderingData renderingData, Material bleachBypass)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("BleachBypass");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            bleachBypass.SetFloat(CustomPostProcessingShaderConstants._Indensity, m_BleachBypass.Indensity.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, bleachBypass);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("BleachBypass");
        }

        #endregion
        

        #region Sharpen
        
        private void SetupSharpenV3(CommandBuffer cmd, ref RenderingData renderingData, Material v3)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("SharpenV3");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            Vector2 p = new Vector2(1.0f + (3.2f * m_V3.Sharpness.value), 0.8f * m_V3.Sharpness.value);
            v3.SetVector(CustomPostProcessingShaderConstants._Params, p);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, v3);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("SharpenV3");
        }

        private void SetupSharpenV2(CommandBuffer cmd, ref RenderingData renderingData, Material v2)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("SharpenV2");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            v2.SetFloat(CustomPostProcessingShaderConstants._Sharpness, m_V2.Sharpness.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, v2);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("SharpenV2");
        }

        private void SetupSharpenV1(CommandBuffer cmd, ref RenderingData renderingData, Material v1)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("SharpenV1");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            v1.SetVector(CustomPostProcessingShaderConstants._Params, new Vector2(m_V1.Strength.value, m_V1.Threshold.value));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, v1);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("SharpenV1");
        }

        #endregion
        

        #region RapidV2

        private void SetupRapidV2(CommandBuffer cmd, ref RenderingData renderingData, Material rapidV2)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Rapid");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            rapidV2.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_RapidV2.vignetteIndensity.value, m_RapidV2.vignetteSharpness.value, m_RapidV2.vignetteCenter.value.x, m_RapidV2.vignetteCenter.value.y));
            if (m_RapidV2.vignetteType.value == VignetteType.ColorMode)
            {
                rapidV2.SetColor(CustomPostProcessingShaderConstants._VignetteColor, m_RapidV2.vignetteColor.value);
            }
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, rapidV2, (int)m_RapidV2.vignetteType.value);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Rapid");
        }

        #endregion
        

        #region Rapid

        private void SetupRapid(CommandBuffer cmd, ref RenderingData renderingData, Material rapid)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Rapid");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            rapid.SetVector(CustomPostProcessingShaderConstants._Params, new Vector3(m_Rapid.vignetteIndensity.value, m_Rapid.vignetteCenter.value.x, m_Rapid.vignetteCenter.value.y));
            if (m_Rapid.vignetteType.value == VignetteType.ColorMode)
            {
                rapid.SetColor(CustomPostProcessingShaderConstants._VignetteColor, m_Rapid.vignetteColor.value);
            }
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, rapid, (int)m_Rapid.vignetteType.value);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Rapid");
        }

        #endregion
        

        #region RapidOldTVV2

        private void SetupRapidOldTVV2(CommandBuffer cmd, ref RenderingData renderingData, Material rapidOldTvv2)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("RapidOldTVV2");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            rapidOldTvv2.SetVector(CustomPostProcessingShaderConstants._Params, new Vector2(m_RapidOldTVV2.vignetteSize.value, m_RapidOldTVV2.sizeOffset.value));
            if (m_RapidOldTVV2.vignetteType.value == VignetteType.ColorMode)
            {
                rapidOldTvv2.SetColor(CustomPostProcessingShaderConstants._VignetteColor, m_RapidOldTVV2.vignetteColor.value);
            }
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, rapidOldTvv2, (int)m_RapidOldTVV2.vignetteType.value);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("RapidOldTVV2");
        }

        #endregion
        

        #region RapidOldTV

        private void SetupRapidOldTV(CommandBuffer cmd, ref RenderingData renderingData, Material rapidOldTV)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("RapidOldTV");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            rapidOldTV.SetVector(CustomPostProcessingShaderConstants._Params, new Vector3(m_RapidOldTV.vignetteIndensity.value, m_RapidOldTV.vignetteCenter.value.x, m_RapidOldTV.vignetteCenter.value.y));
            if (m_RapidOldTV.vignetteType.value == VignetteType.ColorMode)
            {
                rapidOldTV.SetColor(CustomPostProcessingShaderConstants._VignetteColor, m_RapidOldTV.vignetteColor.value);
            }
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, rapidOldTV, (int)m_RapidOldTV.vignetteType.value);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("RapidOldTV");
        }

        #endregion
        

        #region Aurora

        private void SetupAurora(CommandBuffer cmd, ref RenderingData renderingData, Material aurora)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Aurora");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            TimeX += Time.deltaTime;
            if (TimeX > 100)
            {
                TimeX = 0;
            }
            aurora.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_Aurora.vignetteArea.value, m_Aurora.vignetteSmothness.value, m_Aurora.colorChange.value, TimeX));
            aurora.SetVector(CustomPostProcessingShaderConstants._Params2, new Vector4(m_Aurora.colorFactorR.value, m_Aurora.colorFactorG.value,m_Aurora.colorFactorB.value, m_Aurora.vignetteFading.value));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, aurora);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Aurora");
        }

        #endregion
        

        #region Triangle

        private void SetupTriangle(CommandBuffer cmd, ref RenderingData renderingData, Material triangle)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Triangle");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            float size = (1.01f - m_Triangle.pixelSize.value) * 5f;

            float ratio = m_Triangle.pixelRatio.value;
            if (m_Triangle.useAutoScreenRatio.value)
            {
                ratio = (float)(opaqueDesc.width / (float)opaqueDesc.height);
                if (ratio == 0)
                {
                    ratio = 1f;
                }
            }
            triangle.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(size, ratio, m_Triangle.pixelScaleX.value * 20, m_Triangle.pixelScaleY.value * 20));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, triangle);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Triangle");
        }

        #endregion
        

        #region Sector

        private void SetupSector(CommandBuffer cmd, ref RenderingData renderingData, Material sector)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Sector");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            float size = (1.01f - m_Sector.pixelSize.value) * 300f;
            Vector4 parameters = new Vector4(size, ((opaqueDesc.width * 2 / (float)opaqueDesc.height) * size / Mathf.Sqrt(3f)), m_Sector.circleRadius.value, 0f);
            sector.SetVector(CustomPostProcessingShaderConstants._Params, parameters);
            sector.SetVector(CustomPostProcessingShaderConstants._Params2, new Vector2(m_Sector.pixelIntervalX.value, m_Sector.pixelIntervalY.value));
            sector.SetColor(CustomPostProcessingShaderConstants._BackgroundColor, m_Sector.backgroundColor.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, sector);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Sector");
        }

        #endregion
        

        #region Quad

        private void SetupQuad(CommandBuffer cmd, ref RenderingData renderingData, Material quad)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Quad");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            float size = (1.01f - m_Quad.pixelSize.value) * 200f;
            quad.SetFloat(CustomPostProcessingShaderConstants._PixelSize, size);
            float ratio = m_Quad.pixelRatio.value;
            if (m_Quad.useAutoScreenRatio.value)
            {
                ratio = (float)(opaqueDesc.width / (float)opaqueDesc.height) ;
                if (ratio==0)
                {
                    ratio = 1f;
                }
            }
            quad.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(size, ratio, m_Quad.pixelScaleX.value, m_Quad.pixelScaleY.value));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, quad);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Quad");
        }


        #endregion
        
        #region Led

        private void SetupLed(CommandBuffer cmd, ref RenderingData renderingData, Material led)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Led");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            float size = (1.01f - m_Led.pixelSize.value) * 300f;

            float ratio = m_Led.pixelRatio.value;
            if (m_Led.useAutoScreenRatio.value)
            {
                ratio = (float)(opaqueDesc.width / (float)opaqueDesc.height);
                if (ratio == 0)
                {
                    ratio = 1f;
                }
            }
            led.SetVector(CustomPostProcessingShaderConstants._Params, new Vector3(size, ratio, m_Led.ledRadius.value));
            led.SetColor(CustomPostProcessingShaderConstants._BackgroundColor, m_Led.backgroundColor.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, led);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Led");
        }

        #endregion
        

        #region Leaf

        private void SetupLeaf(CommandBuffer cmd, ref RenderingData renderingData, Material leaf)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Leaf");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            float size = (1.01f - m_Leaf.pixelSize.value) * 10f;

            float ratio = m_Leaf.pixelRatio.value;
            if (m_Leaf.useAutoScreenRatio.value)
            {
                ratio = (float)(opaqueDesc.width / (float)opaqueDesc.height);
                if (ratio == 0)
                {
                    ratio = 1f;
                }
            }
            leaf.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(size,ratio, m_Leaf.pixelScaleX.value * 20,m_Leaf.pixelScaleY.value * 20));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, leaf);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Leaf");
        }

        #endregion
        

        #region HexagonGrid

        private void SetupHexagonGrid(CommandBuffer cmd, ref RenderingData renderingData, Material hexagonGrid)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("HexagonGrid");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            hexagonGrid.SetVector(CustomPostProcessingShaderConstants._Params, new Vector2(m_HexagonGrid.pixelSize.value, m_HexagonGrid.gridWidth.value));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, hexagonGrid);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("HexagonGrid");
        }

        #endregion
        

        #region Hexagon

        private void SetupHexagon(CommandBuffer cmd, ref RenderingData renderingData, Material hexagon)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Hexagon");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            hexagon.SetFloat(CustomPostProcessingShaderConstants._PixelSize, m_Hexagon.pixelSize.value);
            float size = m_Hexagon.pixelSize.value * 0.2f;
            float ratio = m_Hexagon.pixelRatio.value;
            if (m_Hexagon.useAutoScreenRatio.value)
            {
                ratio = (float)(opaqueDesc.width / (float)opaqueDesc.height);
                if (ratio == 0)
                {
                    ratio = 1f;
                }
            }
            hexagon.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(size,ratio, m_Hexagon.pixelScaleX.value,m_Hexagon.pixelScaleY.value));
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, hexagon);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Hexagon");
        }

        #endregion
        

        #region Diamond

        private void SetupDiamond(CommandBuffer cmd, ref RenderingData renderingData, Material diamond)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Diamond");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            diamond.SetFloat(CustomPostProcessingShaderConstants._PixelSize, m_Diamond.pixelSize.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, diamond);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Diamond");
        }

        #endregion
        

        #region Circle

        private void SetupCircle(CommandBuffer cmd, ref RenderingData renderingData, Material circle)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Circle");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            float size = (1.01f - m_Circle.pixelSize.value) * 300f;
            Vector4 parameters = new Vector4(size, ((opaqueDesc.width * 2 / (float)opaqueDesc.height) * size / Mathf.Sqrt(3f)), m_Circle.circleRadius.value, 0f);
            circle.SetVector(CustomPostProcessingShaderConstants._Params, parameters);
            circle.SetVector(CustomPostProcessingShaderConstants._Params2, new Vector2(m_Circle.pixelIntervalX.value, m_Circle.pixelIntervalY.value));
            circle.SetColor(CustomPostProcessingShaderConstants._BackgroundColor, m_Circle.backgroundColor.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, circle);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Circle");
        }

        #endregion
        

        #region SobelNeon

        private void SetupSobelNeon(CommandBuffer cmd, ref RenderingData renderingData, Material sobelNeon)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("SobelNeon");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            sobelNeon.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_SobelNeon.edgeWidth.value, m_SobelNeon.edgeNeonFade.value, m_SobelNeon.brightness.value, m_SobelNeon.backgroundFade.value));
            sobelNeon.SetColor(CustomPostProcessingShaderConstants._BackgroundColor, m_SobelNeon.backgroundColor.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, sobelNeon);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("SobelNeon");
        }

        #endregion
        

        #region Sobel

        private void SetupSobel(CommandBuffer cmd, ref RenderingData renderingData, Material sobel)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Sobel");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            sobel.SetVector(CustomPostProcessingShaderConstants._Params, new Vector2(m_Sobel.edgeWidth.value, m_Sobel.backgroundFade.value));
            sobel.SetColor(CustomPostProcessingShaderConstants._EdgeColor, m_Sobel.edgeColor.value);
            sobel.SetColor(CustomPostProcessingShaderConstants._BackgroundColor, m_Sobel.backgroundColor.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, sobel);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("Sobel");
        }

        #endregion
        

        #region ScharrNeon

        private void SetupScharrNeon(CommandBuffer cmd, ref RenderingData renderingData, Material scharrNeon)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("ScharrNeon");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            scharrNeon.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_ScharrNeon.edgeWidth.value, m_ScharrNeon.edgeNeonFade.value, m_ScharrNeon.brightness.value, m_ScharrNeon.backgroundFade.value));
            scharrNeon.SetColor(CustomPostProcessingShaderConstants._BackgroundColor, m_ScharrNeon.backgroundColor.value);
            cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_ColorAttachment, scharrNeon);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture01.id);
            cmd.EndSample("ScharrNeon");
        }

        #endregion
        

        private void SetupScharr(CommandBuffer cmd, ref RenderingData renderingData, Material scharr)
        {
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            
            cmd.BeginSample("Scharr");
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            scharr.SetVector(CustomPostProcessingShaderConstants._Params, new Vector2(m_Scharr.edgeWidth.value, m_Scharr.backgroundFade.value));
            scharr.SetColor(CustomPostProcessingShaderConstants._EdgeColor, m_Scharr.edgeColor.value);
            scharr.SetColor(CustomPostProcessingShaderConstants._BackgroundColor, m_Scharr.backgroundColor.value);
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            robertsNeon.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_RobertsNeon.edgeWidth.value, m_RobertsNeon.edgeNeonFade.value, m_RobertsNeon.brightness.value, m_RobertsNeon.backgroundFade.value));
            robertsNeon.SetColor(CustomPostProcessingShaderConstants._BackgroundColor, m_RobertsNeon.backgroundColor.value);
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            roberts.SetVector(CustomPostProcessingShaderConstants._Params, new Vector2(m_Roberts.edgeWidth.value, m_Roberts.backgroundFade.value));
            roberts.SetColor(CustomPostProcessingShaderConstants._EdgeColor, m_Roberts.edgeColor.value);
            roberts.SetColor(CustomPostProcessingShaderConstants._BackgroundColor, m_Roberts.backgroundColor.value);
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            waveJitter.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_WaveJitter.IntervalType.value == IntervalType.Random ? randomFrequency : m_WaveJitter.Frequency.value, m_WaveJitter.RGBSplit.value , m_WaveJitter.Speed.value, m_WaveJitter.Amount.value));
            waveJitter.SetVector(CustomPostProcessingShaderConstants._Resolution, m_WaveJitter.CustomResolution.value ? m_WaveJitter.Resolution.value : new Vector2(opaqueDesc.width,opaqueDesc.height));
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            screenShake.SetFloat(CustomPostProcessingShaderConstants._ScreenShake, m_ScreenShake.ScreenShakeIndensity.value);
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            screenJump.SetVector(CustomPostProcessingShaderConstants._Params, new Vector2(m_ScreenJump.ScreenJumpIndensity.value, m_ScreenJump.isHorizontalReverse.value ? -ScreenJumpTime : ScreenJumpTime));
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            analogNoise.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_AnalogNoise.NoiseSpeed.value, m_AnalogNoise.NoiseFading.value, m_AnalogNoise.LuminanceJitterThreshold.value, TimeX));
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            digitalStripe.SetFloat(CustomPostProcessingShaderConstants._Indensity, m_DigitalStripe.indensity.value);
            if (_noiseTexture != null)
            {
                digitalStripe.SetTexture(CustomPostProcessingShaderConstants._NoiseTex, _noiseTexture);
            }
            if (m_DigitalStripe.needStripColorAdjust.value)
            {
                digitalStripe.EnableKeyword("NEED_TRASH_FRAME");
                digitalStripe.SetColor(CustomPostProcessingShaderConstants._StripColorAdjustColor, m_DigitalStripe.stripColorAdjustColor.value);
                digitalStripe.SetFloat(CustomPostProcessingShaderConstants._StripColorAdjustIndensity, m_DigitalStripe.stripColorAdjustIndensity.value);
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            scanLineJitter.SetVector(CustomPostProcessingShaderConstants._Params, new Vector3(displacement, threshold, m_ScanLineJitter.IntervalType.value == IntervalType.Random ? randomFrequency : m_ScanLineJitter.Frequency.value));
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            tileJitter.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_TileJitter.SplittingNumber.value, m_TileJitter.Amount.value , m_TileJitter.Speed.value * 100f, m_TileJitter.IntervalType.value == IntervalType.Random ? randomFrequency : m_TileJitter.Frequency.value));
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            lineBlock.SetVector(CustomPostProcessingShaderConstants._Params, new Vector3(
                m_LineBlock.IntervalType.value == IntervalType.Random ? randomFrequency : m_LineBlock.Frequency.value,
                TimeX * m_LineBlock.Speed.value * 0.2f , m_LineBlock.Amount.value));
            lineBlock.SetVector(CustomPostProcessingShaderConstants._Params2, new Vector3(m_LineBlock.Offset.value, 1 / m_LineBlock.LinesWidth.value, m_LineBlock.Alpha.value));
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            imageBlock.SetVector(CustomPostProcessingShaderConstants._Params, new Vector3(TimeX * m_ImageBlock.Speed.value, m_ImageBlock.Amount.value, m_ImageBlock.Fade.value));
            imageBlock.SetVector(CustomPostProcessingShaderConstants._Params2, new Vector4(m_ImageBlock.BlockLayer1_U.value, m_ImageBlock.BlockLayer1_V.value, m_ImageBlock.BlockLayer2_U.value, m_ImageBlock.BlockLayer2_V.value));
            imageBlock.SetVector(CustomPostProcessingShaderConstants._Params3, new Vector3(m_ImageBlock.RGBSplitIndensity.value, m_ImageBlock.BlockLayer1_Indensity.value, m_ImageBlock.BlockLayer2_Indensity.value));
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            rgbSplit.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_RGBSplit.Fading.value, m_RGBSplit.Amount.value, m_RGBSplit.Speed.value, m_RGBSplit.CenterFading.value));
            rgbSplit.SetVector(CustomPostProcessingShaderConstants._Params2, new Vector3(TimeX, m_RGBSplit.AmountR.value, m_RGBSplit.AmountB.value));
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            float sinVal = (Mathf.Sin(m_DirectionalBlur.angle.value) * m_DirectionalBlur.indensity.value * 0.05f) / m_DirectionalBlur.blurCount.value;
            float cosVal = (Mathf.Cos(m_DirectionalBlur.angle.value) * m_DirectionalBlur.indensity.value * 0.05f) / m_DirectionalBlur.blurCount.value;  
            directionalBlur.SetVector(CustomPostProcessingShaderConstants._Params, new Vector3(m_DirectionalBlur.blurCount.value, sinVal, cosVal));
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            radialBlur.SetVector(CustomPostProcessingShaderConstants._Params, new Vector3(m_RadialBlur.indensity.value * 0.02f, m_RadialBlur.RadialCenterX.value, m_RadialBlur.RadialCenterY.value));
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            grainyBlur.SetVector(CustomPostProcessingShaderConstants._Params, new Vector2(m_GrainyBlur.indensity.value / opaqueDesc.height, m_GrainyBlur.blurCount.value));
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            irisBlur.SetVector(CustomPostProcessingShaderConstants._GoldenRot, mGoldenRot);
            irisBlur.SetVector(CustomPostProcessingShaderConstants._Gradient, new Vector3(m_IrisBlur.centerOffsetX.value, m_IrisBlur.centerOffsetY.value, m_IrisBlur.areaSize.value * 0.1f));
            irisBlur.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_IrisBlur.blurCount.value, m_IrisBlur.indensity.value, 1f / opaqueDesc.width, 1f / opaqueDesc.height));
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            tiltShiftBlur.SetVector(CustomPostProcessingShaderConstants._GoldenRot, mGoldenRot);
            tiltShiftBlur.SetVector(CustomPostProcessingShaderConstants._Gradient, new Vector3(m_TiltShiftBlur.centerOffset.value, m_TiltShiftBlur.areaSize.value, m_TiltShiftBlur.areaSmooth.value));
            tiltShiftBlur.SetVector(CustomPostProcessingShaderConstants._Params, new Vector4(m_TiltShiftBlur.blurCount.value, m_TiltShiftBlur.indensity.value, 1f / opaqueDesc.width, 1f / opaqueDesc.height));
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            bokehBlur.SetVector(CustomPostProcessingShaderConstants._GoldenRot, mGoldenRot);
            bokehBlur.SetVector(CustomPostProcessingShaderConstants._Offset, new Vector4(m_BokehBlur.blurCount.value, m_BokehBlur.indensity.value, 1f / opaqueDesc.width, 1f / opaqueDesc.height));
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
            dualKawaseBlur.SetFloat(CustomPostProcessingShaderConstants._Offset, m_DualKawaseBlur.indensity.value);
            RenderTargetIdentifier lastDown = m_ColorAttachment;
            for (int i = 0; i < m_DualKawaseBlur.blurCount.value; i++)
            {
                int mipDown = m_Pyramid[i].down;
                int mipUp = m_Pyramid[i].up;
                cmd.GetTemporaryRT(mipDown, width, height, 0, FilterMode.Bilinear);
                cmd.GetTemporaryRT(mipUp, width, height, 0, FilterMode.Bilinear);
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.GetTemporaryRT(m_TemporaryColorTexture02.id, opaqueDesc, FilterMode.Bilinear);
            bool needSwitch = true;
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            for (int i = 0; i < m_KawaseBlur.blurCount.value; i++) {
                kawaseBlur.SetFloat(CustomPostProcessingShaderConstants._Offset, i / m_KawaseBlur.downSample.value + m_KawaseBlur.indensity.value);
                cmd.Blit(needSwitch ? m_TemporaryColorTexture01.Identifier() : m_TemporaryColorTexture02.Identifier(), needSwitch ? m_TemporaryColorTexture02.Identifier() : m_TemporaryColorTexture01.Identifier(),kawaseBlur);
                needSwitch = !needSwitch;
            }
            kawaseBlur.SetFloat(CustomPostProcessingShaderConstants._Offset, m_KawaseBlur.blurCount.value / m_KawaseBlur.downSample.value + m_KawaseBlur.indensity.value);
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.GetTemporaryRT(m_TemporaryColorTexture02.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            for (int i = 0; i < m_BoxBlur.blurCount.value; i++) {
                boxBlur.SetVector(CustomPostProcessingShaderConstants._Offset, new Vector4(m_BoxBlur.indensity.value, 0, 0, 0));
                cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_TemporaryColorTexture02.Identifier(), boxBlur);
                boxBlur.SetVector(CustomPostProcessingShaderConstants._Offset, new Vector4(0, m_BoxBlur.indensity.value, 0, 0));
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
            cmd.GetTemporaryRT(m_TemporaryColorTexture01.id, opaqueDesc, FilterMode.Bilinear);
            cmd.GetTemporaryRT(m_TemporaryColorTexture02.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit(m_ColorAttachment, m_TemporaryColorTexture01.Identifier());
            for (int i = 0; i < m_GaussianBlur.blurCount.value; i++) {
                //y-direction
                gaussianBlur.SetVector(CustomPostProcessingShaderConstants._Offset, new Vector4(0, m_GaussianBlur.indensity.value, 0, 0));
                cmd.Blit(m_TemporaryColorTexture01.Identifier(), m_TemporaryColorTexture02.Identifier(), gaussianBlur);
                //x-direction
                gaussianBlur.SetVector(CustomPostProcessingShaderConstants._Offset, new Vector4(m_GaussianBlur.indensity.value, 0, 0, 0));
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


