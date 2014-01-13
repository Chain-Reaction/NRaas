using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.HybridSpace.Options.SkillLevel
{
    public class BanishSim : SkillLevelBase
    {
        protected override int Value
        {
            get
            {
                return Hybrid.Settings.mSkillLevelBanishSim;
            }
            set
            {
                Hybrid.Settings.mSkillLevelBanishSim = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "BanishSim";
        }
    }
}
