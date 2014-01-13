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
    public class SimSchool : CareerBase<SimSchool.Item>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.School";
        }

        public class Item : ItemBase
        {
            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<OccupationNames, OccupationNames> results)
            {
                if (me.Household == null) return false;

                if (me.CareerManager == null) return false;

                School school = me.CareerManager.School;

                if (school != null)
                {
                    results[school.Guid] = school.Guid;
                }
                else
                {
                    if (SimTypes.IsSpecial(me)) return false;

                    if (me.YoungAdultOrAbove) return false;

                    results[OccupationNames.Undefined] = OccupationNames.Undefined;
                }

                return true;
            }
        }
    }
}
