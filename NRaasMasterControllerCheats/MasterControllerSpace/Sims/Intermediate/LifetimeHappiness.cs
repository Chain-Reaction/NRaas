using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class LifetimeHappiness : SimFromList, IIntermediateOption
    {
        int mDelta = 0;

        protected override OptionResult RunResult
        {
            get
            {
                return OptionResult.SuccessRetain;
            }
        }

        public override string GetTitlePrefix()
        {
            return "LifetimeHappiness";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                string text = StringInputDialog.Show(Name, Common.Localize("LifetimeHappiness:Prompt", me.IsFemale, new object[] { me, me.mSpendableHappiness }), "0");
                if ((text == null) || (text == "")) return false;

                mDelta = 0;
                if (!int.TryParse(text, out mDelta))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }
            }

            if ((mDelta > 0) && (me.CreatedSim != null) && (me.CreatedSim.IsSelectable))
            {
                me.IncrementLifetimeHappiness(mDelta);
            }
            else
            {
                me.mSpendableHappiness += mDelta;
                if (me.mSpendableHappiness < 0)
                {
                    me.mSpendableHappiness = 0;
                }

                me.mLifetimeHappiness += mDelta;
                if (me.mLifetimeHappiness < 0)
                {
                    me.mLifetimeHappiness = 0;
                }
            }
            return true;
        }
    }
}
