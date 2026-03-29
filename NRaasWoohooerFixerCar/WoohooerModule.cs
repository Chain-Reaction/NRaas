using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class WoohooerModule
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        static WoohooerModule()
        { }

        public WoohooerModule()
        { }

        public static List<Sim> GetPotentials(Sim sim, bool woohoo, bool autonomous)
        {
            List<Sim> results = new List<Sim>();

            if (sim.SimDescription == null)
            {
                return results;
            }

            foreach(Sim lotSim in sim.LotCurrent.GetAllActors())
            {
                if (sim.SimDescription == null) continue;

                if (lotSim.SimDescription.IsPet) continue;

                if (sim.SimDescription.Teen && !Woohooer.Settings.AllowTeenAdult(woohoo)) continue;

                string reason;
                GreyedOutTooltipCallback callback = null;
                if(!CommonSocials.CanGetRomantic(sim, lotSim, autonomous, woohoo, true, ref callback, out reason)) continue;

                results.Add(lotSim);
            }

            return results;
        }
    }
}
