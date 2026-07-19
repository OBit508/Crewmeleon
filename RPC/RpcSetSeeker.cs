using AmongUs.Data;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Crewmeleon.Essential;
using Crewmeleon.Roles;
using FungleAPI.Base.Rpc;
using FungleAPI.GameModes;
using FungleAPI.Role;
using FungleAPI.Utilities;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Globalization.CultureInfo;

namespace Crewmeleon.RPC
{
    internal class RpcSetSeeker : AdvancedRpc<PlayerControl>
    {
        public override void Write(MessageWriter messageWriter, PlayerControl data)
        {
            messageWriter.WriteNetObject(data);

            TransformSeeker(data);
        }
        public override void Handle(InnerNetObject innerNetObject, MessageReader messageReader)
        {
            PlayerControl playerControl = messageReader.ReadNetObject<PlayerControl>();

            TransformSeeker(playerControl);
        }
        public static void TransformSeeker(PlayerControl playerControl)
        {
            RoleManager.Instance.SetRole(playerControl, CustomRoleManager.GetRoleType<SeekerRole>());
            playerControl.Data.Role.SpawnTaskHeader(playerControl);
            playerControl.MyPhysics.SetBodyType(playerControl.BodyType);
            if (playerControl.AmOwner)
            {
                HudManager.Instance.MapButton.gameObject.SetActive(true);
                HudManager.Instance.ReportButton.gameObject.SetActive(true);
                HudManager.Instance.UseButton.gameObject.SetActive(true);
            }
            PlayerControl.AllPlayerControls.ForEach(new Action<PlayerControl>(delegate (PlayerControl pc)
            {
                PlayerNameColor.Set(pc);
            }));

            playerControl.Data.Role.SafeCast<SeekerRole>()?.StartStun(false);

            playerControl.StopAllCoroutines();
        }
    }
}
