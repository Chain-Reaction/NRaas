using NRaas.CommonSpace.Booters;
using NRaas.VectorSpace;
using NRaas.VectorSpace.Booters;
using NRaas.VectorSpace.Options.Sims;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;

namespace NRaas
{
    public class Vector : Common, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        static PersistedSettings sSettings = null;

        static Vector()
        {
            Bootstrap();

            BooterHelper.Add(new BuffBooter("NRaas.BootStrap", false));
            BooterHelper.Add(new ScoringBooter());
            BooterHelper.Add(new ScoringBooter("VectorMethodFile", "NRaas.BootStrap", false));
            BooterHelper.Add(new SymptomBooter("NRaas.BootStrap"));
            BooterHelper.Add(new ResistanceBooter("NRaas.BootStrap"));
            BooterHelper.Add(new VectorBooter("NRaas.BootStrap"));

            BooterHelper.Add(new SocializingBooter());
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

        // Externalized to Hybrid
        public static void ApplyDisease(SimDescription sim, bool random)
        {
            try
            {
                InfectSetting.Perform(sim, false, random);
            }
            catch (Exception e)
            {
                Common.Exception("ApplyDisease", e);
            }
        }

        // Externalized to Hybrid
        public static void RemoveDisease(SimDescription sim)
        {
            try
            {
                CureSetting.Perform(sim);
            }
            catch (Exception e)
            {
                Common.Exception("RemoveDisease", e);
            }
        }
    }
}
