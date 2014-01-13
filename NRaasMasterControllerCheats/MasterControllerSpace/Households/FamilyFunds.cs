using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class FamilyFunds : HouseholdFromList, IHouseholdOption
    {
        private int mDelta = 0;

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        public override string GetTitlePrefix()
        {
            return "FamilyFunds";
        }

        protected override bool Allow(Lot lot, Household me)
        {
            if (!base.Allow(lot, me)) return false;

            return (me != null);
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            if (me != null)
            {
                if (!ApplyAll)
                {
                    string text = StringInputDialog.Show(Name, Common.Localize("FamilyFunds:Prompt", false, new object[] { me.Name, me.FamilyFunds }), "0");
                    if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

                    mDelta = 0;
                    if (!int.TryParse(text, out mDelta))
                    {
                        SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                        return OptionResult.Failure;
                    }
                }

                me.ModifyFamilyFunds(mDelta);
            }

            return OptionResult.SuccessClose;
        }
    }
}
