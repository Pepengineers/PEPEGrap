using Pepengineers.PEPEGrap.GrabTexture.Passes;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Pepengineers.PEPEGrap
{
    [DisallowMultipleRendererFeature("GrabRenderTexture")]
    [Tooltip("GrabRenderTexture")]
    [SupportedOnRenderer(typeof(UniversalRendererData))]
    public sealed class GrabRenderTextureFeature : ScriptableRendererFeature
    {
        [SerializeField] [Tooltip("Don't forget about _ in the name if you want to use texture in shaders")]
        private string textureName = "_GrabRenderingTexture";

        [SerializeField] private RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

        private GrabPass grabPass;

        public override void Create()
        {
            grabPass = new GrabPass(textureName, renderPassEvent);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game && renderingData.cameraData.cameraType != CameraType.SceneView) return;
            if (renderingData.cameraData.requiresOpaqueTexture == false)
            {
                Debug.LogWarning("Opaque Texture was not enabled");
                return;
            }

            renderer.EnqueuePass(grabPass);
        }
    }
}