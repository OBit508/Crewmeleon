using FungleAPI.GameModes;
using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Options;
using FungleAPI.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crewmeleon.Essential
{
    public class ChameleonModeSettings : GameModeOptions
    {
        public class GeneralSettings : SettingsGroup
        {
            public override StringNames GroupName => TranslationManager.GetStringName("Geral");
            public static ModdedNumberOption Time = new ModdedNumberOption("Duração do jogo", 150, 30, 500);
            public static ModdedNumberOption KillCooldown = new ModdedNumberOption("Recarga para matar", 3, 1, 10);
            public static ModdedNumberOption SeekersCount = new ModdedNumberOption("Numero de procuradores", 2, 1, 10);
            public static ModdedNumberOption HideTime = new ModdedNumberOption("Tempo para se esconder", 15, 1, 50);
        }
        public class ChameleonSettings : SettingsGroup
        {
            public override StringNames GroupName => TranslationManager.GetStringName("Camaleões");
            public static ModdedNumberOption StopOutline = new ModdedNumberOption("Tempo para camuflar", 2.5f, 1, 10);
            public static ModdedNumberOption Proximity = new ModdedNumberOption("Proximidade máxima", 1.5f, 0.2f, 3, 0.1f);
            public static ModdedNumberOption FoundBar = new ModdedNumberOption("Nivel da barra de proximidade", 0.8f, 0.2f, 2, 0.1f);
        }
    }
}
