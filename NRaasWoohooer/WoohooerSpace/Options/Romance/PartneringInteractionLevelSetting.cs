using NRaas.CommonSpace.Options;
using NRaas.WoohooerSpace.Options.Romance;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.Romance
{
    public class PartneringInteractionLevelSetting : EnumSettingOption<MyLoveBuffLevel,GameObject>, IRomanceOption
    {
        protected override MyLoveBuffLevel Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mPartneringInteractionLevel;
            }
            set
            {
                NRaas.Woohooer.Settings.mPartneringInteractionLevel = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "PartneringInteractionLevel";
        }

        protected override string GetValuePrefix()
        {
            return "MyLoveBuff";
        }

        public override MyLoveBuffLevel Default
        {
            get { return WoohooerTuning.kPartneringInteractionLevel; }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public static bool Satisfies(Sim actor, Sim target)
        {
            switch (NRaas.Woohooer.Settings.mPartneringInteractionLevel)
            {
                case MyLoveBuffLevel.AnyRomantic:
                    Relationship relation = Relationship.Get(actor, target, false);
                    if (relation == null) return false;

                    if (relation.AreRomantic()) return true;

                    return (actor.Partner == target.SimDescription);
                case MyLoveBuffLevel.Partner:
                    return (actor.Partner == target.SimDescription);
                case MyLoveBuffLevel.Spouse:
                    return (actor.Genealogy.Spouse == target.Genealogy);
            }

            return false;
        }
    }
}
