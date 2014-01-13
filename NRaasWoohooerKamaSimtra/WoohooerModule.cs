using NRaas.CommonSpace.Booters;
using NRaas.WoohooerSpace;
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
        {
            BooterHelper.AddFirst(new BuffBooter("NRaas.KamaSimtra.Buffs", true));
            BooterHelper.AddFirst(new SkillBooter("NRaas.KamaSimtra.Skills", true));
        }

        public WoohooerModule()
        { }
    }
}
