using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessingExtends
{
    public abstract class CustomPostProcessingBase : VolumeComponent, IPostProcessComponent, IDisposable
    {
        protected Material _material = null;
        private Material _copyMaterial = null;

        private const string _copyShaderName = "Hidden/PostProcess/PostProcessCopy";

        private int _SourceTextureId = Shader.PropertyToID("_SourceTexture");
        

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_copyMaterial == null)
            {
                _copyMaterial = CoreUtils.CreateEngineMaterial(_copyShaderName);
            }
            
        }

        protected RenderTextureDescriptor GetCameraRenderTextureDescriptor(RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.msaaSamples = 1;
            descriptor.depthBufferBits = 0;
            descriptor.useMipMap = false;
            return descriptor;
        }

        public virtual void Draw(CommandBuffer cmd, in RTHandle source, in RTHandle destination, int pass = -1)
        {
            cmd.SetGlobalTexture(_SourceTextureId, source);
            cmd.SetRenderTarget(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            if (pass == -1 || _material == null)
            {
                cmd.DrawProcedural(Matrix4x4.identity, _copyMaterial, 0, MeshTopology.Triangles, 3);
            }
            else
            {
                cmd.DrawProcedural(Matrix4x4.identity, _material, pass, MeshTopology.Triangles, 3);
            }
        }

        protected void SetKeyword(string keyword, bool enabled = true)
        {
            if (enabled)
            {
                _material.EnableKeyword(keyword);
            }
            else
            {
                _material.DisableKeyword(keyword);
            }
        }

        public abstract bool IsActive();
        
        public virtual bool IsTileCompatible() => false;

        public virtual CustomPostProcessingInjectionPoint InjectionPoint =>
            CustomPostProcessingInjectionPoint.AfterPostProcess;

        public virtual int OrderInInjectionPoint => 0;

        public virtual void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            
        }

        public abstract void Setup();

        public abstract void Render(CommandBuffer cmd, ref RenderingData renderingData, in RTHandle source,
            in RTHandle destination);
        
        

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            
        }
    }
}
