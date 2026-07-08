using Crewmeleon.Roles;
using FungleAPI.Api;
using FungleAPI.Extensions;
using FungleAPI.GameModes;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Translation;
using Il2CppSystem.Xml.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace Crewmeleon.Essential
{
    public class ChameleonGameMode : NormalGameMode
    {
        public TextMeshPro TitleText;
        public ProgressTracker ProgressTracker;
        public float Time;
        public override GameModeOptions ModeOptions { get; } = new ChameleonModeSettings();
        public override StringNames GameModeName => TranslationManager.GetStringName("Camaleões");
        public override void OnGameStart()
        {
            HudManager.Instance.CrewmatesKilled.gameObject.SetActive(true);

            ProgressTracker = HudManager.Instance.TaskPanel.transform.parent.GetChild(1).GetComponent<ProgressTracker>();
            TitleText = ProgressTracker.transform.GetChild(2).GetComponent<TextMeshPro>();

            Time = ChameleonModeSettings.GeneralSettings.Time.FloatValue;
        }
        public override float GetKillCooldown()
        {
            return ChameleonModeSettings.GeneralSettings.KillCooldown.FloatValue;
        }
        public override bool CanReportBodies()
        {
            return false;
        }
        public override void OnPlayerDeath(PlayerControl player, bool assignGhostRole)
        {
            HudManager.Instance.CrewmatesKilled.OnCrewmateKilled();
        }
        public override bool CanUse(IUsable usable, PlayerControl player)
        {
            return false;
        }
        public override MapOptions GetMapOptions()
        {
            return new MapOptions
            {
                Mode = MapOptions.Modes.Normal
            };
        }
        public override TaskBarMode GetTaskBarMode()
        {
            return TaskBarMode.Normal;
        }
        public override void SelectRoles(RoleManager roleManager)
        {
            List<PlayerControl> playerControls = PlayerControl.AllPlayerControls.ToSystemList();
            for (int i = 0; i < ChameleonModeSettings.GeneralSettings.SeekersCount.IntValue; i++)
            {
                if (playerControls.Count <= 0) break;

                PlayerControl playerControl = playerControls.Random();
                playerControls.Remove(playerControl);

                playerControl.RpcSetRole(CustomRoleManager.GetRoleType<SeekerRole>());
            }
            foreach (PlayerControl playerControl in playerControls)
            {
                playerControl.RpcSetRole(CustomRoleManager.GetRoleType<ChameleonRole>());
            }
        }
        public override void CheckEndCriteria()
        {
            Time -= UnityEngine.Time.deltaTime;
            ProgressTracker.curValue = Time / ChameleonModeSettings.GeneralSettings.Time.FloatValue;
            TitleText.text = $"Tempo restante: {(int)Time}s";

            if (Time <= 0)
            {
                Manager.RpcEndGame(GameOverReason.CrewmatesByTask, false);
                return;
            }

            if (!PlayerControl.AllPlayerControls.Any(p => p.Data.Role.TeamType != RoleTeamTypes.Impostor && !p.Data.IsDead))
            {
                Manager.RpcEndGame(GameOverReason.ImpostorsByKill, false);
            }
        }
    }
}
