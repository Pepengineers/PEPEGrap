using Pepengineers.PEPEGrap.GrabTexture.Data;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Pepengineers.PEPEGrap.GrabTexture.Passes
{
    internal sealed class GrabPass : ScriptableRenderPass
    {
        private readonly string textureName;
        private readonly int textureShaderIndex;

        public GrabPass(string textureName, RenderPassEvent renderPassEvent)
        {
            this.textureName = textureName;
            textureShaderIndex = Shader.PropertyToID(textureName);
            this.renderPassEvent = renderPassEvent;
            profilingSampler = new ProfilingSampler($"{nameof(GrabPass)}:{textureName}");
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            var capturedData = frameData.GetOrCreate<GrabData>();

            var source = resourceData.activeColorTexture;

            var desc = renderGraph.GetTextureDesc(source);
            desc.name = textureName;
            desc.clearBuffer = false;

            capturedData.Texture = renderGraph.CreateTexture(desc);
            var destination = capturedData.Texture;

            using (var builder = renderGraph.AddUnsafePass<PassData>(string.Empty, out var passData, profilingSampler))
            {
                passData.DestinationTexture = destination;
                passData.SourceTexture = source;

                builder.AllowPassCulling(false);
                builder.AllowGlobalStateModification(true);
                builder.SetGlobalTextureAfterPass(destination, textureShaderIndex);
                builder.UseTexture(source);
                builder.UseTexture(destination, AccessFlags.Write);
                builder.SetRenderFunc((PassData data, UnsafeGraphContext context) =>
                {
                    var unsafeCmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
                    unsafeCmd.CopyTexture(data.SourceTexture, data.DestinationTexture);
                });
            }
        }

        private class PassData
        {
            internal TextureHandle DestinationTexture;
            internal TextureHandle SourceTexture;
        }
    }
}