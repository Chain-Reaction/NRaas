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

namespace NRaas.MasterControllerSpace.Households
{
    public class AddRoommate : AddSim
    {
        public override string GetTitlePrefix()
        {
            return "AddRoommate";
        }

        protected override bool Allow(Lot lot, Household house)
        {
            if (house != Household.ActiveHousehold) return false;

            return base.Allow(lot, house);
        }

        public static void SetRoommate(Lot lot, Household me, List<IMiniSimDescription> sims)
        {
            // In case the household is new to the lot
            if (lot != null)
            {
                me = lot.Household;
            }

            foreach (IMiniSimDescription miniSim in sims)
            {
                SimDescription sim = me.FindMember(miniSim.SimDescriptionId);
                if (sim == null) continue;

                if (Household.RoommateManager.mRoommates.Count >= Household.RoommateManager.mMaxNumRoommates)
                {
                    Household.RoommateManager.mMaxNumRoommates = Household.RoommateManager.mRoommates.Count + 1;
                }

                Household.RoommateManager.AddRoommate(sim, me);
            }
        }

        protected override OptionResult PrivatePerform(Lot lot, Household me, List<IMiniSimDescription> sims)
        {
            OptionResult result = base.PrivatePerform(lot, me, sims);
            if (result == OptionResult.Failure) return OptionResult.Failure;

            SetRoommate(lot, me, sims);

            return result;
        }
    }
}
