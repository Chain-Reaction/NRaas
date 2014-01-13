using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Situations
{
    public class GoToLotSituation : RootSituation
    {
        // Fields
        private bool mHaveBothSimsArrivedAtTheLot;
        private AlarmHandle mPulse;
        public readonly Sim mSimA;
        public readonly Sim mSimB;
        public readonly FirstActionDelegate mFirstAction;

        public delegate bool FirstActionDelegate(GoToLotSituation parent, MeetUp meetUp);

        protected GoToLotSituation()
        {
            Sim.sOnLotChangedDelegates += OnLotChanged;

            mPulse = AlarmManager.AddAlarmRepeating((float)Sims3.Gameplay.Situations.InviteToLotSituation.kPulseMinutes, TimeUnit.Minutes, Pulse, (float)Sims3.Gameplay.Situations.InviteToLotSituation.kPulseMinutes, TimeUnit.Minutes, "InviteToLot pulse", AlarmType.NeverPersisted, mSimA);
        }
        public GoToLotSituation(Sim simA, Sim simB, Lot lot, FirstActionDelegate firstAction) 
            : base(lot)
        {
            Sim.sOnLotChangedDelegates += OnLotChanged;

            mFirstAction = firstAction;

            foreach (Situation situation in new List<Situation>(simA.Autonomy.SituationComponent.Situations))
            {
                if (situation is GroupingSituation)
                {
                    try
                    {
                        situation.Exit();
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(mSimA, e);
                    }
                }
            }

            foreach (Situation situation in new List<Situation>(simB.Autonomy.SituationComponent.Situations))
            {
                if (situation is GroupingSituation)
                {
                    try
                    {
                        situation.Exit();
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(mSimB, e);
                    }
                }
            }

            mSimA = simA.AssignRole(this);
            mSimB = simB.AssignRole(this);

            AlarmManager.RemoveAlarm(mPulse);
            mPulse = AlarmManager.AddAlarmRepeating((float)Sims3.Gameplay.Situations.InviteToLotSituation.kPulseMinutes, TimeUnit.Minutes, Pulse, (float)Sims3.Gameplay.Situations.InviteToLotSituation.kPulseMinutes, TimeUnit.Minutes, "InviteToLot pulse", AlarmType.NeverPersisted, mSimA);
 
            SetState(new BothGoToLot(this));
        }

        protected void OnLotChanged(Sim sim, Lot oldLot, Lot newLot)
        {
            try
            {
                if (Lot != oldLot) return;

                if ((mSimA == sim) || (mSimB == sim))
                {
                    Exit();
                }
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
            }
        }

        private bool AreBothSimsOnTheLot()
        {
            return ((mSimA.LotCurrent == Lot) && (mSimB.LotCurrent == Lot));
        }

        public override void CleanUp()
        {
            Sim.sOnLotChangedDelegates -= OnLotChanged;

            mSimA.RemoveRole(this);
            mSimB.RemoveRole(this);
            AlarmManager.RemoveAlarm(mPulse);
            base.CleanUp();
        }

        public static GoToLotSituation FindInviteToLotSituationInvolving(Sim sim)
        {
            foreach (Situation situation in sim.Autonomy.SituationComponent.Situations)
            {
                GoToLotSituation situation2 = situation as GoToLotSituation;
                if ((situation2 != null) && ((situation2.mSimA == sim) || (situation2.mSimB == sim)))
                {
                    return situation2;
                }
            }
            return null;
        }

        private bool MotivesLow(Sim sim)
        {
            if (!sim.Motives.InMotiveDistress)
            {
                return false;
            }
            return true;
        }

        public override void OnParticipantDeleted(Sim participant)
        {
            if ((participant == mSimA) || (participant == mSimB))
            {
                Exit();
            }
        }

        private void Pulse()
        {
            if (mHaveBothSimsArrivedAtTheLot && ((TooLate() || MotivesLow(mSimA)) || ((MotivesLow(mSimB) || SimHasLeftLot(mSimA)) || SimHasLeftLot(mSimB))))
            {
                Exit();
            }
        }

        private static bool IsValid(Sim sim)
        {
            if (sim == null) return false;

            if (sim.SimDescription == null) return false;

            if (!SimTypes.IsDead(sim.SimDescription)) return false;

            return true;
        }

        private bool SimHasLeftLot(Sim sim)
        {
            if (sim.LotCurrent == Lot)
            {
                return false;
            }
            return true;
        }

        private bool TooLate()
        {
            if (!VisitSituation.IsTooLateToVisit())
            {
                return false;
            }
            return true;
        }

        public class BothGoToLot : ChildSituation<GoToLotSituation>
        {
            private AlarmHandle mAlarmHandle;
            private bool mFirst = true;

            protected BothGoToLot()
            {
                mAlarmHandle = AlarmManager.AddAlarm(Sims3.Gameplay.Situations.InviteToLotSituation.kNumHoursForBothGoToLotToWaitBeforeGivingUp, TimeUnit.Hours, TimeOut, "Waiting for Sims to get to lot", AlarmType.NeverPersisted, Parent.mSimA);
            }
            public BothGoToLot(GoToLotSituation parent) 
                : base(parent)
            { }

            public override void CleanUp()
            {
                AlarmManager.RemoveAlarm(mAlarmHandle);
                base.CleanUp();
            }

            public override void Init(GoToLotSituation parent)
            {
                AlarmManager.RemoveAlarm(mAlarmHandle);
                mAlarmHandle = AlarmManager.AddAlarm(Sims3.Gameplay.Situations.InviteToLotSituation.kNumHoursForBothGoToLotToWaitBeforeGivingUp, TimeUnit.Hours, TimeOut, "Waiting for Sims to get to lot", AlarmType.NeverPersisted, Parent.mSimA);

                ForceSituationSpecificInteraction(parent.Lot, parent.mSimA, ManagerSituation.GetVisitInteraction(parent.mSimA, parent.Lot), null, OnRouteToLotSucceeded, OnRouteToLotFailed, InteractionPriorityLevel.UserDirected);
                ForceSituationSpecificInteraction(parent.Lot, parent.mSimB, ManagerSituation.GetVisitInteraction(parent.mSimB, parent.Lot), null, OnRouteToLotSucceeded, OnRouteToLotFailed, InteractionPriorityLevel.UserDirected);
            }

            private void OnRouteToLotFailed(Sim actor, float x)
            {
                if (actor.LotCurrent != Parent.Lot)
                {
                    if (mFirst)
                    {
                        mFirst = false;
                        ForceSituationSpecificInteraction(Parent.Lot, actor, ManagerSituation.GetVisitInteraction(actor, Parent.Lot), null, OnRouteToLotSucceeded, OnRouteToLotFailed, InteractionPriorityLevel.UserDirected);
                    }
                    else
                    {
                        Exit();
                    }
                }
                else
                {
                    OnRouteToLotSucceeded(actor, x);
                }
            }

            private void OnRouteToLotSucceeded(Sim actor, float x)
            {
                if (Parent.AreBothSimsOnTheLot())
                {
                    Parent.SetState(new MeetUp(Parent));
                }
            }

            private void TimeOut()
            {
                Exit();
            }
        }

        public class HangingOut : ChildSituation<GoToLotSituation>
        {
            private AlarmHandle mAlarmHandle;

            protected HangingOut()
            {
                mAlarmHandle = AlarmManager.AddAlarm(Sims3.Gameplay.Situations.InviteToLotSituation.kNumHoursForHangingOutToWaitBeforeExiting, TimeUnit.Hours, TimeOut, "Waiting for Sims to hang out before leaving", AlarmType.NeverPersisted, Parent.mSimA);
            }
            public HangingOut(GoToLotSituation parent) 
                : base(parent)
            { }

            public override void CleanUp()
            {
                Parent.mSimB.SocialComponent.RemoveShortTermDesireToSocializeWith(Parent.mSimA);
                Parent.mSimA.SocialComponent.RemoveShortTermDesireToSocializeWith(Parent.mSimB);
                AlarmManager.RemoveAlarm(mAlarmHandle);
                base.CleanUp();
            }

            public override void Init(GoToLotSituation parent)
            {
                AlarmManager.RemoveAlarm(mAlarmHandle);
                mAlarmHandle = AlarmManager.AddAlarm(Sims3.Gameplay.Situations.InviteToLotSituation.kNumHoursForHangingOutToWaitBeforeExiting, TimeUnit.Hours, TimeOut, "Waiting for Sims to hang out before leaving", AlarmType.NeverPersisted, Parent.mSimA);

                parent.mSimA.SocialComponent.AddShortTermDesireToSocializeWith(parent.mSimB, Sims3.Gameplay.Situations.InviteToLotSituation.kDesireToSocialize);
                parent.mSimB.SocialComponent.AddShortTermDesireToSocializeWith(parent.mSimA, Sims3.Gameplay.Situations.InviteToLotSituation.kDesireToSocialize);
            }

            private void TimeOut()
            {
                Exit();
            }
        }

        public delegate bool OnAction(Sim a, Sim b, GoToLotSituation.MeetUp meetUp, Sim actor, float x);

        public class Action
        {
            GoToLotSituation.MeetUp mMeetUp;

            OnAction mPerform;

            public Action(GoToLotSituation.MeetUp meetUp, OnAction perform)
            {
                mMeetUp = meetUp;
                mPerform = perform;
            }

            public void OnPerform(Sim actor, float x)
            {
                if (!mPerform(mMeetUp.Parent.mSimA, mMeetUp.Parent.mSimB, mMeetUp, actor, x))
                {
                    mMeetUp.OnSocialFailed(actor, x);
                }
            }
        }

        public class MeetUp : ChildSituation<GoToLotSituation>
        {
            private AlarmHandle mAlarmHandle;
            private AlarmHandle mWait;

            protected MeetUp()
            {
                mAlarmHandle = AlarmManager.AddAlarm(Sims3.Gameplay.Situations.InviteToLotSituation.kNumHoursForMeetUpToWaitBeforeGivingUp, TimeUnit.Hours, TimeOut, "Waiting for Sims to get to lot", AlarmType.NeverPersisted, Parent.mSimA);
            }
            public MeetUp(GoToLotSituation parent) 
                : base(parent)
            { }

            public override void CleanUp()
            {
                AlarmManager.RemoveAlarm(mAlarmHandle);
                AlarmManager.RemoveAlarm(mWait);
                base.CleanUp();
            }

            public override void Init(GoToLotSituation parent)
            {
                parent.mHaveBothSimsArrivedAtTheLot = true;

                if (parent.mFirstAction != null)
                {
                    if (!parent.mFirstAction(parent, this))
                    {
                        Exit();
                    }
                }

                AlarmManager.RemoveAlarm(mAlarmHandle);
                mAlarmHandle = AlarmManager.AddAlarm(Sims3.Gameplay.Situations.InviteToLotSituation.kNumHoursForMeetUpToWaitBeforeGivingUp, TimeUnit.Hours, TimeOut, "Waiting for Sims to get to lot", AlarmType.NeverPersisted, Parent.mSimA);
            }

            public void OnSocialFailed(Sim actor, float x)
            {
                mWait = AlarmManager.AddAlarm(15, TimeUnit.Minutes, OnRetry, "Waiting for sims to get act together", AlarmType.NeverPersisted, Parent.mSimA);
            }

            private void OnRetry()
            {
                if ((Parent.mFirstAction != null) && (IsValid(Parent.mSimA)) && (IsValid(Parent.mSimB)))
                {
                    if (Parent.mFirstAction(Parent, this)) return;
                }

                Exit();
            }

            public void OnSocialSucceeded(Sim actor, float x)
            {
                Parent.SetState(new HangingOut(Parent));
            }

            private void TimeOut()
            {
                Parent.SetState(new HangingOut(Parent));
            }
        }
    }
}

