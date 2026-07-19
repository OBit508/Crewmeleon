using Crewmeleon.Essential;
using Crewmeleon.Roles;
using FungleAPI.Base.Buttons;
using FungleAPI.Hud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Crewmeleon.Buttons
{
    public class ColorButton : RoleButton<ChameleonRole>
    {
        public override bool Active => base.Active && ChameleonHelper.PaintState != PaintState.None;
        public override ButtonLocation Location => ButtonLocation.BottomLeft;
        public override Sprite ButtonSprite => ChameleonAssets.ColorButton;
        public override string OverrideText => "Conta gotas";
        public override Color32 TextOutlineColor => Color.black;
        public override float Cooldown => 1;
        public override void OnClick() 
        {
            ChameleonHelper.PaintState = PaintState.ColorPicker;
        }
        public override void Update()
        {
            base.Update();
            Button.graphic.color = ChameleonHelper.BrushColor;

            if (Input.GetMouseButtonDown(0) && ChameleonHelper.PaintState == PaintState.ColorPicker)
            {
                ChameleonHelper.PaintState = PaintState.Painting;
                ChameleonHelper.BrushColor = ChameleonHelper.GetMousePixelColor();
            }
        }
    }
}
