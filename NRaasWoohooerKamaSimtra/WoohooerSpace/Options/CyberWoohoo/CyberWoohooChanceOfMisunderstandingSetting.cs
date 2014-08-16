using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.CyberWoohoo
{
    public class CyberWoohooChanceOfMisunderstandingSetting : IntegerSettingOption<GameObject>, ICyberWoohooOption
    {
        protected override int Value
        {
            get
            {
                return Skills.KamaSimtra.Settings.mCyberWoohooChanceOfMisunderstanding;
            }
            set
            {
                Skills.KamaSimtra.Settings.mCyberWoohooChanceOfMisunderstanding = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "CyberWoohooChanceOfMisunderstanding";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (Woohooer.Settings.mGenderPreferenceForUserDirectedV2) return false;

            return base.Allow(parameters);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
