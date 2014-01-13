using NRaas.CommonSpace.Options;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.Romance
{
    public class SteadyInteractionLevelSetting : EnumSettingOption<MyLoveBuffLevel, GameObject>, IRomanceOption
    {
        protected override MyLoveBuffLevel Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mSteadyInteractionLevel;
            }
            set
            {
                NRaas.Woohooer.Settings.mSteadyInteractionLevel = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "SteadyInteractionLevel";
        }

        protected override string GetValuePrefix()
        {
            return "MyLoveBuff";
        }

        public override MyLoveBuffLevel Default
        {
            get { return WoohooerTuning.kSteadyInteractionLevel; }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public static bool Satisfies(SimDescription actor, SimDescription target, bool resultOnDefault)
        {
            switch (NRaas.Woohooer.Settings.mSteadyInteractionLevel)
            {
                case MyLoveBuffLevel.AnyRomantic:
                    Relationship relation = Relationship.Get(actor, target, false);
                    if (relation == null) return false;

                    if (relation.AreRomantic()) return true;

                    return (actor.Partner == target);
                case MyLoveBuffLevel.Partner:
                    return (actor.Partner == target);
                case MyLoveBuffLevel.Spouse:
                    return (actor.Genealogy.Spouse == target.Genealogy);
            }

            return resultOnDefault;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            AttractionHelper.UpdateAttractionControllers();

            return base.Run(parameters);
        }
    }
}
