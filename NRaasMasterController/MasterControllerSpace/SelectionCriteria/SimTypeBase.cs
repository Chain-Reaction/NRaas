using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public abstract class SimTypeBase : SelectionTestableOptionList<SimTypeBase.Item, SimType, SimType>, IDoesNotNeedSpeciesFilter
    {
        public SimTypeBase()
        { }
        public SimTypeBase(List<Item> options)
            : base(options)
        { }

        public override bool IsSpecial
        {
            get { return true; }
        }

        public class Item : TestableOption<SimType, SimType>
        {
            public Item()
            { }
            public Item(SimType type)
            {
                mValue = type;
            }

            protected bool Handled(SimType type)
            {
                switch (type)
                {
                    case SimType.Alien:
                    case SimType.Fairy:
                    case SimType.Frankenstein:
                    case SimType.Genie:
                    case SimType.ImaginaryFriend:
                    case SimType.Mummy:
                    case SimType.Plantsim:
                    case SimType.SimBot:
                    case SimType.Unicorn:
                    case SimType.Vampire:
                    case SimType.Werewolf:
                    case SimType.Witch:
                    case SimType.Zombie:
                    case SimType.Bisexual:
                    case SimType.Straight:
                    case SimType.Gay:
                    case SimType.Occult:
                        return false;
                }

                return true;
            }

            public override bool Get(MiniSimDescription me, IMiniSimDescription actor, Dictionary<SimType, SimType> results)
            {
                foreach (SimType type in Enum.GetValues(typeof(SimType)))
                {
                    if (!Handled(type)) continue;

                    if (!SimTypes.Matches(me, type)) continue;

                    results[type] = type;
                }

                return true;
            }
            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<SimType, SimType> results)
            {
                foreach (SimType type in Enum.GetValues(typeof(SimType)))
                {
                    if (!Handled(type)) continue;

                    if (!SimTypes.Matches(me, type)) continue;

                    results[type] = type;
                }

                return true;
            }

            public override void SetValue(SimType value, SimType storeType)
            {
                mValue = value;

                mName = SimTypes.GetLocalizedName(value);
            }
        }
    }
}
