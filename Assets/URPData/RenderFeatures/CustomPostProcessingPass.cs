using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends
{
    public class CustomPostProcessingPass : ScriptableRenderPass
    {
        private List<CustomPostProcessingBase> _customPostProcessings;
        private List<int> _activeCustomPostprocessingIndex;

        private string _profilerTag;
        private List<ProfilingSampler> _profilingSamplers;

        private RTHandle _sourceRT;
        private RTHandle _destinationRT;
        private RTHandle _tempRT0;
        private RTHandle _tempRT1;

        private string _tempRT0Name => "TemporaryRenderTexture0";
        private string _tempRT1Name => "TemporaryRenderTexture1";

        public CustomPostProcessingPass(string profilerTag, List<CustomPostProcessingBase> customPostProcessings)
        {
            _profilerTag = profilerTag;
            _customPostProcessings = customPostProcessings;
            _activeCustomPostprocessingIndex = new List<int>(customPostProcessings.Count);
            _profilingSamplers = customPostProcessings.Select(c => new ProfilingSampler(c.ToString())).ToList();

        }

        public bool SetupCustomPostProcessing()
        {
            _activeCustomPostprocessingIndex.Clear();
            for (int i = 0; i < _customPostProcessings.Count; i++)
            {
                // if (_customPostProcessings[i] != null)
                // {
                    _customPostProcessings[i].Setup();
                    if (_customPostProcessings[i].IsActive())
                    {
                        _activeCustomPostprocessingIndex.Add(i);
                    }
                // }
                
            }

            return _activeCustomPostprocessingIndex.Count != 0;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.msaaSamples = 1;
            descriptor.depthBufferBits = 0;
            
            RenderingUtils.ReAllocateIfNeeded(ref _tempRT0, descriptor, name: _tempRT0Name);
            RenderingUtils.ReAllocateIfNeeded(ref _tempRT1, descriptor, name: _tempRT1Name);

            foreach (var i in _activeCustomPostprocessingIndex)
            {
                _customPostProcessings[i].OnCameraSetup(cmd, ref renderingData);
            }
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            _sourceRT = null;
            _destinationRT = null;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            
            
            _destinationRT = renderingData.cameraData.renderer.cameraColorTargetHandle;
            _sourceRT = renderingData.cameraData.renderer.cameraColorTargetHandle;


            if (_activeCustomPostprocessingIndex.Count == 1)
            {
                int index = _activeCustomPostprocessingIndex[0];
                using (new ProfilingScope(cmd, _profilingSamplers[index]))
                {
                    _customPostProcessings[index].Render(cmd, ref renderingData, _sourceRT, _tempRT0);
                }
            }
            else
            {
                Blitter.BlitCameraTexture(cmd, _sourceRT, _tempRT0);
                
                for (int i = 0; i < _activeCustomPostprocessingIndex.Count; i++)
                {
                    int index = _activeCustomPostprocessingIndex[i];
                    var customProcessing = _customPostProcessings[index];
                    using (new ProfilingScope(cmd, _profilingSamplers[index]))
                    {
                        customProcessing.Render(cmd, ref renderingData, _tempRT0, _tempRT1);
                    }
                    CoreUtils.Swap(ref _tempRT0, ref _tempRT1);
                }
            }
            
            Blitter.BlitCameraTexture(cmd, _tempRT0, _destinationRT);
            
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Dispose()
        {
            _tempRT0?.Release();
            _tempRT1?.Release();
        }
    }
    
    
}

