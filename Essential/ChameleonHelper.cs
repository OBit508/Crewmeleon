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
        private static PaintState state = PaintState.None;

        public static LateSprite DefaultIdle = new LateSprite("Crewmeleon.Assets.DefaultIdle.png", 200);
        public static LateSprite ColorButton = new LateSprite("Crewmeleon.Assets.ColorButton.png", 450);
        public static LateSprite ColorGradient = new LateSprite("Crewmeleon.Assets.ColorGradient.png", 100);

        public static LateSprite ColorBrush = new LateSprite("Crewmeleon.Assets.ColorBrush.png", 200, true);
        public static LateSprite ColorDropper = new LateSprite("Crewmeleon.Assets.ColorDropper.png", 100, true);

        public static Color32 BrushColor = Color.white;
        public static PaintState PaintState
        {
            get => state;
            set
            {
                state = value;

                if (state == PaintState.None)
                {
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    return;
                }

                Texture2D texture2D = state == PaintState.Painting ? ColorBrush.Asset.texture : ColorDropper.Asset.texture;

                Cursor.SetCursor(texture2D, new Vector2(0, texture2D.height), CursorMode.Auto);
            }
        }

        public static Color32 GetMousePixelColor()
        {
            if (texture == null)
            {
                texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            }

            Vector2 pos = Input.mousePosition;

            texture.ReadPixels(new Rect(pos.x, pos.y, 1, 1), 0, 0);

            texture.Apply();

            return texture.GetPixel(0, 0);
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
