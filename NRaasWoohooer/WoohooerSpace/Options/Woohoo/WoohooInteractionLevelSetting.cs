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

namespace NRaas.WoohooerSpace.Options.Woohoo
{
    public class WoohooInteractionLevelSetting : EnumSettingOption<MyLoveBuffLevel,GameObject>, IWoohooOption, ISpeciesItem
    {
        CASAgeGenderFlags mSpecies;
        
        public WoohooInteractionLevelSetting()
        {}
        public WoohooInteractionLevelSetting(CASAgeGenderFlags species)
        {
            mSpecies = species;
        }

        protected int SpeciesIndex
        {
            get
            {
                return PersistedSettings.GetSpeciesIndex(mSpecies);
            }
        }

        protected override MyLoveBuffLevel Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mWoohooInteractionLevelV2[SpeciesIndex];
            }
            set
            {
                NRaas.Woohooer.Settings.mWoohooInteractionLevelV2[SpeciesIndex] = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "WoohooInteractionLevel";
        }

        protected override string GetValuePrefix()
        {
            return "MyLoveBuff";
        }

        public override MyLoveBuffLevel Default
        {
            get { return MyLoveBuffLevel.Default; }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public static bool Satisfies(Sim actor, Sim target, bool resultOnDefault)
        {
            switch (NRaas.Woohooer.Settings.mWoohooInteractionLevelV2[PersistedSettings.GetSpeciesIndex(actor)])
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

            return resultOnDefault;
        }

        public CASAgeGenderFlags Species
        {
            get { return mSpecies; }
        }

        public ISpeciesItem Clone(CASAgeGenderFlags species)
        {
            return new WoohooInteractionLevelSetting(species);
        }
    }
}
