using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options
{
    public abstract class SpeciesList<T> : OptionList<T>
        where T : class, ISpeciesItem
    {
        CASAgeGenderFlags mSpecies;

        public SpeciesList(CASAgeGenderFlags species)
        {
            mSpecies = species;
        }

        public override string Name
        {
            get
            {
                return Common.Localize("SpeciesRoot:MenuName", false, new object[] { Common.Localize("Species:" + mSpecies) });
            }
        }

        protected CASAgeGenderFlags Species
        {
            get { return mSpecies; }
        }

        public override string GetTitlePrefix()
        {
            return mSpecies.ToString();
        }

        public override List<T> GetOptions()
        {
            List<T> items = new List<T>();
            foreach (T item in Common.DerivativeSearch.Find<T>())
            {
                T result = item.Clone(mSpecies) as T;
                if (result == null) continue;

                if (result.Species != mSpecies) continue;

                items.Add(result);
            }

            return items;
        }
    }
}
