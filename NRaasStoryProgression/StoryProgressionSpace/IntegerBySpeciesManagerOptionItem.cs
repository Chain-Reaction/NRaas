using NRaas.CommonSpace.Options;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace
{
    public abstract class IntegerBySpeciesManagerOptionItem<TManager, TOption> : BySpeciesManagerOptionItem<TManager, TOption, int>
        where TManager : StoryProgressionObject
        where TOption : IntegerBySpeciesManagerOptionItem<TManager, TOption>.SubBaseOption, new()
    {
        public IntegerBySpeciesManagerOptionItem()
        { }
        public IntegerBySpeciesManagerOptionItem(TManager manager)
            : base (manager)
        { }

        protected override int Combine(int a, int b)
        {
            return a + b;
        }

        public abstract class SubBaseOption : IntegerManagerOptionItem<TManager>, ISpeciesOption, INameableOption
        {
            CASAgeGenderFlags mSpecies;

            public SubBaseOption(int value)
                : base(value)
            { }

            public override string Name
            {
                get
                {
                    return Common.Localize("Species:" + Species);
                }
            }

            public override string GetStoreKey()
            {
                return base.GetStoreKey() + Species;
            }

            public CASAgeGenderFlags Species
            {
                get
                {
                    return mSpecies;
                }
                set
                {
                    mSpecies = value;
                }
            }
        }
    }
}

