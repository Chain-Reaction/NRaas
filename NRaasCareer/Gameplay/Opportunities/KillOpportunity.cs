using NRaas.CareerSpace.Skills;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Opportunities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;

namespace NRaas.Gameplay.Opportunities
{
    [Persistable]
    public class KillOpportunity : OpportunityEx
    {
        bool mKilled = false;

        public KillOpportunity()
        { }
        public KillOpportunity(Opportunity.OpportunitySharedData sharedData)
            : base(sharedData)
        { }
        public KillOpportunity(KillOpportunity obj)
            : base(obj.SharedData)
        { }

        public override bool IsComplete()
        {
            return mKilled;
        }

        protected ListenerAction OnKill(Event e)
        {
            try
            {
                SimDescription sim = null;

                SimDescriptionEvent event2 = e as SimDescriptionEvent;
                if (event2 != null)
                {
                    sim = event2.SimDescription;
                }
                else
                {
                    MiniSimDescriptionEvent event3 = e as MiniSimDescriptionEvent;
                    if (event3 != null)
                    {
                        sim = event3.MiniSimDescription as SimDescription;
                    }
                }

                if (!TargetDead(sim))
                {
                    return ListenerAction.Keep;
                }
                else
                {
                    return ListenerAction.Remove;
                }
            }
            catch (Exception exception)
            {
                Common.Exception(e.Actor, e.TargetObject, exception);
                return ListenerAction.Keep;
            }
        }

        protected bool TargetDead(SimDescription sim)
        {
            if (sim == null)
            {
                return false;
            }

            if ((sim != mSuspendedSim) &&
                ((TargetObject == null) || ((TargetObject as Sim).SimDescription != sim)))
            {
                return false;
            }

            mKilled = true;

            Assassination skill = Assassination.EnsureSkill(Actor);
            if (skill != null)
            {
                skill.AddActualKill(sim, skill.IsPotential(sim), false);
            }

            mbTargetDeleted = true;
            return true;
        }

        public override void RemoveAllIncompleteListeners()
        {
            base.RemoveAllIncompleteListeners();

            if ((IsTargetDeleted()) && (!mKilled))
            {
                SimDescription sim = mSuspendedSim;
                if (TargetObject is Sim)
                {
                    sim = (TargetObject as Sim).SimDescription;
                }

                if (TargetDead(sim))
                {
                    OpportunityNames names;
                    OnCompletion(out names);
                    if (names != OpportunityNames.Undefined)
                    {
                        OpportunityManager.GetStaticOpportunity(names);

                        OpportunityManager.OpportunityPropagateInfo item = null;
                        OpportunityManager.OpportunityPropagateInfo info2 = null;
                        item = new OpportunityManager.OpportunityPropagateInfo(SourceObject, SourceData, SourceType, CustomSource, OriginalSource);
                        info2 = new OpportunityManager.OpportunityPropagateInfo(TargetObject, TargetData, TargetType, CustomTarget, OriginalTarget);
                        List<OpportunityNames> list8 = new List<OpportunityNames>(ParentOpportunities);
                        list8.Add(Guid);

                        Opportunity opportunity3;
                        if (this.Actor.OpportunityManager.AddOpportunityNow(names, item, info2, list8, !TriggerQuietly, out opportunity3))
                        {
                            opportunity3.WorldStartedIn = WorldStartedIn;
                        }
                    }
                }
            }
        }

        public new ListenerAction TargetDeletedDelegate(Event e)
        {
            try
            {
                if (!TrySuspendOpportunity())
                {
                    bool flag = true;
                    foreach (EventListener listener in mListenerList)
                    {
                        if (!listener.IsCompleted)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (!flag || (((mCompletionListener != null) && !mCompletionListener.IsCompleted) && (mCompletionListener.TargetObject == e.TargetObject)))
                    {
                        mbTargetDeleted = true;
                    }
                }

                EventTracker.RemoveListener(mSimCreatedListener);
                mSimCreatedListener = EventTracker.AddListener(EventTypeId.kSimInstantiated, SimCreatedEx);

                mTargetDeletedListener = null;
            }
            catch (Exception exception)
            {
                Common.Exception(e.Actor, e.TargetObject, exception);
            }
            return ListenerAction.Remove;
        }

        public override void ResetListeners ()
        {
            base.ResetListeners();

            if (mSimDiedListener != null)
            {
                EventTracker.RemoveListener(mSimDiedListener);
                mSimDiedListener = null;
            }

            mSimDiedListener = EventTracker.AddListener(EventTypeId.kSimDied, OnKill);

            if (mSimDescriptionDisposedListener != null)
            {
                EventTracker.RemoveListener(mSimDescriptionDisposedListener);
                mSimDescriptionDisposedListener = null;
            }

            mSimDescriptionDisposedListener = EventTracker.AddListener(EventTypeId.kSimDescriptionDisposed, OnKill);

            if (mTargetDeletedListener != null)
            {
                EventTracker.RemoveListener(mTargetDeletedListener);
                mTargetDeletedListener = null;
            }

            if (TargetObject != null)
            {
                mTargetDeletedListener = EventTracker.AddListener(EventTypeId.kEventObjectDisposed, TargetDeletedDelegate, null, TargetObject);
            }
        }

        public override Opportunity Clone()
        {
            return new KillOpportunity(this);
        }
    }
}
