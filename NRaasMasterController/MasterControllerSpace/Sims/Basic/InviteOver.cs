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

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class InviteOver : SimFromList, IBasicOption
    {
        Sim mActor;

        SimFromList mTest;

        public InviteOver()
        { }
        public InviteOver(SimFromList test)
        {
            mTest = test;
        }

        public override string GetTitlePrefix()
        {
            return "InviteOver";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override List<SimSelection.ICriteria> GetCriteria(GameHitParameters<GameObject> parameters)
        {
            if (parameters.mTarget is Sim)
            {
                return SelectionCriteria.SelectionOption.List;
            }
            else
            {
                return base.GetCriteria(parameters);
            }
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AllowRunOnActive
        {
            get { return false; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mActor = parameters.mActor as Sim;

            if (mActor.LotCurrent == null) return false;

            if (mActor.LotCurrent.IsWorldLot) return false;

            Sim targetSim = parameters.mTarget as Sim;
            if (targetSim != null)
            {
                // Bypass base class
                if (targetSim.Household == Household.ActiveHousehold) return true;
            }
            else
            {
                Lot lot = parameters.mTarget as Lot;
                if (lot != null)
                {
                    if (mActor.LotHome == lot) return true;
                }
            }

            return base.Allow(parameters);
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (Household.ActiveHousehold == null) return false;
            // Base class checks IsValidDescription
            //if (!base.Allow(me)) return false;

            if ((me.ToddlerOrBelow) && (me.LotHome != null))
            {
                return false;
            }

            if (mTest != null)
            {
                if (!mTest.Test(me)) return false;
            }

            return true;
        }
        protected override bool PrivateAllow(MiniSimDescription me)
        {
            // Base class checks IsValidDescription
            //if (!base.Allow(me)) return false;

            if (me.ToddlerOrBelow) return false;

            if (mTest != null)
            {
                if (!mTest.Test(me)) return false;
            }

            return true;
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            bool singleSelection = (sims.Count == 1);

            foreach (IMiniSimDescription sim in sims)
            {
                if (sim is SimDescription)
                {
                    Run(sim as SimDescription, singleSelection);
                }
                else if (sim is MiniSimDescription)
                {
                    SimDescription import = MiniSims.ImportWithCheck(sim as MiniSimDescription);
                    if (import != null)
                    {
                        Run(import, singleSelection);
                    }
                }
            }

            return OptionResult.SuccessClose;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (Instantiation.EnsureInstantiate(me, mActor.LotCurrent))
            {
                me.CreatedSim.InteractionQueue.CancelAllInteractions();

                if (me.ToddlerOrBelow)
                {
                    Common.Notify(Common.Localize("InviteOver:Success", me.IsFemale, new object[] { me }), me.CreatedSim.ObjectId);
                    return true;
                }
                else
                {
                    if (me.CreatedSim.InteractionQueue.Add(GoToLot.Singleton.CreateInstance(mActor.LotCurrent, me.CreatedSim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true)))
                    {
                        EventTracker.SendEvent(EventTypeId.kInvitedSimOver, mActor, me.CreatedSim);

                        Common.Notify(Common.Localize("InviteOver:Success", me.IsFemale, new object[] { me }), me.CreatedSim.ObjectId);
                        return true;
                    }
                }
            }

            Common.Notify(Common.Localize("InviteOver:Failure", me.IsFemale, new object[] { me }));
            return true;
        }
    }
}
