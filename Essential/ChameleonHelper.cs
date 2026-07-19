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
        private static Texture2D texture;
        public static Color32 BrushColor = Color.white;
        public static PaintState PaintState;
        public static Color32 GetMousePixelColor()
        {
            if (texture == null)
            {
                texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            }

            Vector2 pos = Input.mousePosition;

            texture.ReadPixels(new Rect(pos.x, pos.y, 1, 1), 0, 0);

            texture.Apply();

            Color32 color = texture.GetPixel(0, 0);
            color.a = byte.MaxValue;

            return color;
        }
        public static Texture2D ApplyMaterial(Texture2D source, Material material)
        {
            RenderTexture rt = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.ARGB32);

            Graphics.Blit(source, rt, material);

            RenderTexture.active = rt;

            Texture2D result = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);

            result.ReadPixels( new Rect(0, 0, source.width, source.height), 0, 0);

            result.Apply();

            RenderTexture.active = null;
            rt.Release();

            return result;
        }
    }
}
