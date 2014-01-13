using NRaas.CommonSpace.Helpers;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class RandomOpportunity : SimFromList, IBasicOption
    {
        public override string GetTitlePrefix()
        {
            return "RandomOpportunity";
        }

        protected override bool AllowSpecies(IMiniSimDescription me)
        {
            return me.IsHuman;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.CreatedSim == null) return false;

            if (me.CreatedSim.OpportunityManager == null) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            List<Opportunity> allOpportunities = OpportunityEx.GetAllOpportunities(me.CreatedSim, OpportunityCategory.None);
            if (allOpportunities.Count == 0)
            {
                SimpleMessageDialog.Show(Name, Common.Localize("Opportunity:None"));
                return false;
            }

            return OpportunityEx.Perform(me, RandomUtil.GetRandomObjectFromList(allOpportunities).Guid);
        }
    }
}
