using FungleAPI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Crewmeleon.GameMode
{
    public class HostImpCommand : BaseChatCommand
    {
        public static bool HostImpostorEnabled;
        public override string CommandName => "HostImpostor";
        public override string[] Arguments { get; } = new string[] { "true / false" };
        public override void ExecuteCommand(IEnumerable<string> args, ref bool cancelSend)
        {
            cancelSend = true;

            if (args.Count() <= 0)
            {
                HudManager.Instance.Chat.AddChatWarning($"{Color.black.ToTextColor()}Erro: Argumentos faltando.</color>");
                return;
            }

            if (bool.TryParse(args.ElementAt(0), out bool result))
            {
                HostImpostorEnabled = result;
                HudManager.Instance.Chat.AddChatWarning($"{Color.black.ToTextColor()}HostImpostor {(result ? "ativado" : "desativado")}.</color>");
            }
            else
            {
                HudManager.Instance.Chat.AddChatWarning($"{Color.black.ToTextColor()}Erro: Falha ao identificar argumentos.</color>");
            }
        }
    }
}
