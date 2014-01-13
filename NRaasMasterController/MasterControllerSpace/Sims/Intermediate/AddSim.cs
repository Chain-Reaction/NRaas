using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class AddSim : SimFromList, IIntermediateOption
    {
        public override string GetTitlePrefix()
        {
            return "AddSimHouse";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (Household.ActiveHousehold == null) return false;

            if (Households.AddSim.GetSpace(Household.ActiveHousehold) <= 0) return false;

            return base.Allow(parameters);
        }

        protected override bool PrivateAllow(MiniSimDescription me)
        {
            return true;
        }
        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.Household == Household.ActiveHousehold) return false;

            return base.PrivateAllow(me);
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt", false, new object[] { sims.Count })))
            {
                return OptionResult.Failure;
            }

            return Households.AddSim.Perform(Household.ActiveHousehold.LotHome, Household.ActiveHousehold, sims);
        }
    }
}
