extern alias SP;

using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
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
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class HouseholdOptions : HouseholdFromList, IHouseholdOption
    {
        List<SP.NRaas.StoryProgressionSpace.GenericOptionBase.DefaultableOption> mOptions = null;

        public override string GetTitlePrefix()
        {
            return "ProgressionHouseholdOptions";
        }

        protected override OptionResult RunResult
        {
            get { return OptionResult.SuccessRetain; }
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        public override void Reset()
        {
            mOptions = null;

            base.Reset();
        }

        protected override bool Allow(Lot lot, Household house)
        {
            if (house == null) return false;

            return base.Allow(lot, house);
        }

        protected override OptionResult Run(Lot lot, Household house)
        {
            SP.NRaas.StoryProgressionSpace.GenericOptionBase data = SP.NRaas.StoryProgression.Main.GetHouseOptions(house);

            if (!ApplyAll)
            {
                mOptions = data.ListOptions(SP.NRaas.StoryProgression.Main, Common.Localize("HouseholdOptions:MenuName"), false);
                if (mOptions == null) return OptionResult.Failure;

                foreach (SP.NRaas.StoryProgressionSpace.GenericOptionBase.DefaultableOption option in mOptions)
                {
                    option.Perform();
                }
            }
            else
            {
                if (mOptions == null) return OptionResult.Failure;

                foreach (SP.NRaas.StoryProgressionSpace.GenericOptionBase.DefaultableOption option in mOptions)
                {
                    option.Persist(data);
                }
            }
            return OptionResult.SuccessRetain;
        }
    }
}
