using NRaas.CommonSpace.Helpers;
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
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace
{
    public abstract class IntegerByOccultManagerOptionItem<TManager, TOption> : ByOccultManagerOptionItem<TManager, TOption, int>
        where TManager : StoryProgressionObject
        where TOption : IntegerByOccultManagerOptionItem<TManager, TOption>.SubBaseOption, new()
    {
        public IntegerByOccultManagerOptionItem()
        { }
        public IntegerByOccultManagerOptionItem(TManager manager)
            : base (manager)
        { }

        protected override int Combine(int a, int b)
        {
            return a + b;
        }

        public abstract class SubBaseOption : IntegerManagerOptionItem<TManager>, IOccultOption, INameableOption
        {
            OccultTypes mOccult;

            public SubBaseOption(int value)
                : base(value)
            { }

            public override string Name
            {
                get
                {
                    return OccultTypeHelper.GetLocalizedName(Occult);
                }
            }

            public override string GetStoreKey()
            {
                return base.GetStoreKey() + Occult;
            }

            public OccultTypes Occult
            {
                get
                {
                    return mOccult;
                }
                set
                {
                    mOccult = value;
                }
            }
        }
    }
}

