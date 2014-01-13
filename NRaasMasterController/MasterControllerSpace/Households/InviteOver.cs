using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class InviteOver : HouseholdFromList, IHouseholdOption
    {
        Sim mActor;

        GameObject mTarget;

        public override string GetTitlePrefix()
        {
            return "InviteOver";
        }

        protected override int GetMaxSelection()
        {
            return 1;
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mActor = parameters.mActor as Sim;

            mTarget = parameters.mTarget;

            if (parameters.mTarget is Sim)
            {
                if (parameters.mActor != parameters.mTarget) return false;
            }

            return base.Allow(parameters);
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me == null) return false;

            if (!me.ChildOrAbove) return false;

            return (me.CreatedSim != null);
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            SimDescription targetSim = mActor.SimDescription;
            if (mTarget is Sim)
            {
                targetSim = (mTarget as Sim).SimDescription;
            }
            else if ((me != null) && (CommonSpace.Helpers.Households.NumSims(me) > 0))
            {
                if (Sim.ActiveActor != null)
                {
                    targetSim = Sim.ActiveActor.SimDescription;
                }
                else
                {
                    targetSim = me.AllSimDescriptions[0];
                }
            }

            bool okayed = false;
            List<IMiniSimDescription> sims = new Sims.Basic.InviteOver(this).GetSelection(targetSim, Name, SelectionOption.List, 0, CanApplyAll(), out okayed);
            if ((sims == null) || (sims.Count == 0))
            {
                if (okayed)
                {
                    return OptionResult.SuccessClose;
                }
                else
                {
                    return OptionResult.Failure;
                }
            }

            foreach (IMiniSimDescription miniSim in sims)
            {
                SimDescription simDesc = miniSim as SimDescription;
                if (simDesc == null) continue;

                Sim sim = simDesc.CreatedSim;
                if (sim == null) continue;

                if (sim.LotCurrent == lot) continue;

                sim.InteractionQueue.CancelAllInteractions();

                if (lot.IsCommunityLot)
                {
                    sim.InteractionQueue.AddInteraction(VisitCommunityLot.Singleton.CreateInstance(lot, sim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true), true);
                }
                else
                {
                    sim.InteractionQueue.AddInteraction(VisitLot.Singleton.CreateInstance(lot, sim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true), true);
                }
            }

            return OptionResult.SuccessClose;
        }
    }
}
