using Crewmeleon.Roles;
using FungleAPI.Event;
using FungleAPI.Event.Vanilla;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crewmeleon.Essential
{
    public static class ChameleonEvents
    {
        [EventRegister]
        public static void OnIntroEnd(IntroEndEvent introEndEvent)
        {
            if (!GameManager.Instance.IsHideAndSeek())
            {
                foreach (PlayerControl playerControl in PlayerControl.AllPlayerControls)
                {
                    if (playerControl.Data.Role.Is(out SeekerRole seekerRole))
                    {
                        seekerRole.StartStun();
                    }
                }
            }
        }
    }
}
