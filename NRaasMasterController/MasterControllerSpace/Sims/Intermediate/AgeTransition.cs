using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class AgeTransition : SimFromList, IIntermediateOption
    {
        public override string GetTitlePrefix()
        {
            return null;
        }

        public override string Name
        {
            get
            {
                return Cheats.LocalizeString("TriggerAgeTransition", new object[0]);
            }
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

            if (me.CreatedSim == null) return false;

            if (me.CreatedSim.LotCurrent == null) return false;

            if (me.CreatedSim.LotCurrent.IsWorldLot) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                if (!AcceptCancelDialog.Show(Common.Localize("AgeTransition:Prompt", me.IsFemale, new object[] { me })))
                {
                    return false;
                }
            }

            AgeTransitionTask.Perform(me.CreatedSim);
            return true;
        }
    }
}
