using NRaas.CommonSpace.Helpers;
using NRaas.HybridSpace;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class Hybrid : Common, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        static PersistedSettings sSettings = null;

        static Hybrid()
        {
            Bootstrap();
        }

        public static PersistedSettings Settings
        {
            get
            {
                if (sSettings == null)
                {
                    sSettings = new PersistedSettings();
                }

                return sSettings;
            }
        }

        public static void ResetSettings()
        {
            sSettings = null;
        }

        public void OnWorldLoadFinished()
        {
            kDebugging = Settings.Debugging;
        }

        public static bool IsValidOccult(Sim sim, List<OccultTypes> types)
        {
            if (sim == null) return false;

            return IsValidOccult(sim.SimDescription, types);
        }
        public static bool IsValidOccult(SimDescription sim, List<OccultTypes> types)
        {
            if (sim == null) return false;

            OccultManager manager = sim.OccultManager;
            if (manager == null) return false;

            foreach (OccultTypes type in types)
            {
                if (manager.HasOccultType(type)) return true;
            }

            return false;
        }
    }
}
