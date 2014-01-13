using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced.Pregnancy
{
    public class DiscontinuePregnancy : SimFromList, IPregnancyOption
    {
        public override string GetTitlePrefix()
        {
            return "DiscontinuePregnancy";
        }

        protected override int GetMaxSelection()
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

            return (me.IsPregnant);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                if (!AcceptCancelDialog.Show(Common.Localize("DiscontinuePregnancy:Prompt", me.IsFemale, new object[] { me })))
                {
                    return false;
                }
            }

            if (me.IsPregnant)
            {
                me.Pregnancy.ClearPregnancyData();

                if (me.CreatedSim != null)
                {
                    me.CreatedSim.BuffManager.RemoveElement(BuffNames.Pregnant);
                }

                Common.Notify(Common.Localize("DiscontinuePregnancy:Success", me.IsFemale, new object[] { me }));
            }
            return true;
        }
    }
}
