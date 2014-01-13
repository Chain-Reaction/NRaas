using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
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
    public class PriorCareer : CareerBase<PriorCareer.Item>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.PriorCareer";
        }

        public class Item : ItemBase
        {
            public Item()
            { }
            public Item(OccupationNames value, string name, int count)
                : base(value, name, count)
            { }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<OccupationNames, OccupationNames> results)
            {
                if (me.Household == null) return false;

                if (me.CareerManager == null) return false;

                if (me.CareerManager.QuitCareers == null) return false;

                foreach (Occupation career in me.CareerManager.QuitCareers.Values)
                {
                    if (career == null) continue;

                    results[career.Guid] = career.Guid;
                }

                return true;
            }
        }
    }
}
