using Crewmeleon.Essential;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Crewmeleon.Components
{
    public class MouseIconBehaviour : MonoBehaviour
    {
        public PaintState LastIcon;
        public PaintState CurrentIcon;
        public void Update()
        {
            if (AmongUsClient.Instance == null) return;

            CurrentIcon = PaintState.None;
            if (AmongUsClient.Instance.IsInGame)
            {
                CurrentIcon = ChameleonHelper.PaintState;
            }

            if (LastIcon != CurrentIcon)
            {
                LastIcon = CurrentIcon;
                if (CurrentIcon == PaintState.None)
                {
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    return;
                }

                Texture2D texture2D = CurrentIcon == PaintState.Painting ? ChameleonAssets.ColorBrush.Asset.texture : ChameleonAssets.ColorDropper.Asset.texture;

                Cursor.SetCursor(texture2D, new Vector2(0, texture2D.height), CursorMode.Auto);
            }
        }
    }
}
