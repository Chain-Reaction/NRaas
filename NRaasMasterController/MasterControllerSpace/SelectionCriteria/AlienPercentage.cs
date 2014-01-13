using NRaas.MasterControllerSpace.Demographics;
using NRaas.MasterControllerSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.RealEstate;
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
    public class AlienPercentage : SelectionTestableOptionList<AlienPercentage.Item, int, int>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.AlienPercentage";
        }

        public class Item : TestableOption<int, int>
        {
            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<int, int> results)
            {
                int percent = (int)(me.AlienDNAPercentage * 100f) / 10;

                results[percent] = percent;
                return true;
            }

            public override void SetValue(int value, int storeType)
            {
                mValue = value;

                mName = EAText.GetNumberString(value * 10);
            }
        }
    }
}
