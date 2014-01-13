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
    public class SimCareer : CareerBase<SimCareer.Item>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.Career";
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

                if (me.Occupation != null)
                {
                    results[me.Occupation.Guid] = me.Occupation.Guid;
                }
                else
                {
                    if (SimTypes.IsSpecial(me)) return false;

                    if (!me.TeenOrAbove) return false;

                    results[OccupationNames.Undefined] = OccupationNames.Undefined;
                }

                return true;
            }
        }
    }
}
