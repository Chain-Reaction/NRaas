using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class MoneyTransfer : DualSimFromList, IBasicOption
    {
        int mFunds = 0;

        public override string GetTitlePrefix()
        {
            return "MoneyTransfer";
        }

        protected override string GetTitleA()
        {
            return Common.Localize("MoneyTransfer:Source");
        }

        protected override string GetTitleB()
        {
            return Common.Localize("MoneyTransfer:Destination");
        }

        protected override int GetMaxSelectionA()
        {
            return 0;
        }

        protected override int GetMaxSelectionB(IMiniSimDescription sim)
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.Household == null) return false;

            //if (SimTypes.IsSpecial(me)) return false;

            return true;
        }

        protected override bool PrivateAllow(SimDescription a, SimDescription b)
        {
            if (!base.PrivateAllow(a, b)) return false;

            if (a.Household == b.Household) return false;

            return true;
        }

        protected override bool Run(SimDescription a, SimDescription b)
        {
            if (a.Household == null) return true;
            if (b.Household == null) return true;

            if (!ApplyAll)
            {
                string text = StringInputDialog.Show(Name, Common.Localize("MoneyTransfer:Prompt", a.IsFemale, new object[] { a, b }), a.FamilyFunds.ToString(), 256, StringInputDialog.Validation.None);
                if (string.IsNullOrEmpty(text)) return false;

                mFunds = 0;
                if (!int.TryParse(text, out mFunds))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }
            }

            if (mFunds > 0)
            {
                if (mFunds > a.FamilyFunds)
                {
                    Common.Notify(Common.Localize("MoneyTransfer:TooMuch", a.IsFemale, new object[] { a.Household.Name, a.FamilyFunds }));
                    return false;
                }
            }
            else
            {
                if (-mFunds > b.FamilyFunds)
                {
                    Common.Notify(Common.Localize("MoneyTransfer:TooLittle", b.IsFemale, new object[] { b.Household.Name, b.FamilyFunds }));
                    return false;
                }
            }

            a.ModifyFunds (-mFunds);
            b.ModifyFunds (mFunds);

            return true;
        }
    }
}
