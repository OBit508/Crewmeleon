using Crewmeleon.Components;
using Crewmeleon.Essential;
using Crewmeleon.Roles;
using FungleAPI.Base.Buttons;
using FungleAPI.Extensions;
using FungleAPI.Hud;
using FungleAPI.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crewmeleon.Buttons
{
    internal class SpectateButton : RoleButton<ChameleonRole>
    {
        public override bool Active => base.Active && ChameleonHelper.PaintState == PaintState.None;
        public override ButtonLocation Location => ButtonLocation.BottomLeft;
        public override Sprite ButtonSprite => ChameleonAssets.Spectate1;
        public override string OverrideText => ChameleonTranslation.Spectate.GetString();
        public override Color32 TextOutlineColor => Color.black;
        public override float Cooldown => 1f;
        public override void OnClick()
        {
            SpectatorBehaviour spectatorBehaviour = Player.GetComponent<SpectatorBehaviour>();

            spectatorBehaviour.Spectating = !spectatorBehaviour.Spectating;
            spectatorBehaviour.Update();

            Button.graphic.sprite = spectatorBehaviour.Spectating ? ChameleonAssets.Spectate2 : ChameleonAssets.Spectate1;
        }
    }
}