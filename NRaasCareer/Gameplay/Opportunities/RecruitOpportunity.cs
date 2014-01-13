using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.Gameplay.Opportunities
{
    [Persistable]
    public class RecruitOpportunity : OpportunityEx
    {
        bool mRecruited = false;

        EventListener mListener;

        public RecruitOpportunity()
        { }
        public RecruitOpportunity(Opportunity.OpportunitySharedData sharedData)
            : base(sharedData)
        { }
        public RecruitOpportunity(RecruitOpportunity obj)
            : base(obj.SharedData)
        { }

        public override bool IsComplete()
        {
            return mRecruited;
        }

        protected ListenerAction OnHired(Event e)
        {
            SimDescription sim = null;

            CareerEvent cEvent = e as CareerEvent;
            if ((cEvent != null) && (cEvent.Career != null))
            {
                sim = cEvent.Career.OwnerDescription;
            }

            if (sim == null)
            {
                return ListenerAction.Keep;
            }

            if ((sim != mSuspendedSim) &&
                ((TargetObject == null) || ((TargetObject as Sim).SimDescription != sim)))
            {
                return ListenerAction.Keep;
            }

            if ((Actor.Occupation == null) || (sim.Occupation == null) || (Actor.Occupation.Guid != sim.Occupation.Guid))
            {
                return ListenerAction.Keep;
            }

            mRecruited = true;

            return ListenerAction.Remove;
        }

        public override void ResetListeners()
        {
            base.ResetListeners();

            if (Actor != null)
            {
                mListener = EventTracker.AddListener(EventTypeId.kEventCareerHired, OnHired);
            }
        }

        public override void RemoveAllIncompleteListeners()
        {
            EventTracker.RemoveListener(mListener);
            mListener = null;

            base.RemoveAllIncompleteListeners();
        }

        public override Opportunity Clone()
        {
            return new RecruitOpportunity(this);
        }
    }
}
