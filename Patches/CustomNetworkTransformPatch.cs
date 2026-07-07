using Crewmeleon.Essential;
using FungleAPI.GameModes;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crewmeleon.Patches
{
    [HarmonyPatch(typeof(CustomNetworkTransform))]
    internal static class CustomNetworkTransformPatch
    {
        [HarmonyPatch(nameof(CustomNetworkTransform.Serialize))]
        [HarmonyPrefix]
        public static void SerializePrefix(CustomNetworkTransform __instance)
        {
            bool flag = GameModeManager.GetCurrentGameMode().GameModeId == GameMode<ChameleonGameMode>.Instance.GameModeId;
        }
        [HarmonyPatch(nameof(CustomNetworkTransform.Deserialize))]
        [HarmonyPrefix]
        public static void DeserializePrefix(CustomNetworkTransform __instance)
        {

        }
    }
}
