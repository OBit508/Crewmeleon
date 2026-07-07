using Crewmeleon.Essential;
using Crewmeleon.Roles;
using FungleAPI.Base.Buttons;
using FungleAPI.Hud;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Crewmeleon.Buttons
{
    [FungleAPI.Attributes.RegisterPriority(0)]
    public class DrawButton : RoleButton<ChameleonRole>
    {
        public override ButtonLocation Location => ButtonLocation.BottomLeft;
        public override Sprite ButtonSprite => ChameleonHelper.ColorButton;
        public override string OverrideText => string.Empty;
        public override Color32 TextOutlineColor => Color.black;
        public override float Cooldown => 1;
        public override void OnClick()
        {
            if (ChameleonHelper.PaintState == PaintState.None)
            {
                ChameleonHelper.PaintState = PaintState.Painting;
            }
            else
            {
                ChameleonHelper.PaintState = PaintState.None;
            }
            HudHelper.UpdateActiveState();
        }
        public override void Update()
        {
            base.Update();
            Button.buttonLabelText.text = ChameleonHelper.PaintState == PaintState.None ? "Desenhar" : "Parar";

            if (ChameleonRole.Local != null)
            {
                Button.graphic.sprite = ChameleonRole.Local.sprite;
            }
        }
    }
}
