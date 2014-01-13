using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Status
{
    public class HouseholdStatus : SimFromList, IStatusOption
    {
        protected override bool TestValid
        {
            get { return false; }
        }

        protected override OptionResult RunResult
        {
            get { return OptionResult.SuccessRetain; }
        }

        public override string GetTitlePrefix()
        {
            return "HouseStatus";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(MiniSimDescription me)
        {
            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            return Perform(me);
        }
        protected override bool Run(MiniSimDescription me, bool singleSelection)
        {
            return Perform(me);
        }

        public string GetDetails(IMiniSimDescription mini)
        {
            SimDescription sim = mini as SimDescription;
            if (sim != null)
            {
                return NRaas.MasterControllerSpace.Households.StatusBase.GetDetails(sim.LotHome, sim.Household);
            }
            else
            {
                return null;
            }
        }

        protected bool Perform(IMiniSimDescription me)
        {
            SimpleMessageDialog.Show(Name, GetDetails(me));
            return true;
        }
    }
}
