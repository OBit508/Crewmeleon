using FungleAPI.Assets.Late;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crewmeleon.Essential
{
    public static class ChameleonAssets
    {
        public static LateSprite Spectate1 = new LateSprite("Crewmeleon.Assets.Spectate1.png", 200);
        public static LateSprite Spectate2 = new LateSprite("Crewmeleon.Assets.Spectate2.png", 200);
        public static LateSprite DefaultIdle = new LateSprite("Crewmeleon.Assets.DefaultIdle.png", 200);
        public static LateSprite ColorButton = new LateSprite("Crewmeleon.Assets.ColorButton.png", 450);
        public static LateSprite SpectatorBackground = new LateSprite("Crewmeleon.Assets.SpectatorBackground.png", 100);
        public static LateSprite Gradient = new LateSprite("Crewmeleon.Assets.Gradient.png", 130);

        public static LateSprite ColorBrush = new LateSprite("Crewmeleon.Assets.ColorBrush.png", 200, true);
        public static LateSprite ColorDropper = new LateSprite("Crewmeleon.Assets.ColorDropper.png", 100, true);
    }
}
