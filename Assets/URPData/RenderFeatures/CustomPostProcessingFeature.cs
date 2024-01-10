using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends
{
    public class CustomPostProcessingFeature : ScriptableRendererFeature
    {
        private List<CustomPostProcessingBase> _customPostProcessings;
        
        private CustomPostProcessingPass _afterOpaqueAndSkyExtendsPass;
        private CustomPostProcessingPass _beforePostProcessingExtendsPass;
        private CustomPostProcessingPass _afterPostProcessingExtendsPass;

        public override void Create()
        {
            var stack = VolumeManager.instance.stack;

            _customPostProcessings = VolumeManager.instance.baseComponentTypeArray
                .Where(t => t.IsSubclassOf(typeof(CustomPostProcessingBase)))
                .Select(t => stack.GetComponent(t) as CustomPostProcessingBase).ToList();

            var afterOpaqueAndSkyExtends = _customPostProcessings
                .Where(c => c.InjectionPoint == CustomPostProcessingInjectionPoint.AfterOpaqueAndSkybox)
                .OrderBy(c => c.OrderInInjectionPoint).ToList();

            _afterOpaqueAndSkyExtendsPass =
                new CustomPostProcessingPass("Custom PostProcess After Skybox", afterOpaqueAndSkyExtends);
            _afterOpaqueAndSkyExtendsPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;

            var beforePostProcessingExtends = _customPostProcessings
                .Where(c => c.InjectionPoint == CustomPostProcessingInjectionPoint.BeforePostProcess)
                .OrderBy(c => c.OrderInInjectionPoint).ToList();
            _beforePostProcessingExtendsPass = new CustomPostProcessingPass("Custom PostProcess Before PostProcess", beforePostProcessingExtends);
            _beforePostProcessingExtendsPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

            var afterPostProcessingExtends = _customPostProcessings
                .Where(c => c.InjectionPoint == CustomPostProcessingInjectionPoint.AfterPostProcess)
                .OrderBy(c => c.OrderInInjectionPoint).ToList();
            _afterPostProcessingExtendsPass = new CustomPostProcessingPass("Custom PostProcess After PostProcess", afterPostProcessingExtends);
            _afterPostProcessingExtendsPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.postProcessEnabled)
            {
                if (_afterPostProcessingExtendsPass.SetupCustomPostProcessing())
                {
                    _afterOpaqueAndSkyExtendsPass.ConfigureInput(ScriptableRenderPassInput.Color);
                    renderer.EnqueuePass(_afterOpaqueAndSkyExtendsPass);
                }

                if (_beforePostProcessingExtendsPass.SetupCustomPostProcessing())
                {
                    _beforePostProcessingExtendsPass.ConfigureInput(ScriptableRenderPassInput.Color);
                    renderer.EnqueuePass(_beforePostProcessingExtendsPass);
                }
                
                if (_afterPostProcessingExtendsPass.SetupCustomPostProcessing())
                {
                    _afterPostProcessingExtendsPass.ConfigureInput(ScriptableRenderPassInput.Color);
                    renderer.EnqueuePass(_afterPostProcessingExtendsPass);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            _afterOpaqueAndSkyExtendsPass.Dispose();
            _beforePostProcessingExtendsPass.Dispose();
            _afterPostProcessingExtendsPass.Dispose();
            
            if (disposing && _customPostProcessings != null)
            {
                foreach (var c in _customPostProcessings)
                {
                    c.Dispose();
                }
            }
        }
    }
}

