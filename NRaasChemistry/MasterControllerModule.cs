using NRaas.CommonSpace.Booters;
using NRaas.ChemistrySpace.Booters;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    // This is here to make the deriative search for criteria find this assembly
    public class MasterControllerModule : Common
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        static MasterControllerModule()
        {
            //Bootstrap();
        }

        public MasterControllerModule()
        { }
    }
}
