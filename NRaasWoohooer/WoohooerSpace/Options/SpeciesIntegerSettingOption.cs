using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options
{
    public abstract class SpeciesIntegerSettingOption : IntegerSettingOption<GameObject>, ISpeciesItem
    {
        CASAgeGenderFlags mSpecies;

        public SpeciesIntegerSettingOption()
        { }
        public SpeciesIntegerSettingOption(CASAgeGenderFlags species)
        {
            mSpecies = species;
        }

        public CASAgeGenderFlags Species
        {
            get { return mSpecies; }
        }

        protected int SpeciesIndex
        {
            get
            {
                return PersistedSettings.GetSpeciesIndex(mSpecies);
            }
        }

        public override string ExportName
        {
            get
            {
                return base.ExportName + mSpecies;
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP5))
            {
                if (mSpecies != CASAgeGenderFlags.Human) return false;
            }

            return base.Allow(parameters);
        }

        public override void Import(Persistence.Lookup settings)
        {
            foreach (CASAgeGenderFlags species in new CASAgeGenderFlags[] { CASAgeGenderFlags.Human, CASAgeGenderFlags.Horse, CASAgeGenderFlags.Cat, CASAgeGenderFlags.Dog })
            {
                mSpecies = species;

                base.Import(settings);
            }
        }

        public override void Export(Persistence.Lookup settings)
        {
            foreach (CASAgeGenderFlags species in new CASAgeGenderFlags[] { CASAgeGenderFlags.Human, CASAgeGenderFlags.Horse, CASAgeGenderFlags.Cat, CASAgeGenderFlags.Dog })
            {
                mSpecies = species;

                base.Export(settings);
            }
        }

        public abstract ISpeciesItem Clone(CASAgeGenderFlags species);
    }
}
