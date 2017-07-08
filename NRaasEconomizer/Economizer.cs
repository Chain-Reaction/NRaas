using NRaas.CommonSpace;
using NRaas.EconomizerSpace;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class Economizer : Common
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        static PersistedSettings sSettings = null;        

        static Economizer()
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
    }
}
