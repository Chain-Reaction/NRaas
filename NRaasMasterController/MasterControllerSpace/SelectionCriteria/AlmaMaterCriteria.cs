using NRaas.CommonSpace.Options;
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
    public class AlmaMaterCriteria : SelectionTestableOptionList<AlmaMaterCriteria.Item, AlmaMaterCriteria.Values, AlmaMaterCriteria.Values>
    {
        public override string GetTitlePrefix()
        {
            return "AlmaMater";
        }

        protected override bool Allow(SimDescription me, IMiniSimDescription actor)
        {
            if (me.TeenOrBelow) return false;

            return base.Allow(me, actor);
        }

        public struct Values
        {
            public readonly AlmaMater mAlmaMater;
            public readonly string mAlmaMaterName;

            public Values(AlmaMater almaMater, string almaMaterName)
            {
                mAlmaMater = almaMater;
                mAlmaMaterName = almaMaterName;
            }
        }

        public class Item : TestableOption<Values, Values>
        {
            public Item()
            { }
            public Item(AlmaMater value, string name, int count)
                : base(new Values(value, name), name, count)
            { }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<Values, Values> results)
            {
                if (me.TeenOrBelow) return false;

                if (me.AlmaMater == AlmaMater.None) return false;

                Values value = new Values(me.AlmaMater, me.AlmaMaterName);

                results[value] = value;
                return true;
            }

            public override void SetValue(Values value, Values storeType)
            {
                mValue = value;

                mName = value.mAlmaMaterName;
            }
        }
    }
}
