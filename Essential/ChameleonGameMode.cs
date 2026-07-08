using AmongUs.GameOptions;
using Crewmeleon.Roles;
using FungleAPI.Api;
using FungleAPI.Components;
using FungleAPI.Extensions;
using FungleAPI.GameModes;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using Il2CppSystem.Xml.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Crewmeleon.Essential
{
    public class ChameleonGameMode : NormalGameMode
    {
        public TextMeshPro TitleText;
        public ProgressTracker ProgressTracker;
        public float Time;
        public float HideTime;
        public float RevelationTime;
        public bool CanCount;
        public override GameModeOptions ModeOptions { get; } = new ChameleonModeSettings();
        public override StringNames GameModeName => TranslationManager.GetStringName("Camaleões");
        public override void OnGameStart()
        {
            HudManager.Instance.CrewmatesKilled.gameObject.SetActive(true);

            ProgressTracker = HudManager.Instance.TaskPanel.transform.parent.GetChild(1).GetComponent<ProgressTracker>();
            TitleText = ProgressTracker.transform.GetChild(2).GetComponent<TextMeshPro>();

            Time = ChameleonModeSettings.GeneralSettings.Time.FloatValue;
            HideTime = ChameleonModeSettings.GeneralSettings.HideTime.FloatValue;
            RevelationTime = 15;

            Manager.gameObject.AddComponent<Updater>().update = delegate
            {
                if (CanCount)
                {
                    if (HideTime > 0)
                    {
                        HideTime -= UnityEngine.Time.deltaTime;
                        ProgressTracker.curValue = HideTime / ChameleonModeSettings.GeneralSettings.HideTime.FloatValue;
                        TitleText.text = $"Tempo para se esconder: {(int)HideTime}s";
                    }
                    else if (Time > 0)
                    {
                        Time -= UnityEngine.Time.deltaTime;
                        ProgressTracker.curValue = Time / ChameleonModeSettings.GeneralSettings.Time.FloatValue;
                        TitleText.text = $"Tempo restante: {(int)Time}s";
                        if (Time <= 0)
                        {
                            AudioClip audioClip = RoleManager.Instance.GetRole(RoleTypes.Noisemaker).SafeCast<NoisemakerRole>().deathSound;

                            SoundManager.Instance.PlaySound(audioClip, false, 1.5f);

                            foreach (PlayerControl playerControl in PlayerControl.AllPlayerControls)
                            {
                                if (playerControl.Data.Role.Is(out ChameleonRole chameleonRole))
                                {
                                    chameleonRole.Reveal();
                                }
                            }
                            SeekerRole.SafeToKill.Clear();
                        }
                    }
                    else
                    {
                        RevelationTime -= UnityEngine.Time.deltaTime;
                        ProgressTracker.curValue = RevelationTime / 15;
                        TitleText.text = $"Revelação: {(int)RevelationTime}s";
                    }
                    ProgressTracker.enabled = false;
                    ProgressTracker.TileParent.enabled = true;
                    ProgressTracker.TileParent.material.SetFloat("_Buckets", 1);
                    ProgressTracker.TileParent.material.SetFloat("_FullBuckets", ProgressTracker.curValue);
                }
            };
        }
        public override void OnGameEnd()
        {
            Manager.GetComponent<Updater>()?.Destroy();
        }
        public override bool GetChatInGame()
        {
            return ChameleonModeSettings.GeneralSettings.ChatDuringGame.BooleanValue;
        }
        public override float GetKillCooldown()
        {
            return ChameleonModeSettings.GeneralSettings.KillCooldown.FloatValue;
        }
        public override float GetPlayerSpeedMod(PlayerControl pc)
        {
            return ChameleonModeSettings.GeneralSettings.PlayerSpeed.FloatValue;
        }
        public override bool CanReportBodies()
        {
            return false;
        }
        public override void OnPlayerDeath(PlayerControl player, bool assignGhostRole)
        {
            HudManager.Instance.CrewmatesKilled.OnCrewmateKilled();
            if (AmongUsClient.Instance.AmHost && assignGhostRole)
            {
                player.RpcSetRole(RoleTypes.CrewmateGhost, false);
            }
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
            EnumerableExtensions.Shuffle(playerControls);

            for (int i = 0; i < ChameleonModeSettings.GeneralSettings.SeekersCount.IntValue; i++)
            {
                if (playerControls.Count <= 0) break;

                PlayerControl playerControl = playerControls[0];
                playerControls.RemoveAt(0);

                playerControl.RpcSetRole(CustomRoleManager.GetRoleType<SeekerRole>());
            }
            foreach (PlayerControl playerControl in playerControls)
            {
                playerControl.RpcSetRole(CustomRoleManager.GetRoleType<ChameleonRole>());
            }
        }
        public override void CheckEndCriteria()
        {
            if (RevelationTime <= 0)
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
