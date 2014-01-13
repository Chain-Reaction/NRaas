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
    public abstract class SkillLevelBase : IntegerSettingOption<GameObject>, ISkillLevelOption
    {
        protected override string GetPrompt()
        {
            string value;
            if (!Common.Localize("SkillLevel:Prompt", false, new object[] { Name }, out value)) return null;

            return value;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
