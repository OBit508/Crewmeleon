using FungleAPI.Assets.Late;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Rewired.Demos.CustomPlatform.MyPlatformControllerExtension;

namespace Crewmeleon.Essential
{
    public static class ChameleonHelper
    {
        public static LateSprite DefaultIdle = new LateSprite("Crewmeleon.Assets.DefaultIdle.png", 200);
        public static Texture2D ApplyMaterial(Texture2D source, Material material)
        {
            RenderTexture rt = new RenderTexture(
                source.width,
                source.height,
                0,
                RenderTextureFormat.ARGB32
            );

            Graphics.Blit(source, rt, material);

            RenderTexture.active = rt;

            Texture2D result = new Texture2D(
                source.width,
                source.height,
                TextureFormat.RGBA32,
                false
            );

            result.ReadPixels(
                new Rect(0, 0, source.width, source.height),
                0,
                0
            );

            result.Apply();

            RenderTexture.active = null;
            rt.Release();

            return result;
        }
        public static Texture2D MakeReadable(Texture2D texture)
        {
            if (texture == null)
                return null;

            RenderTexture rt = RenderTexture.GetTemporary(
                texture.width,
                texture.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);

            Graphics.Blit(texture, rt);

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = rt;

            Texture2D readable = new Texture2D(
                texture.width,
                texture.height,
                TextureFormat.RGBA32,
                false);

            readable.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            readable.Apply();

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(rt);

            return readable;
        }

    }
}
