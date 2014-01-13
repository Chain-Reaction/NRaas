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
    public class AddRoommate : AddSim
    {
        public override string GetTitlePrefix()
        {
            return "AddRoommate";
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt", false, new object[] { sims.Count })))
            {
                return OptionResult.Failure;
            }

            OptionResult result = Households.AddSim.Perform(Household.ActiveHousehold.LotHome, Household.ActiveHousehold, sims);
            if (result == OptionResult.Failure) return OptionResult.Failure;

            Households.AddRoommate.SetRoommate(Household.ActiveHousehold.LotHome, Household.ActiveHousehold, sims);

            return result;
        }
    }
}
