using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace Pepengineers.PEPEGrap.GrabTexture.Data
{
    public sealed class GrabData : ContextItem
    {
        public TextureHandle Texture { get; internal set; }

        public override void Reset()
        {
            Texture = TextureHandle.nullHandle;
        }
    }
}