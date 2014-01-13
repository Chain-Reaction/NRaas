using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios;
using Sims3.Gameplay.CAS;
using Sims3.UI.Hud;
using System;

namespace NRaas.StoryProgressionSpace.Personalities
{
    public class CustomAccept
    {
        static Common.MethodStore sActivateProfessional = new Common.MethodStore("NRaasWoohooer", "NRaas.WoohooerSpace.Skills.KamaSimtra", "ActivateProfessional", new Type[] { typeof(SimDescription) });
        static Common.MethodStore sActivateRendezvous = new Common.MethodStore("NRaasWoohooer", "NRaas.WoohooerSpace.Skills.KamaSimtra", "ActivateRendezvous", new Type[] { typeof(SimDescription) });

        public static void OnAcceptBike(SimPersonality personality, SimDescription sim)
        {
            sActivateRendezvous.Invoke<bool>(new object[] { sim });
        }

        public static void OnAcceptLestat(SimPersonality personality, SimDescription sim)
        {
            sActivateRendezvous.Invoke<bool>(new object[] { sim });
        }

        public static void OnAcceptGigolo(SimPersonality personality, SimDescription sim)
        {
            sActivateProfessional.Invoke<bool>(new object[] { sim });
            sActivateRendezvous.Invoke<bool>(new object[] { sim });
        }
    }
}
