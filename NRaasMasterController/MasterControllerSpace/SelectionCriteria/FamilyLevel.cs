using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class FamilyLevel : SelectionTestableOptionList<FamilyLevel.Item, int, int>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.FamilyLevel";
        }

        public class Item : TestableOption<int, int>
        {
            public override void SetValue(int value, int storeType)
            {
                mValue = value;

                mName = EAText.GetNumberString(value);
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<int, int> results)
            {
                int level = Genealogies.GetFamilyLevel(me.CASGenealogy);

                results[level] = level;
                return true;
            }
            public override bool Get(MiniSimDescription me, IMiniSimDescription actor, Dictionary<int, int> results)
            {
                int level = Genealogies.GetFamilyLevel(me.CASGenealogy);

                results[level] = level;
                return true;
            }
        }

        public override bool IsSpecial
        {
            get { return true; }
        }
    }
}
