extern alias SP;

using CasteOptions = SP::NRaas.StoryProgressionSpace.CasteOptions;
using SimData = SP::NRaas.StoryProgressionSpace.SimData;

using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class ProgressionCaste : SelectionTestableOptionList<ProgressionCaste.Item, CasteOptions, ulong>, IDoesNotNeedSpeciesFilter
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.ProgressionCaste";
        }

        public class Item : TestableOption<CasteOptions,ulong>
        {
            public Item()
            { }
            public Item(CasteOptions caste, int count)
                : base(caste.ID, caste.Name, count)
            { }

            public override void SetValue(CasteOptions value, ulong storeType)
            {
                mValue = storeType;

                mName = value.Name;
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<ulong, CasteOptions> results)
            {
                SimData data = SP::NRaas.StoryProgression.Main.GetData(me);

                foreach (CasteOptions caste in data.Castes)
                {
                    results[caste.ID] = caste;
                }

                return true;
            }

            public override string DisplayKey
            {
                get { return "Has"; }
            }
        }
    }
}
