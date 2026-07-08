using BepInEx.Unity.IL2CPP.Utils.Collections;
using Crewmeleon.Components;
using Crewmeleon.Essential;
using FungleAPI.Base.Roles;
using FungleAPI.GameModes;
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
        public StringNames RoleName { get; } = TranslationManager.GetStringName("Procurador");
        public StringNames RoleBlur { get; } = TranslationManager.GetStringName("Encontre os camaleões");
        public StringNames RoleBlurMed { get; } = TranslationManager.GetStringName("Encontre os camaleões");
        public StringNames RoleBlurLong { get; } = TranslationManager.GetStringName("Encontre os camaleões");
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
            }
        };
        public override bool ValidTarget(NetworkedPlayerInfo target)
        {
            return base.ValidTarget(target) && SafeToKill.Contains(target);
        }
        public void StartStun()
        {
            StartCoroutine(CoStun().WrapToIl2Cpp());
        }
        public System.Collections.IEnumerator CoStun()
        {
            Player.cosmetics.SetBodyCosmeticsVisible(false);
            yield return Player.MyPhysics.CoAnimateCustom(HudManager.Instance.IntroPrefab.HnSSeekerSpawnAnim);
            Player.cosmetics.ToggleHat(true);
            Player.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
            Player.moveable = false;
            
            while (GameMode<ChameleonGameMode>.Instance.HideTime > 0)
            {
                yield return null;
            }

            Player.moveable = true;
        }
    }
}
