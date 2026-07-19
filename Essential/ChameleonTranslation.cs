using FungleAPI.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crewmeleon.Essential
{
    public static class ChameleonTranslation
    {
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
    }
}
