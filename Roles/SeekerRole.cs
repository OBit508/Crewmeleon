using BepInEx.Unity.IL2CPP.Utils.Collections;
using Crewmeleon.Components;
using Crewmeleon.Essential;
using Crewmeleon.GameMode;
using Crewmeleon.RPC;
using FungleAPI.Base.Roles;
using FungleAPI.GameModes;
using FungleAPI.Networking;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Crewmeleon.Roles
{
    public class SeekerRole : ImpostorBase, ICustomRole
    {
        public static List<NetworkedPlayerInfo> SafeToKill = new List<NetworkedPlayerInfo>();

        public ModdedTeam Team { get; } = ModdedTeamManager.Impostors;
        public StringNames RoleName { get; } = ChameleonTranslation.SeekerName;
        public StringNames RoleBlur { get; } = ChameleonTranslation.SeekerBlur;
        public StringNames RoleBlurMed { get; } = ChameleonTranslation.SeekerBlur;
        public StringNames RoleBlurLong { get; } = ChameleonTranslation.SeekerBlur;
        public Color RoleColor { get; } = Color.red;
        public RoleConfiguration Configuration => new RoleConfiguration(this)
        {
            CanUseVent = false,
            CanSabotage = false,
            HideInLobby = true
        };
        public KillButtonConfig KillConfig { get; } = new KillButtonConfig(out KillButtonConfig self)
        {
            SetTarget = delegate (PlayerControl target)
            {
                self.Button.currentTarget = target;
            },
            DoClick = delegate
            {
                if (self.CanUse())
                {
                    if (ChameleonModeSettings.InfectionSettings.Infection.BooleanValue)
                    {
                        Rpc<RpcSetSeeker>.Instance.Send(self.Button.currentTarget, PlayerControl.LocalPlayer);
                    }
                    else
                    {
                        PlayerControl.LocalPlayer.CmdCheckMurder(self.Button.currentTarget);
                    }
                    self.SetTarget(null);
                }
            }
        };
        public override bool ValidTarget(NetworkedPlayerInfo target)
        {
            return base.ValidTarget(target) && SafeToKill.Contains(target);
        }
        public void StartStun(bool stun)
        {
            StartCoroutine(CoStun(stun).WrapToIl2Cpp());
        }
        public System.Collections.IEnumerator CoStun(bool stun)
        {
            if (Player.AmOwner && stun)
            {
                HudManager.Instance.FullScreen.gameObject.SetActive(true);
                HudManager.Instance.FullScreen.color = Color.black;
            }

            Player.MyPhysics.Speed = 0;
            Player.rigidbody2D.velocity = Vector2.zero;
            Player.cosmetics.SetBodyCosmeticsVisible(false);
            yield return Player.MyPhysics.CoAnimateCustom(HudManager.Instance.IntroPrefab.HnSSeekerSpawnAnim);
            Player.cosmetics.ToggleHat(true);
            Player.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
            Player.MyPhysics.Speed = 0;
            Player.rigidbody2D.velocity = Vector2.zero;

            if (GameMode<ChameleonGameMode>.Instance.HideTime <= 0)
            {
                if (Player.AmOwner && stun)
                {
                    HudManager.Instance.FullScreen.gameObject.SetActive(false);
                }
                Player.MyPhysics.Speed = 2.5f;
                yield break;
            }
            while (GameMode<ChameleonGameMode>.Instance.HideTime > 0)
            {
                yield return null;
            }

            if (Player.AmOwner && stun)
            {
                HudManager.Instance.FullScreen.gameObject.SetActive(false);
            }
            Player.MyPhysics.Speed = 2.5f;
        }
    }
}
