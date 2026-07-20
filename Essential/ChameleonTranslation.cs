using FungleAPI.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crewmeleon.Essential
{
    [Translation("Crewmeleon.Assets.Languages")]
    public static class ChameleonTranslation
    {
        public static Translator Stop { get; } = new Translator("Stop");
        public static Translator Draw { get; } = new Translator("Draw");
        public static Translator Dropper { get; } = new Translator("Color Dropper");
        public static Translator Gradient { get; } = new Translator("Gradient");
        public static Translator Zoom { get; } = new Translator("Zoom");
        public static Translator Spectate { get; } = new Translator("Spectate");

        public static Translator SeekerName { get; } = new Translator("Seeker");
        public static Translator SeekerBlur { get; } = new Translator("Find all chameleons");
        public static Translator ChameleonName { get; } = new Translator("Chameleon");
        public static Translator ChameleonBlur { get; } = new Translator("Camouflage yourself to fool the seekers");

        public static Translator InfectionText { get; } = new Translator("Infection");
        public static Translator ChameleonsText { get; } = new Translator("Chameleons");

        public static Translator ChameleonSpeed { get; } = new Translator("Chameleon speed");
        public static Translator SeekerSpeed { get; } = new Translator("Seeker speed");
        public static Translator SeekerCount { get; } = new Translator("Seeker count");
        public static Translator GameDuration { get; } = new Translator("Game duration");
        public static Translator HideTime { get; } = new Translator("Hide time");
        public static Translator ShowNames { get; } = new Translator("Show names");
        public static Translator ChatActive { get; } = new Translator("Chat during the game");

        public static Translator InfectionActive { get; } = new Translator("Infection active");
        public static Translator HidePlayers { get; } = new Translator("Hide players");

        public static Translator CamouflageTime { get; } = new Translator("Camouflage time");
        public static Translator MaxProximity { get; } = new Translator("Max proximity");
        public static Translator DangerBarLevel { get; } = new Translator("Danger bar level");

        public static Translator HideTimerText { get; } = new Translator("Time to hide: {0}s");
        public static Translator RemainTimeText { get; } = new Translator("Time remaining: {0}s");
        public static Translator RevelationText { get; } = new Translator("Revelation: {0}s");
    }
}
